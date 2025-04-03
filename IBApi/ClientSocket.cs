using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using IBApi.Interfaces;
using Microsoft.VisualBasic;

namespace IBApi
{
    public class ClientSocket(IWrapper wrapper, IReaderSignal readerSignal)
        : IClient,
            IClientMsgSink
    {
        public int ServerVersion { get; set; }
        public ITransport? SocketTransport { get; set; }
        public IWrapper Wrapper { get; } = wrapper;
        public bool IsConnected { get; set; }
        public int ClientId { get; set; } = -1;
        public bool ExtraAuth { get; set; }
        public bool UseV100Plus { get; set; }
        public string? ConnectOptions { get; private set; }
        public bool AllowRedirect { get; set; }
        public int Port
        {
            get => port;
        }
        public TcpClient? TcpClient { get; private set; }
        public IReaderSignal ReaderSignal { get; } = readerSignal;
        public bool AsyncConnect { get; set; }
        public string OptionalCapabilities { get; set; } = string.Empty;
        public string ServerTime { get; private set; } = string.Empty;
        public Stream? TcpStream { get; private set; }
        public int RedirectCount { get; private set; }

        private int port;
        private static readonly string encodedVersion =
            Constants.MinVersion
            + (
                Constants.MaxVersion != Constants.MinVersion
                    ? $"..{Constants.MaxVersion}"
                    : string.Empty
            );

        void IClientMsgSink.ServerVersion(int version, string time)
        {
            ServerVersion = version;

            if (!UseV100Plus)
            {
                if (!CheckServerVersion(MinServerVer.MIN_VERSION, ""))
                {
                    ReportUpdateTWS(Util.CurrentTimeMillis(), "");
                    return;
                }
            }
            else if (ServerVersion < Constants.MinVersion || ServerVersion > Constants.MaxVersion)
            {
                Wrapper.Error(
                    ClientId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UNSUPPORTED_VERSION.Code,
                    ClientErrors.UNSUPPORTED_VERSION.Message,
                    ""
                );
                return;
            }

            if (ServerVersion >= 3)
            {
                if (ServerVersion < MinServerVer.LINKING)
                {
                    var buf = new List<byte>();

                    buf.AddRange(Encoding.UTF8.GetBytes(ClientId.ToString()));
                    buf.Add(Constants.EOL);

                    if (SocketTransport != null)
                    {
                        SocketTransport.Send(new EMessage([.. buf]));
                    }
                }
            }

            ServerTime = time;
            IsConnected = true;

            if (!AsyncConnect)
            {
                StartApi();
            }
        }

        private Stream CreateClientStream(string host, int port)
        {
            TcpClient = new TcpClient(host, port);
            SetKeepAlive(true, 2000, 100);
            TcpClient.NoDelay = true;

            return TcpClient.GetStream();
        }

        public void Connect(string host, int port, int clientId, bool extraAuth = false)
        {
            try
            {
                ValidateInvalidSymbols(host);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            if (IsConnected)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    ClientErrors.AlreadyConnected.Code,
                    ClientErrors.AlreadyConnected.Message,
                    ""
                );
                return;
            }
            try
            {
                TcpStream = CreateClientStream(host, port);
                this.port = port;
                SocketTransport = new IBSocket(TcpStream);

                ClientId = clientId;
                ExtraAuth = extraAuth;

                SendConnectRequest();

                if (!AsyncConnect)
                {
                    EReader eReader = new(this, ReaderSignal);

                    while (ServerVersion == 0 && eReader.putMessageToQueue())
                    {
                        ReaderSignal.WaitForSignal();
                        eReader.ProcessMsgs();
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                Wrapper.Error(ane);
            }
            catch (SocketException se)
            {
                Wrapper.Error(se);
            }
            catch (ClientException e)
            {
                var cmp = e.Err;

                Wrapper.Error(-1, Util.CurrentTimeMillis(), cmp.Code, cmp.Message, "");
            }
            catch (Exception e)
            {
                Wrapper.Error(e);
            }
        }

        public uint PrepareBuffer(BinaryWriter paramsList)
        {
            var rval = (uint)paramsList.BaseStream.Position;

            if (UseV100Plus)
                paramsList.Write(0);

            return rval;
        }

        public void CloseAndSend(BinaryWriter request, uint lengthPos)
        {
            if (UseV100Plus)
            {
                request.Seek((int)lengthPos, SeekOrigin.Begin);
                request.Write(
                    IPAddress.HostToNetworkOrder(
                        (int)(request.BaseStream.Length - lengthPos - sizeof(int))
                    )
                );
            }

            request.Seek(0, SeekOrigin.Begin);

            var buf = new MemoryStream();
            try
            {
                request.BaseStream.CopyTo(buf);

                if (SocketTransport != null)
                {
                    SocketTransport.Send(new EMessage(buf.ToArray()));
                }
            }
            finally
            {
                buf.Dispose();
            }
        }

        public void Redirect(string host)
        {
            if (!AllowRedirect)
            {
                Wrapper.Error(
                    ClientId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.CONNECT_FAIL.Code,
                    ClientErrors.CONNECT_FAIL.Message,
                    ""
                );
                return;
            }

            var srv = host.Split(':');

            if (srv.Length > 1 && !int.TryParse(srv[1], out port))
                throw new ClientException(ClientErrors.BAD_MESSAGE);

            ++RedirectCount;

            if (RedirectCount > Constants.REDIRECT_COUNT_MAX)
            {
                Disconnect();
                Wrapper.Error(
                    ClientId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.CONNECT_FAIL.Code,
                    "Redirect count exceeded",
                    ""
                );
                return;
            }

            Disconnect(false);
            Connect(srv[0], port, ClientId, ExtraAuth);
        }

        public void Disconnect(bool resetState = true)
        {
            if (resetState)
            {
                RedirectCount = 0;
            }
            TcpClient = null;

            if (SocketTransport == null)
            {
                return;
            }

            if (resetState)
            {
                IsConnected = false;
                ExtraAuth = false;
                ClientId = -1;
                ServerVersion = 0;
                OptionalCapabilities = string.Empty;
            }

            TcpStream?.Close();

            if (resetState)
                Wrapper.ConnectionClosed();
        }

        private void SetKeepAlive(bool on, uint keepAliveTime, uint keepAliveInterval)
        {
            if (TcpClient != null)
            {
                TcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    on
                );
                TcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Tcp,
                    SocketOptionName.TcpKeepAliveTime,
                    Math.Max(keepAliveTime / 1000, 1)
                );
                TcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Tcp,
                    SocketOptionName.TcpKeepAliveInterval,
                    Math.Max(keepAliveInterval / 1000, 1)
                );
                TcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Tcp,
                    SocketOptionName.TcpKeepAliveRetryCount,
                    5
                );
            }
        }

        internal bool Poll(int timeout)
        {
            if (TcpClient == null)
            {
                return false;
            }

            return TcpClient.Client.Poll(timeout, SelectMode.SelectRead);
        }

        public void SetConnectOptions(string connectOptions)
        {
            if (IsConnected)
            {
                Wrapper.Error(
                    ClientId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.AlreadyConnected.Code,
                    ClientErrors.AlreadyConnected.Message,
                    ""
                );
                return;
            }

            ConnectOptions = connectOptions;
        }

        public void DisableUseV100Plus()
        {
            UseV100Plus = false;
            ConnectOptions = "";
        }

        public void SendConnectRequest()
        {
            try
            {
                if (UseV100Plus)
                {
                    var paramsList = new BinaryWriter(new MemoryStream());

                    paramsList.AddParameter("API");

                    var lengthPos = PrepareBuffer(paramsList);

                    paramsList.Write(
                        Encoding.ASCII.GetBytes($"v{encodedVersion}{' '}{ConnectOptions}")
                    );

                    CloseAndSend(paramsList, lengthPos);
                }
                else
                {
                    var buf = new List<byte>();

                    buf.AddRange(Encoding.UTF8.GetBytes(Constants.ClientVersion.ToString()));
                    buf.Add(Constants.EOL);

                    if (SocketTransport != null)
                    {
                        SocketTransport.Send(new EMessage(buf.ToArray()));
                    }
                }
            }
            catch (IOException)
            {
                Wrapper.Error(
                    ClientId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.CONNECT_FAIL.Code,
                    ClientErrors.CONNECT_FAIL.Message,
                    ""
                );
                throw;
            }
        }

        public void ValidateInvalidSymbols(string host)
        {
            if (host != null && !IBParamsList.IsAsciiPrintable(host))
            {
                throw new ClientException(ClientErrors.INVALID_SYMBOL, host);
            }
            if (ConnectOptions != null && !IBParamsList.IsAsciiPrintable(ConnectOptions))
            {
                throw new ClientException(ClientErrors.INVALID_SYMBOL, ConnectOptions);
            }
            if (
                OptionalCapabilities != null
                && !IBParamsList.IsAsciiPrintable(OptionalCapabilities)
            )
            {
                throw new ClientException(ClientErrors.INVALID_SYMBOL, OptionalCapabilities);
            }
        }

        public static bool UseProtoBuf(int serverVersion, OutgoingMessages outgoingMessage)
        {
            return serverVersion >= MinServerVer.MIN_SERVER_VER_PROTOBUF
                && Constants.PROTOBUF_MSG_IDS.Contains(outgoingMessage);
        }

        public void StartApi()
        {
            if (!CheckConnection())
                return;

            const int VERSION = 2;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.StartApi, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(ClientId);

                if (ServerVersion >= MinServerVer.OPTIONAL_CAPABILITIES)
                    paramsList.AddParameter(OptionalCapabilities);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos);
        }

        public void Close() => Disconnect();

        public void ReqCompletedOrders(bool apiOnly)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.COMPLETED_ORDERS,
                    " It does not support completed orders requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqCompletedOrders, ServerVersion);
            paramsList.AddParameter(apiOnly);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCOMPLETEDORDERS);
        }

        public void CancelTickByTickData(int requestId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.TICK_BY_TICK,
                    " It does not support tick-by-tick cancels."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelTickByTickData, ServerVersion);
            paramsList.AddParameter(requestId);

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_CANCELTICKBYTICKDATA
            );
        }

        public void ReqTickByTickData(
            int requestId,
            Contract contract,
            string tickType,
            int numberOfTicks,
            bool ignoreSize
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.TICK_BY_TICK,
                    " It does not support tick-by-tick requests."
                )
            )
                return;
            if (
                (numberOfTicks != 0 || ignoreSize)
                && !CheckServerVersion(
                    MinServerVer.TICK_BY_TICK_IGNORE_SIZE,
                    " It does not support ignoreSize and numberOfTicks parameters in tick-by-tick requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqTickByTickData, ServerVersion);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(tickType);

                if (ServerVersion >= MinServerVer.TICK_BY_TICK_IGNORE_SIZE)
                {
                    paramsList.AddParameter(numberOfTicks);
                    paramsList.AddParameter(ignoreSize);
                }
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_REQTICKBYTICKDATA
            );
        }

        public void CancelHistoricalData(int reqId)
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(24, " It does not support historical data cancellations."))
                return;
            const int VERSION = 1;
            //No server version validation takes place here since minimum is already higher
            SendCancelRequest(
                OutgoingMessages.CancelHistoricalData,
                VERSION,
                reqId,
                ClientErrors.FAIL_SEND_CANHISTDATA,
                ServerVersion
            );
        }

        public void CalculateImpliedVolatility(
            int reqId,
            Contract contract,
            double optionPrice,
            double underPrice,
            //reserved for future use, must be blank
            List<TagValue> impliedVolatilityOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_CALC_IMPLIED_VOLAT,
                    " It does not support calculate implied volatility."
                )
            )
                return;
            if (
                !Util.StringIsEmpty(contract.TradingClass)
                && !CheckServerVersion(MinServerVer.TRADING_CLASS, "")
            )
                return;

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqCalcImpliedVolat, ServerVersion);
                paramsList.AddParameter(version);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(optionPrice);
                paramsList.AddParameter(underPrice);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(impliedVolatilityOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCALCIMPLIEDVOLAT);
        }

        public void CalculateOptionPrice(
            int reqId,
            Contract contract,
            double volatility,
            double underPrice,
            //reserved for future use, must be blank
            List<TagValue> optionPriceOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_CALC_OPTION_PRICE,
                    " It does not support calculation price requests."
                )
            )
                return;
            if (
                !Util.StringIsEmpty(contract.TradingClass)
                && !CheckServerVersion(
                    MinServerVer.REQ_CALC_OPTION_PRICE,
                    " It does not support tradingClass parameter in calculateOptionPrice."
                )
            )
                return;

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqCalcOptionPrice, ServerVersion);
                paramsList.AddParameter(version);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(volatility);
                paramsList.AddParameter(underPrice);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(optionPriceOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCALCOPTIONPRICE);
        }

        public void CancelAccountSummary(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.ACCT_SUMMARY,
                    " It does not support account summary cancellation."
                )
            )
                return;
            SendCancelRequest(
                OutgoingMessages.CancelAccountSummary,
                1,
                reqId,
                ClientErrors.FAIL_SEND_CANACCOUNTDATA,
                ServerVersion
            );
        }

        public void CancelCalculateImpliedVolatility(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.CANCEL_CALC_IMPLIED_VOLAT,
                    " It does not support calculate implied volatility cancellation."
                )
            )
                return;
            SendCancelRequest(
                OutgoingMessages.CancelImpliedVolatility,
                1,
                reqId,
                ClientErrors.FAIL_SEND_CANCALCIMPLIEDVOLAT,
                ServerVersion
            );
        }

        public void CancelCalculateOptionPrice(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.CANCEL_CALC_OPTION_PRICE,
                    " It does not support calculate option price cancellation."
                )
            )
                return;
            SendCancelRequest(
                OutgoingMessages.CancelOptionPrice,
                1,
                reqId,
                ClientErrors.FAIL_SEND_CANCALCOPTIONPRICE,
                ServerVersion
            );
        }

        public void CancelFundamentalData(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.FUNDAMENTAL_DATA,
                    " It does not support fundamental data requests."
                )
            )
                return;
            SendCancelRequest(
                OutgoingMessages.CancelFundamentalData,
                1,
                reqId,
                ClientErrors.FAIL_SEND_CANFUNDDATA,
                ServerVersion
            );
        }

        public void CancelMktData(int tickerId)
        {
            if (!CheckConnection())
                return;

            SendCancelRequest(
                OutgoingMessages.CancelMarketData,
                1,
                tickerId,
                ClientErrors.FAIL_SEND_CANMKT,
                ServerVersion
            );
        }

        public void CancelMktDepth(int tickerId, bool isSmartDepth)
        {
            if (!CheckConnection())
                return;

            if (
                isSmartDepth
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.SMART_DEPTH,
                    " It does not support SMART depth cancel."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelMarketDepth, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(tickerId);
            if (ServerVersion >= MinServerVer.SMART_DEPTH)
                paramsList.AddParameter(isSmartDepth);

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_CANMKTDEPTH);
        }

        public void CancelNewsBulletin()
        {
            if (!CheckConnection())
                return;
            SendCancelRequest(
                OutgoingMessages.CancelNewsBulletin,
                1,
                ClientErrors.FAIL_SEND_CORDER,
                ServerVersion
            );
        }

        public void CancelOrder(int orderId, OrderCancel orderCancel)
        {
            if (!CheckConnection())
                return;
            if (
                !IsEmpty(orderCancel.ManualOrderCancelTime)
                && !CheckServerVersion(
                    orderId,
                    MinServerVer.MANUAL_ORDER_TIME,
                    " It does not support manual order cancel time attribute"
                )
            )
                return;

            if (
                (
                    !IsEmpty(orderCancel.ExtOperator)
                    || orderCancel.ManualOrderIndicator != int.MaxValue
                )
                && !CheckServerVersion(
                    orderId,
                    MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS,
                    " It does not support ext operator and manual order indicator parameters"
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.CancelOrder, ServerVersion);
                if (ServerVersion < MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS)
                {
                    paramsList.AddParameter(VERSION);
                }
                paramsList.AddParameter(orderId);
                if (ServerVersion >= MinServerVer.MANUAL_ORDER_TIME)
                    paramsList.AddParameter(orderCancel.ManualOrderCancelTime);

                if (
                    ServerVersion >= MinServerVer.MIN_SERVER_VER_RFQ_FIELDS
                    && ServerVersion < MinServerVer.MIN_SERVER_VER_UNDO_RFQ_FIELDS
                )
                {
                    paramsList.AddParameter("");
                    paramsList.AddParameter("");
                    paramsList.AddParameter(int.MaxValue);
                }
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS)
                {
                    paramsList.AddParameter(orderCancel.ExtOperator);
                    paramsList.AddParameter(orderCancel.ManualOrderIndicator);
                }
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    orderId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CANCELPNL);
        }

        public void CancelPositions()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.ACCT_SUMMARY,
                    " It does not support position cancellation."
                )
            )
                return;

            SendCancelRequest(
                OutgoingMessages.CancelPositions,
                1,
                ClientErrors.FAIL_SEND_CANPOSITIONS,
                ServerVersion
            );
        }

        public void CancelRealTimeBars(int tickerId)
        {
            if (!CheckConnection())
                return;

            SendCancelRequest(
                OutgoingMessages.CancelRealTimeBars,
                1,
                tickerId,
                ClientErrors.FAIL_SEND_CANRTBARS,
                ServerVersion
            );
        }

        public void CancelScannerSubscription(int tickerId)
        {
            if (!CheckConnection())
                return;

            SendCancelRequest(
                OutgoingMessages.CancelScannerSubscription,
                1,
                tickerId,
                ClientErrors.FAIL_SEND_CANSCANNER,
                ServerVersion
            );
        }

        public void ExerciseOptions(
            int tickerId,
            Contract contract,
            int exerciseAction,
            int exerciseQuantity,
            string account,
            int ovrd,
            string manualOrderTime,
            string customerAccount,
            bool professionalCustomer
        )
        {
            //WARN needs to be tested!
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(21, " It does not support options exercise from the API."))
                return;
            if (
                (!Util.StringIsEmpty(contract.TradingClass) || contract.ConId > 0)
                && !CheckServerVersion(
                    MinServerVer.TRADING_CLASS,
                    " It does not support conId not tradingClass parameter when exercising options."
                )
            )
            {
                return;
            }

            if (
                (!Util.StringIsEmpty(manualOrderTime))
                && !CheckServerVersion(
                    MinServerVer.MIN_SERVER_VER_MANUAL_ORDER_TIME_EXERCISE_OPTIONS,
                    " It does not support manual order time parameter when exercising options."
                )
            )
            {
                return;
            }

            if (
                (!Util.StringIsEmpty(customerAccount))
                && !CheckServerVersion(
                    MinServerVer.MIN_SERVER_VER_CUSTOMER_ACCOUNT,
                    " It does not support customer account parameter when exercising options."
                )
            )
            {
                return;
            }

            if (
                professionalCustomer
                && !CheckServerVersion(
                    MinServerVer.MIN_SERVER_VER_PROFESSIONAL_CUSTOMER,
                    " It does not support professional customer parameter when exercising options."
                )
            )
            {
                return;
            }

            var VERSION = 2;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ExerciseOptions, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(tickerId);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(exerciseAction);
                paramsList.AddParameter(exerciseQuantity);
                paramsList.AddParameter(account);
                paramsList.AddParameter(ovrd);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_MANUAL_ORDER_TIME_EXERCISE_OPTIONS)
                    paramsList.AddParameter(manualOrderTime);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_CUSTOMER_ACCOUNT)
                    paramsList.AddParameter(customerAccount);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_PROFESSIONAL_CUSTOMER)
                    paramsList.AddParameter(professionalCustomer);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_GENERIC);
        }

        public void PlaceOrder(int id, Contract contract, Order order)
        {
            if (!CheckConnection())
                return;
            if (!VerifyOrder(order, id, StringsAreEqual(Constants.BagSecType, contract.SecType)))
                return;
            if (!VerifyOrderContract(contract, id))
                return;

            var MsgVersion = ServerVersion < MinServerVer.NOT_HELD ? 27 : 45;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.PlaceOrder, ServerVersion);
                if (ServerVersion < MinServerVer.ORDER_CONTAINER)
                    paramsList.AddParameter(MsgVersion);
                paramsList.AddParameter(id);
                if (ServerVersion >= MinServerVer.PLACE_ORDER_CONID)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                if (ServerVersion >= 15)
                    paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                if (ServerVersion >= 14)
                    paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                if (ServerVersion >= 2)
                    paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                if (ServerVersion >= MinServerVer.SEC_ID_TYPE)
                {
                    paramsList.AddParameter(contract.SecIdType);
                    paramsList.AddParameter(contract.SecId);
                }

                // paramsList.AddParameter main order fields
                paramsList.AddParameter(order.Action);

                if (ServerVersion >= MinServerVer.FRACTIONAL_POSITIONS)
                    paramsList.AddParameter(order.TotalQuantity);
                else
                    paramsList.AddParameter((int)order.TotalQuantity);

                paramsList.AddParameter(order.OrderType);
                if (ServerVersion < MinServerVer.ORDER_COMBO_LEGS_PRICE)
                    paramsList.AddParameter(order.LmtPrice == double.MaxValue ? 0 : order.LmtPrice);
                else
                    paramsList.AddParameterMax(order.LmtPrice);
                if (ServerVersion < MinServerVer.TRAILING_PERCENT)
                    paramsList.AddParameter(order.AuxPrice == double.MaxValue ? 0 : order.AuxPrice);
                else
                    paramsList.AddParameterMax(order.AuxPrice);

                // paramsList.AddParameter extended order fields
                paramsList.AddParameter(order.Tif);
                paramsList.AddParameter(order.OcaGroup);
                paramsList.AddParameter(order.Account);
                paramsList.AddParameter(order.OpenClose);
                paramsList.AddParameter(order.Origin);
                paramsList.AddParameter(order.OrderRef);
                paramsList.AddParameter(order.Transmit);
                if (ServerVersion >= 4)
                    paramsList.AddParameter(order.ParentId);

                if (ServerVersion >= 5)
                {
                    paramsList.AddParameter(order.BlockOrder);
                    paramsList.AddParameter(order.SweepToFill);
                    paramsList.AddParameter(order.DisplaySize);
                    paramsList.AddParameter(order.TriggerMethod);
                    if (ServerVersion < 38)
                        paramsList.AddParameter( /* order.ignoreRth */
                            false
                        ); // will never happen
                    else
                        paramsList.AddParameter(order.OutsideRth);
                }
                if (ServerVersion >= 7)
                    paramsList.AddParameter(order.Hidden);

                // paramsList.AddParameter combo legs for BAG requests
                var isBag = StringsAreEqual(Constants.BagSecType, contract.SecType);
                if (ServerVersion >= 8 && isBag)
                {
                    if (contract.ComboLegs == null)
                    {
                        paramsList.AddParameter(0);
                    }
                    else
                    {
                        paramsList.AddParameter(contract.ComboLegs.Count);

                        ComboLeg comboLeg;
                        for (var i = 0; i < contract.ComboLegs.Count; i++)
                        {
                            comboLeg = contract.ComboLegs[i];
                            paramsList.AddParameter(comboLeg.ConId);
                            paramsList.AddParameter(comboLeg.Ratio);
                            paramsList.AddParameter(comboLeg.Action);
                            paramsList.AddParameter(comboLeg.Exchange);
                            paramsList.AddParameter(comboLeg.OpenClose);

                            if (ServerVersion >= MinServerVer.SSHORT_COMBO_LEGS)
                            {
                                paramsList.AddParameter(comboLeg.ShortSaleSlot);
                                paramsList.AddParameter(comboLeg.DesignatedLocation);
                            }
                            if (ServerVersion >= MinServerVer.SSHORTX_OLD)
                            {
                                paramsList.AddParameter(comboLeg.ExemptCode);
                            }
                        }
                    }
                }

                // add order combo legs for BAG requests
                if (ServerVersion >= MinServerVer.ORDER_COMBO_LEGS_PRICE && isBag)
                {
                    if (order.OrderComboLegs == null)
                    {
                        paramsList.AddParameter(0);
                    }
                    else
                    {
                        paramsList.AddParameter(order.OrderComboLegs.Count);

                        for (var i = 0; i < order.OrderComboLegs.Count; i++)
                        {
                            var orderComboLeg = order.OrderComboLegs[i];
                            paramsList.AddParameterMax(orderComboLeg.Price);
                        }
                    }
                }

                if (ServerVersion >= MinServerVer.SMART_COMBO_ROUTING_PARAMS && isBag)
                {
                    var smartComboRoutingParams = order.SmartComboRoutingParams;
                    var smartComboRoutingParamsCount = smartComboRoutingParams?.Count ?? 0;
                    paramsList.AddParameter(smartComboRoutingParamsCount);
                    if (smartComboRoutingParamsCount > 0)
                    {
                        for (var i = 0; i < smartComboRoutingParamsCount; ++i)
                        {
                            var tagValue = smartComboRoutingParams[i];
                            paramsList.AddParameter(tagValue.Tag);
                            paramsList.AddParameter(tagValue.Value);
                        }
                    }
                }

                if (ServerVersion >= 9)
                {
                    // paramsList.AddParameter deprecated sharesAllocation field
                    paramsList.AddParameter("");
                }

                if (ServerVersion >= 10)
                    paramsList.AddParameter(order.DiscretionaryAmt);
                if (ServerVersion >= 11)
                    paramsList.AddParameter(order.GoodAfterTime);
                if (ServerVersion >= 12)
                    paramsList.AddParameter(order.GoodTillDate);
                if (ServerVersion >= 13)
                {
                    paramsList.AddParameter(order.FaGroup);
                    paramsList.AddParameter(order.FaMethod);
                    paramsList.AddParameter(order.FaPercentage);
                    if (ServerVersion < MinServerVer.MIN_SERVER_VER_FA_PROFILE_DESUPPORT)
                    {
                        paramsList.AddParameter(""); // send deprecated faProfile field
                    }
                }
                if (ServerVersion >= MinServerVer.MODELS_SUPPORT)
                    paramsList.AddParameter(order.ModelCode);
                if (ServerVersion >= 18)
                { // institutional short sale slot fields.
                    paramsList.AddParameter(order.ShortSaleSlot); // 0 only for retail, 1 or 2 only for institution.
                    paramsList.AddParameter(order.DesignatedLocation); // only populate when order.shortSaleSlot = 2.
                }
                if (ServerVersion >= MinServerVer.SSHORTX_OLD)
                    paramsList.AddParameter(order.ExemptCode);
                if (ServerVersion >= 19)
                {
                    paramsList.AddParameter(order.OcaType);
                    if (ServerVersion < 38)
                    {
                        // will never happen
                        paramsList.AddParameter( /* order.rthOnly */
                            false
                        );
                    }
                    paramsList.AddParameter(order.Rule80A);
                    paramsList.AddParameter(order.SettlingFirm);
                    paramsList.AddParameter(order.AllOrNone);
                    paramsList.AddParameterMax(order.MinQty);
                    paramsList.AddParameterMax(order.PercentOffset);
                    paramsList.AddParameter(false);
                    paramsList.AddParameter(false);
                    paramsList.AddParameterMax(double.MaxValue);
                    paramsList.AddParameterMax(order.AuctionStrategy);
                    paramsList.AddParameterMax(order.StartingPrice);
                    paramsList.AddParameterMax(order.StockRefPrice);
                    paramsList.AddParameterMax(order.Delta);
                    // Volatility orders had specific watermark price attribs in server version 26
                    var lower =
                        ServerVersion == 26 && Util.IsVolOrder(order.OrderType)
                            ? double.MaxValue
                            : order.StockRangeLower;
                    var upper =
                        ServerVersion == 26 && Util.IsVolOrder(order.OrderType)
                            ? double.MaxValue
                            : order.StockRangeUpper;
                    paramsList.AddParameterMax(lower);
                    paramsList.AddParameterMax(upper);
                }
                if (ServerVersion >= 22)
                    paramsList.AddParameter(order.OverridePercentageConstraints);
                if (ServerVersion >= 26)
                { // Volatility orders
                    paramsList.AddParameterMax(order.Volatility);
                    paramsList.AddParameterMax(order.VolatilityType);
                    if (ServerVersion < 28)
                    {
                        var isDeltaNeutralTypeMKT =
                            string.Compare("MKT", order.DeltaNeutralOrderType, true) == 0;
                        paramsList.AddParameter(isDeltaNeutralTypeMKT);
                    }
                    else
                    {
                        paramsList.AddParameter(order.DeltaNeutralOrderType);
                        paramsList.AddParameterMax(order.DeltaNeutralAuxPrice);

                        if (
                            ServerVersion >= MinServerVer.DELTA_NEUTRAL_CONID
                            && !IsEmpty(order.DeltaNeutralOrderType)
                        )
                        {
                            paramsList.AddParameter(order.DeltaNeutralConId);
                            paramsList.AddParameter(order.DeltaNeutralSettlingFirm);
                            paramsList.AddParameter(order.DeltaNeutralClearingAccount);
                            paramsList.AddParameter(order.DeltaNeutralClearingIntent);
                        }

                        if (
                            ServerVersion >= MinServerVer.DELTA_NEUTRAL_OPEN_CLOSE
                            && !IsEmpty(order.DeltaNeutralOrderType)
                        )
                        {
                            paramsList.AddParameter(order.DeltaNeutralOpenClose);
                            paramsList.AddParameter(order.DeltaNeutralShortSale);
                            paramsList.AddParameter(order.DeltaNeutralShortSaleSlot);
                            paramsList.AddParameter(order.DeltaNeutralDesignatedLocation);
                        }
                    }
                    paramsList.AddParameter(order.ContinuousUpdate);
                    if (ServerVersion == 26)
                    {
                        // Volatility orders had specific watermark price attribs in server version 26
                        var lower = Util.IsVolOrder(order.OrderType)
                            ? order.StockRangeLower
                            : double.MaxValue;
                        var upper = Util.IsVolOrder(order.OrderType)
                            ? order.StockRangeUpper
                            : double.MaxValue;
                        paramsList.AddParameterMax(lower);
                        paramsList.AddParameterMax(upper);
                    }
                    paramsList.AddParameterMax(order.ReferencePriceType);
                }

                if (ServerVersion >= 30)
                    paramsList.AddParameterMax(order.TrailStopPrice); // TRAIL_STOP_LIMIT stop price
                if (ServerVersion >= MinServerVer.TRAILING_PERCENT)
                    paramsList.AddParameterMax(order.TrailingPercent);
                if (ServerVersion >= MinServerVer.SCALE_ORDERS)
                {
                    if (ServerVersion >= MinServerVer.SCALE_ORDERS2)
                    {
                        paramsList.AddParameterMax(order.ScaleInitLevelSize);
                        paramsList.AddParameterMax(order.ScaleSubsLevelSize);
                    }
                    else
                    {
                        paramsList.AddParameter("");
                        paramsList.AddParameterMax(order.ScaleInitLevelSize);
                    }
                    paramsList.AddParameterMax(order.ScalePriceIncrement);
                }

                if (
                    ServerVersion >= MinServerVer.SCALE_ORDERS3
                    && order.ScalePriceIncrement > 0.0
                    && order.ScalePriceIncrement != double.MaxValue
                )
                {
                    paramsList.AddParameterMax(order.ScalePriceAdjustValue);
                    paramsList.AddParameterMax(order.ScalePriceAdjustInterval);
                    paramsList.AddParameterMax(order.ScaleProfitOffset);
                    paramsList.AddParameter(order.ScaleAutoReset);
                    paramsList.AddParameterMax(order.ScaleInitPosition);
                    paramsList.AddParameterMax(order.ScaleInitFillQty);
                    paramsList.AddParameter(order.ScaleRandomPercent);
                }

                if (ServerVersion >= MinServerVer.SCALE_TABLE)
                {
                    paramsList.AddParameter(order.ScaleTable);
                    paramsList.AddParameter(order.ActiveStartTime);
                    paramsList.AddParameter(order.ActiveStopTime);
                }

                if (ServerVersion >= MinServerVer.HEDGE_ORDERS)
                {
                    paramsList.AddParameter(order.HedgeType);
                    if (!IsEmpty(order.HedgeType))
                        paramsList.AddParameter(order.HedgeParam);
                }

                if (ServerVersion >= MinServerVer.OPT_OUT_SMART_ROUTING)
                    paramsList.AddParameter(order.OptOutSmartRouting);

                if (ServerVersion >= MinServerVer.PTA_ORDERS)
                {
                    paramsList.AddParameter(order.ClearingAccount);
                    paramsList.AddParameter(order.ClearingIntent);
                }

                if (ServerVersion >= MinServerVer.NOT_HELD)
                    paramsList.AddParameter(order.NotHeld);

                if (ServerVersion >= MinServerVer.DELTA_NEUTRAL)
                {
                    if (contract.DeltaNeutralContract != null)
                    {
                        var deltaNeutralContract = contract.DeltaNeutralContract;
                        paramsList.AddParameter(true);
                        paramsList.AddParameter(deltaNeutralContract.ConId);
                        paramsList.AddParameter(deltaNeutralContract.Delta);
                        paramsList.AddParameter(deltaNeutralContract.Price);
                    }
                    else
                    {
                        paramsList.AddParameter(false);
                    }
                }

                if (ServerVersion >= MinServerVer.ALGO_ORDERS)
                {
                    paramsList.AddParameter(order.AlgoStrategy);
                    if (!IsEmpty(order.AlgoStrategy))
                    {
                        var algoParams = order.AlgoParams;
                        var algoParamsCount = algoParams?.Count ?? 0;
                        paramsList.AddParameter(algoParamsCount);
                        if (algoParamsCount > 0)
                        {
                            for (var i = 0; i < algoParamsCount; ++i)
                            {
                                var tagValue = algoParams[i];
                                paramsList.AddParameter(tagValue.Tag);
                                paramsList.AddParameter(tagValue.Value);
                            }
                        }
                    }
                }

                if (ServerVersion >= MinServerVer.ALGO_ID)
                    paramsList.AddParameter(order.AlgoId);
                if (ServerVersion >= MinServerVer.WHAT_IF_ORDERS)
                    paramsList.AddParameter(order.WhatIf);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(order.OrderMiscOptions);
                if (ServerVersion >= MinServerVer.ORDER_SOLICITED)
                    paramsList.AddParameter(order.Solicited);
                if (ServerVersion >= MinServerVer.RANDOMIZE_SIZE_AND_PRICE)
                {
                    paramsList.AddParameter(order.RandomizeSize);
                    paramsList.AddParameter(order.RandomizePrice);
                }

                if (ServerVersion >= MinServerVer.PEGGED_TO_BENCHMARK)
                {
                    if (Util.IsPegBenchOrder(order.OrderType))
                    {
                        paramsList.AddParameter(order.ReferenceContractId);
                        paramsList.AddParameter(order.IsPeggedChangeAmountDecrease);
                        paramsList.AddParameter(order.PeggedChangeAmount);
                        paramsList.AddParameter(order.ReferenceChangeAmount);
                        paramsList.AddParameter(order.ReferenceExchange);
                    }

                    paramsList.AddParameter(order.Conditions.Count);

                    if (order.Conditions.Count > 0)
                    {
                        foreach (var item in order.Conditions)
                        {
                            paramsList.AddParameter((int)item.Type);
                            item.Serialize(paramsList);
                        }

                        paramsList.AddParameter(order.ConditionsIgnoreRth);
                        paramsList.AddParameter(order.ConditionsCancelOrder);
                    }

                    paramsList.AddParameter(order.AdjustedOrderType);
                    paramsList.AddParameter(order.TriggerPrice);
                    paramsList.AddParameter(order.LmtPriceOffset);
                    paramsList.AddParameter(order.AdjustedStopPrice);
                    paramsList.AddParameter(order.AdjustedStopLimitPrice);
                    paramsList.AddParameter(order.AdjustedTrailingAmount);
                    paramsList.AddParameter(order.AdjustableTrailingUnit);
                }

                if (ServerVersion >= MinServerVer.EXT_OPERATOR)
                    paramsList.AddParameter(order.ExtOperator);

                if (ServerVersion >= MinServerVer.SOFT_DOLLAR_TIER)
                {
                    paramsList.AddParameter(order.Tier.Name);
                    paramsList.AddParameter(order.Tier.Value);
                }

                if (ServerVersion >= MinServerVer.CASH_QTY)
                    paramsList.AddParameterMax(order.CashQty);

                if (ServerVersion >= MinServerVer.DECISION_MAKER)
                {
                    paramsList.AddParameter(order.Mifid2DecisionMaker);
                    paramsList.AddParameter(order.Mifid2DecisionAlgo);
                }

                if (ServerVersion >= MinServerVer.MIFID_EXECUTION)
                {
                    paramsList.AddParameter(order.Mifid2ExecutionTrader);
                    paramsList.AddParameter(order.Mifid2ExecutionAlgo);
                }

                if (ServerVersion >= MinServerVer.AUTO_PRICE_FOR_HEDGE)
                    paramsList.AddParameter(order.DontUseAutoPriceForHedge);
                if (ServerVersion >= MinServerVer.ORDER_CONTAINER)
                    paramsList.AddParameter(order.IsOmsContainer);
                if (ServerVersion >= MinServerVer.D_PEG_ORDERS)
                    paramsList.AddParameter(order.DiscretionaryUpToLimitPrice);
                if (ServerVersion >= MinServerVer.PRICE_MGMT_ALGO)
                    paramsList.AddParameter(order.UsePriceMgmtAlgo);
                if (ServerVersion >= MinServerVer.DURATION)
                    paramsList.AddParameter(order.Duration);
                if (ServerVersion >= MinServerVer.POST_TO_ATS)
                    paramsList.AddParameter(order.PostToAts);
                if (ServerVersion >= MinServerVer.AUTO_CANCEL_PARENT)
                    paramsList.AddParameter(order.AutoCancelParent);
                if (ServerVersion >= MinServerVer.ADVANCED_ORDER_REJECT)
                    paramsList.AddParameter(order.AdvancedErrorOverride);
                if (ServerVersion >= MinServerVer.MANUAL_ORDER_TIME)
                    paramsList.AddParameter(order.ManualOrderTime);
                if (ServerVersion >= MinServerVer.PEGBEST_PEGMID_OFFSETS)
                {
                    if (contract.Exchange == "IBKRATS")
                        paramsList.AddParameterMax(order.MinTradeQty);
                    var sendMidOffsets = false;
                    if (Util.IsPegBestOrder(order.OrderType))
                    {
                        paramsList.AddParameterMax(order.MinCompeteSize);
                        paramsList.AddParameterMax(order.CompeteAgainstBestOffset);
                        if (
                            order.CompeteAgainstBestOffset
                            == Order.COMPETE_AGAINST_BEST_OFFSET_UP_TO_MID
                        )
                            sendMidOffsets = true;
                    }
                    else if (Util.IsPegMidOrder(order.OrderType))
                    {
                        sendMidOffsets = true;
                    }

                    if (sendMidOffsets)
                    {
                        paramsList.AddParameterMax(order.MidOffsetAtWhole);
                        paramsList.AddParameterMax(order.MidOffsetAtHalf);
                    }
                }
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_CUSTOMER_ACCOUNT)
                    paramsList.AddParameter(order.CustomerAccount);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_PROFESSIONAL_CUSTOMER)
                    paramsList.AddParameter(order.ProfessionalCustomer);

                if (
                    ServerVersion >= MinServerVer.MIN_SERVER_VER_RFQ_FIELDS
                    && ServerVersion < MinServerVer.MIN_SERVER_VER_UNDO_RFQ_FIELDS
                )
                {
                    paramsList.AddParameter("");
                    paramsList.AddParameter(int.MaxValue);
                }

                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_INCLUDE_OVERNIGHT)
                    paramsList.AddParameter(order.IncludeOvernight);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS)
                    paramsList.AddParameter(order.ManualOrderIndicator);
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_IMBALANCE_ONLY)
                    paramsList.AddParameter(order.ImbalanceOnly);
            }
            catch (ClientException e)
            {
                Wrapper.Error(id, Util.CurrentTimeMillis(), e.Err.Code, e.Err.Message + e.Text, "");
                return;
            }

            CloseAndSend(id, paramsList, lengthPos, ClientErrors.FAIL_SEND_ORDER);
        }

        public void ReplaceFA(int reqId, int faDataType, string xml)
        {
            if (!CheckConnection())
                return;
            if (
                ServerVersion >= MinServerVer.MIN_SERVER_VER_FA_PROFILE_DESUPPORT
                && faDataType == 2
            )
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    ClientErrors.FA_PROFILE_NOT_SUPPORTED.Code,
                    ClientErrors.FA_PROFILE_NOT_SUPPORTED.Message,
                    ""
                );
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReplaceFA, ServerVersion);
                paramsList.AddParameter(1);
                paramsList.AddParameter(faDataType);
                paramsList.AddParameter(xml);
                if (ServerVersion >= MinServerVer.REPLACE_FA_END)
                {
                    paramsList.AddParameter(reqId);
                }
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_FA_REPLACE);
        }

        public void RequestFA(int faDataType)
        {
            if (!CheckConnection())
                return;
            if (
                ServerVersion >= MinServerVer.MIN_SERVER_VER_FA_PROFILE_DESUPPORT
                && faDataType == 2
            )
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    ClientErrors.FA_PROFILE_NOT_SUPPORTED.Code,
                    ClientErrors.FA_PROFILE_NOT_SUPPORTED.Message,
                    ""
                );
                return;
            }

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestFA, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(faDataType);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_FA_REQUEST);
        }

        public void ReqAccountSummary(int reqId, string group, string tags)
        {
            var VERSION = 1;
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    reqId,
                    MinServerVer.ACCT_SUMMARY,
                    " It does not support account summary requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestAccountSummary, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(group);
                paramsList.AddParameter(tags);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQACCOUNTDATA);
        }

        public void ReqAccountUpdates(bool subscribe, string acctCode)
        {
            var VERSION = 2;
            if (!CheckConnection())
                return;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestAccountData, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(subscribe);
                if (ServerVersion >= 9)
                    paramsList.AddParameter(acctCode);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQACCOUNTDATA);
        }

        public void ReqAllOpenOrders()
        {
            var VERSION = 1;
            if (!CheckConnection())
                return;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAllOpenOrders, ServerVersion);
            paramsList.AddParameter(VERSION);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_OORDER);
        }

        public void ReqAutoOpenOrders(bool autoBind)
        {
            var VERSION = 1;
            if (!CheckConnection())
                return;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAutoOpenOrders, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(autoBind);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_OORDER);
        }

        public void ReqContractDetails(int reqId, Contract contract)
        {
            if (!CheckConnection())
                return;

            if (
                (!IsEmpty(contract.SecIdType) || !IsEmpty(contract.SecId))
                && !CheckServerVersion(
                    reqId,
                    MinServerVer.SEC_ID_TYPE,
                    " It does not support secIdType not secId attributes"
                )
            )
                return;
            if (
                !IsEmpty(contract.TradingClass)
                && !CheckServerVersion(
                    reqId,
                    MinServerVer.TRADING_CLASS,
                    " It does not support the TradingClass parameter when requesting contract details."
                )
            )
                return;
            if (
                !IsEmpty(contract.PrimaryExch)
                && !CheckServerVersion(
                    reqId,
                    MinServerVer.LINKING,
                    " It does not support PrimaryExch parameter when requesting contract details."
                )
            )
                return;
            if (
                !IsEmpty(contract.IssuerId)
                && !CheckServerVersion(
                    reqId,
                    MinServerVer.MIN_SERVER_VER_BOND_ISSUERID,
                    " It does not support IssuerId parameter when requesting contract details."
                )
            )
                return;

            var VERSION = 8;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestContractData, ServerVersion);
                paramsList.AddParameter(VERSION); //version
                if (ServerVersion >= MinServerVer.CONTRACT_DATA_CHAIN)
                    paramsList.AddParameter(reqId);
                if (ServerVersion >= MinServerVer.CONTRACT_CONID)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                if (ServerVersion >= 15)
                    paramsList.AddParameter(contract.Multiplier);
                if (ServerVersion >= MinServerVer.PRIMARYEXCH)
                {
                    paramsList.AddParameter(contract.Exchange);
                    paramsList.AddParameter(contract.PrimaryExch);
                }
                else if (ServerVersion >= MinServerVer.LINKING)
                {
                    if (
                        !IsEmpty(contract.PrimaryExch)
                        && (contract.Exchange == "BEST" || contract.Exchange == "SMART")
                    )
                    {
                        paramsList.AddParameter($"{contract.Exchange}:{contract.PrimaryExch}");
                    }
                    else
                    {
                        paramsList.AddParameter(contract.Exchange);
                    }
                }

                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                if (ServerVersion >= 31)
                    paramsList.AddParameter(contract.IncludeExpired);
                if (ServerVersion >= MinServerVer.SEC_ID_TYPE)
                {
                    paramsList.AddParameter(contract.SecIdType);
                    paramsList.AddParameter(contract.SecId);
                }
                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_BOND_ISSUERID)
                    paramsList.AddParameter(contract.IssuerId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCONTRACT);
        }

        public void ReqCurrentTime()
        {
            var VERSION = 1;
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.CURRENT_TIME,
                    " It does not support current time requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestCurrentTime, ServerVersion);
            paramsList.AddParameter(VERSION); //version
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCURRTIME);
        }

        public void ReqExecutions(int reqId, ExecutionFilter filter)
        {
            if (UseProtoBuf(ServerVersion, OutgoingMessages.RequestExecutions))
            {
                ReqExecutionsProtoBuf(reqId, filter);
            }
            else
            {
                ReqExecutionsNonProtoBuf(reqId, filter);
            }
        }

        public void ReqExecutionsNonProtoBuf(int reqId, ExecutionFilter filter)
        {
            if (!CheckConnection())
                return;
            if (
                (
                    filter.SpecificDates != null && filter.SpecificDates.Any()
                    || filter.LastNDays != int.MaxValue
                )
                && !CheckServerVersion(
                    reqId,
                    MinServerVer.MIN_SERVER_VER_PARAMETRIZED_DAYS_OF_EXECUTIONS,
                    " It does not support last N days and specific dates parameters"
                )
            )
                return;

            var VERSION = 3;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestExecutions, ServerVersion);
                paramsList.AddParameter(VERSION); //version
                if (ServerVersion >= MinServerVer.EXECUTION_DATA_CHAIN)
                    paramsList.AddParameter(reqId);

                //Send the execution rpt filter data
                if (ServerVersion >= 9)
                {
                    paramsList.AddParameter(filter.ClientId);
                    paramsList.AddParameter(filter.AcctCode);

                    // Note that the valid format for time is "yyyyMMdd-HH:mm:ss" (UTC) or "yyyyMMdd HH:mm:ss timezone"
                    paramsList.AddParameter(filter.Time);
                    paramsList.AddParameter(filter.Symbol);
                    paramsList.AddParameter(filter.SecType);
                    paramsList.AddParameter(filter.Exchange);
                    paramsList.AddParameter(filter.Side);

                    if (
                        ServerVersion >= MinServerVer.MIN_SERVER_VER_PARAMETRIZED_DAYS_OF_EXECUTIONS
                    )
                    {
                        paramsList.AddParameter(filter.LastNDays);
                        paramsList.AddParameter(filter.SpecificDates.Count);
                        foreach (int specificDate in filter.SpecificDates)
                        {
                            paramsList.AddParameter(specificDate);
                        }
                    }
                }
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_EXEC);
        }

        public void ReqExecutionsProtoBuf(int reqId, ExecutionFilter filter)
        {
            if (!CheckConnection())
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                // execution filter
                IBApi.protobuf.ExecutionFilter executionFilterProto =
                    new IBApi.protobuf.ExecutionFilter();
                if (filter.ClientId != int.MaxValue)
                {
                    executionFilterProto.ClientId = filter.ClientId;
                }
                if (!Util.StringIsEmpty(filter.AcctCode))
                {
                    executionFilterProto.AcctCode = filter.AcctCode;
                }
                if (!Util.StringIsEmpty(filter.Time))
                {
                    executionFilterProto.Time = filter.Time;
                }
                if (!Util.StringIsEmpty(filter.Symbol))
                {
                    executionFilterProto.Symbol = filter.Symbol;
                }
                if (!Util.StringIsEmpty(filter.SecType))
                {
                    executionFilterProto.SecType = filter.SecType;
                }
                if (!Util.StringIsEmpty(filter.Exchange))
                {
                    executionFilterProto.Exchange = filter.Exchange;
                }
                if (!Util.StringIsEmpty(filter.Side))
                {
                    executionFilterProto.Side = filter.Side;
                }
                if (filter.LastNDays != int.MaxValue)
                {
                    executionFilterProto.LastNDays = filter.LastNDays;
                }
                if (filter.SpecificDates != null && filter.SpecificDates.Any())
                {
                    executionFilterProto.SpecificDates.AddRange(filter.SpecificDates);
                }

                // execution request
                IBApi.protobuf.ExecutionRequest executionRequestProto =
                    new IBApi.protobuf.ExecutionRequest();
                if (reqId != int.MaxValue)
                {
                    executionRequestProto.ReqId = reqId;
                }
                executionRequestProto.ExecutionFilter = executionFilterProto;

                paramsList.AddParameter(OutgoingMessages.RequestExecutions, ServerVersion);
                paramsList.AddParameter(executionRequestProto.ToByteArray());
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_EXEC);
        }

        public void ReqFundamentalData(
            int reqId,
            Contract contract,
            string reportType,
            //reserved for future use, must be blank
            List<TagValue> fundamentalDataOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    reqId,
                    MinServerVer.FUNDAMENTAL_DATA,
                    " It does not support Fundamental Data requests."
                )
            )
                return;
            if (
                (
                    !IsEmpty(contract.TradingClass)
                    || contract.ConId > 0
                    || !IsEmpty(contract.Multiplier)
                ) && !CheckServerVersion(reqId, MinServerVer.TRADING_CLASS, "")
            )
                return;

            const int VERSION = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestFundamentalData, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(reqId);

                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                {
                    //WARN: why are we checking the trading class and multiplier above never send them?
                    paramsList.AddParameter(contract.ConId);
                }

                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                paramsList.AddParameter(reportType);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(fundamentalDataOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQFUNDDATA);
        }

        public void ReqGlobalCancel(OrderCancel orderCancel)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_GLOBAL_CANCEL,
                    "It does not support global cancel requests."
                )
            )
                return;

            if (
                (
                    !IsEmpty(orderCancel.ExtOperator)
                    || orderCancel.ManualOrderIndicator != int.MaxValue
                )
                && !CheckServerVersion(
                    MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS,
                    " It does not support ext operator and manual order indicator parameters"
                )
            )
                return;

            const int VERSION = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestGlobalCancel, ServerVersion);
            if (ServerVersion < MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS)
            {
                paramsList.AddParameter(VERSION);
            }
            if (ServerVersion >= MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS)
            {
                paramsList.AddParameter(orderCancel.ExtOperator);
                paramsList.AddParameter(orderCancel.ManualOrderIndicator);
            }
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQGLOBALCANCEL);
        }

        public void ReqHistoricalData(
            int tickerId,
            Contract contract,
            string endDateTime,
            string durationStr,
            string barSizeSetting,
            string whatToShow,
            int useRTH,
            int formatDate,
            bool keepUpToDate,
            List<TagValue> chartOptions
        )
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(tickerId, 16))
                return;
            if (
                (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.TRADING_CLASS,
                    " It does not support conId nor trading class parameters when requesting historical data."
                )
            )
                return;
            if (
                !IsEmpty(whatToShow)
                && whatToShow.Equals("SCHEDULE")
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.HISTORICAL_SCHEDULE,
                    " It does not support requesting of historical schedule."
                )
            )
                return;

            const int VERSION = 6;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestHistoricalData, ServerVersion);
                if (ServerVersion < MinServerVer.SYNT_REALTIME_BARS)
                    paramsList.AddParameter(VERSION);
                paramsList.AddParameter(tickerId);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(contract.IncludeExpired ? 1 : 0);
                paramsList.AddParameter(endDateTime);
                paramsList.AddParameter(barSizeSetting);
                paramsList.AddParameter(durationStr);
                paramsList.AddParameter(useRTH);
                paramsList.AddParameter(whatToShow);
                paramsList.AddParameter(formatDate);

                if (StringsAreEqual(Constants.BagSecType, contract.SecType))
                {
                    if (contract.ComboLegs != null)
                    {
                        paramsList.AddParameter(contract.ComboLegs.Count);

                        ComboLeg comboLeg;
                        for (var i = 0; i < contract.ComboLegs.Count; i++)
                        {
                            comboLeg = contract.ComboLegs[i];
                            paramsList.AddParameter(comboLeg.ConId);
                            paramsList.AddParameter(comboLeg.Ratio);
                            paramsList.AddParameter(comboLeg.Action);
                            paramsList.AddParameter(comboLeg.Exchange);
                        }
                    }
                    else
                    {
                        paramsList.AddParameter(0);
                    }
                }
                if (ServerVersion >= MinServerVer.SYNT_REALTIME_BARS)
                    paramsList.AddParameter(keepUpToDate);

                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(chartOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQHISTDATA);
        }

        public void ReqIds(int numIds)
        {
            if (!CheckConnection())
                return;
            const int VERSION = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestIds, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(numIds);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_GENERIC);
        }

        public void ReqManagedAccts()
        {
            if (!CheckConnection())
                return;
            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestManagedAccounts, ServerVersion);
            paramsList.AddParameter(VERSION);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_GENERIC);
        }

        public void ReqMktData(
            int tickerId,
            Contract contract,
            string genericTickList,
            bool snapshot,
            bool regulatorySnaphsot,
            List<TagValue> mktDataOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                snapshot
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.SNAPSHOT_MKT_DATA,
                    "It does not support snapshot market data requests."
                )
            )
                return;
            if (
                contract.DeltaNeutralContract != null
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.DELTA_NEUTRAL,
                    " It does not support delta-neutral orders"
                )
            )
                return;
            if (
                contract.ConId > 0
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.CONTRACT_CONID,
                    " It does not support ConId parameter"
                )
            )
                return;
            if (
                !Util.StringIsEmpty(contract.TradingClass)
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.TRADING_CLASS,
                    " It does not support trading class parameter in reqMktData."
                )
            )
                return;

            var version = 11;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestMarketData, ServerVersion);
                paramsList.AddParameter(version);
                paramsList.AddParameter(tickerId);
                if (ServerVersion >= MinServerVer.CONTRACT_CONID)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                if (ServerVersion >= 15)
                    paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                if (ServerVersion >= 14)
                    paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                if (ServerVersion >= 2)
                    paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                if (ServerVersion >= 8 && Constants.BagSecType.Equals(contract.SecType))
                {
                    if (contract.ComboLegs != null)
                    {
                        paramsList.AddParameter(contract.ComboLegs.Count);
                        for (var i = 0; i < contract.ComboLegs.Count; i++)
                        {
                            var leg = contract.ComboLegs[i];
                            paramsList.AddParameter(leg.ConId);
                            paramsList.AddParameter(leg.Ratio);
                            paramsList.AddParameter(leg.Action);
                            paramsList.AddParameter(leg.Exchange);
                        }
                    }
                    else
                    {
                        paramsList.AddParameter(0);
                    }
                }

                if (ServerVersion >= MinServerVer.DELTA_NEUTRAL)
                {
                    if (contract.DeltaNeutralContract != null)
                    {
                        paramsList.AddParameter(true);
                        paramsList.AddParameter(contract.DeltaNeutralContract.ConId);
                        paramsList.AddParameter(contract.DeltaNeutralContract.Delta);
                        paramsList.AddParameter(contract.DeltaNeutralContract.Price);
                    }
                    else
                    {
                        paramsList.AddParameter(false);
                    }
                }
                if (ServerVersion >= 31)
                    paramsList.AddParameter(genericTickList);
                if (ServerVersion >= MinServerVer.SNAPSHOT_MKT_DATA)
                    paramsList.AddParameter(snapshot);
                if (ServerVersion >= MinServerVer.SMART_COMPONENTS)
                    paramsList.AddParameter(regulatorySnaphsot);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(mktDataOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMKT);
        }

        public void ReqMarketDataType(int marketDataType)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_MARKET_DATA_TYPE,
                    " It does not support market data type requests."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketDataType, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(marketDataType);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMARKETDATATYPE);
        }

        public void ReqMarketDepth(
            int tickerId,
            Contract contract,
            int numRows,
            bool isSmartDepth,
            List<TagValue> mktDepthOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.TRADING_CLASS,
                    " It does not support ConId nor TradingClass parameters in reqMktDepth."
                )
            )
                return;
            if (
                isSmartDepth
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.SMART_DEPTH,
                    " It does not support SMART depth request."
                )
            )
                return;
            if (
                !IsEmpty(contract.PrimaryExch)
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.MKT_DEPTH_PRIM_EXCHANGE,
                    " It does not support PrimaryExch parameter in reqMktDepth."
                )
            )
                return;

            const int VERSION = 5;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestMarketDepth, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(tickerId);
                // paramsList.AddParameter contract fields
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                if (ServerVersion >= 15)
                    paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                if (ServerVersion >= MinServerVer.MKT_DEPTH_PRIM_EXCHANGE)
                    paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                if (ServerVersion >= 19)
                    paramsList.AddParameter(numRows);
                if (ServerVersion >= MinServerVer.SMART_DEPTH)
                    paramsList.AddParameter(isSmartDepth);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(mktDepthOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMKTDEPTH);
        }

        public void ReqNewsBulletins(bool allMessages)
        {
            if (!CheckConnection())
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestNewsBulletins, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(allMessages);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_GENERIC);
        }

        public void ReqOpenOrders()
        {
            var VERSION = 1;
            if (!CheckConnection())
                return;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestOpenOrders, ServerVersion);
            paramsList.AddParameter(VERSION);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_OORDER);
        }

        public void ReqPositions()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.ACCT_SUMMARY,
                    " It does not support position requests."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestPositions, ServerVersion);
            paramsList.AddParameter(VERSION);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQPOSITIONS);
        }

        public void ReqRealTimeBars(
            int tickerId,
            Contract contract,
            int barSize,
            string whatToShow,
            bool useRTH,
            List<TagValue> realTimeBarsOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    tickerId,
                    MinServerVer.REAL_TIME_BARS,
                    " It does not support real time bars."
                )
            )
                return;
            if (
                (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
                && !CheckServerVersion(
                    tickerId,
                    MinServerVer.TRADING_CLASS,
                    " It does not support ConId nor TradingClass parameters in reqRealTimeBars."
                )
            )
                return;

            const int VERSION = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestRealTimeBars, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(tickerId);
                // paramsList.AddParameter contract fields
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.ConId);
                paramsList.AddParameter(contract.Symbol);
                paramsList.AddParameter(contract.SecType);
                paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
                paramsList.AddParameter(contract.Strike);
                paramsList.AddParameter(contract.Right);
                paramsList.AddParameter(contract.Multiplier);
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
                paramsList.AddParameter(contract.Currency);
                paramsList.AddParameter(contract.LocalSymbol);
                if (ServerVersion >= MinServerVer.TRADING_CLASS)
                    paramsList.AddParameter(contract.TradingClass);
                paramsList.AddParameter(barSize); // this parameter is not currently used
                paramsList.AddParameter(whatToShow);
                paramsList.AddParameter(useRTH);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(realTimeBarsOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQRTBARS);
        }

        public void ReqScannerParameters()
        {
            if (!CheckConnection())
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestScannerParameters, ServerVersion);
            paramsList.AddParameter(VERSION);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQSCANNERPARAMETERS);
        }

        public void ReqScannerSubscription(
            int reqId,
            ScannerSubscription subscription,
            List<TagValue> scannerSubscriptionOptions,
            List<TagValue> scannerSubscriptionFilterOptions
        ) =>
            ReqScannerSubscription(
                reqId,
                subscription,
                Util.TagValueListToString(scannerSubscriptionOptions),
                Util.TagValueListToString(scannerSubscriptionFilterOptions)
            );

        public void ReqScannerSubscription(
            int reqId,
            ScannerSubscription subscription,
            string scannerSubscriptionOptions,
            string scannerSubscriptionFilterOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                scannerSubscriptionFilterOptions != null
                && !CheckServerVersion(
                    MinServerVer.SCANNER_GENERIC_OPTS,
                    " It does not support API scanner subscription generic filter options"
                )
            )
                return;

            const int VERSION = 4;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestScannerSubscription, ServerVersion);
                if (ServerVersion < MinServerVer.SCANNER_GENERIC_OPTS)
                    paramsList.AddParameter(VERSION);
                paramsList.AddParameter(reqId);
                paramsList.AddParameterMax(subscription.NumberOfRows);
                paramsList.AddParameter(subscription.Instrument);
                paramsList.AddParameter(subscription.LocationCode);
                paramsList.AddParameter(subscription.ScanCode);

                paramsList.AddParameterMax(subscription.AbovePrice);
                paramsList.AddParameterMax(subscription.BelowPrice);
                paramsList.AddParameterMax(subscription.AboveVolume);
                paramsList.AddParameterMax(subscription.MarketCapAbove);
                paramsList.AddParameterMax(subscription.MarketCapBelow);
                paramsList.AddParameter(subscription.MoodyRatingAbove);
                paramsList.AddParameter(subscription.MoodyRatingBelow);
                paramsList.AddParameter(subscription.SpRatingAbove);
                paramsList.AddParameter(subscription.SpRatingBelow);
                paramsList.AddParameter(subscription.MaturityDateAbove);
                paramsList.AddParameter(subscription.MaturityDateBelow);
                paramsList.AddParameterMax(subscription.CouponRateAbove);
                paramsList.AddParameterMax(subscription.CouponRateBelow);
                paramsList.AddParameter(subscription.ExcludeConvertible);

                if (ServerVersion >= 25)
                {
                    paramsList.AddParameterMax(subscription.AverageOptionVolumeAbove);
                    paramsList.AddParameter(subscription.ScannerSettingPairs);
                }

                if (ServerVersion >= 27)
                    paramsList.AddParameter(subscription.StockTypeFilter);
                if (ServerVersion >= MinServerVer.SCANNER_GENERIC_OPTS)
                    paramsList.AddParameter(scannerSubscriptionFilterOptions);
                if (ServerVersion >= MinServerVer.LINKING)
                    paramsList.AddParameter(scannerSubscriptionOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQSCANNER);
        }

        public void SetServerLogLevel(int logLevel)
        {
            if (!CheckConnection())
                return;

            const int VERSION = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ChangeServerLog, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(logLevel);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_SERVER_LOG_LEVEL);
        }

        public void VerifyRequest(string apiName, string apiVersion)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support verification request."
                )
            )
                return;
            if (!ExtraAuth)
            {
                ReportError(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    ClientErrors.FAIL_SEND_VERIFYMESSAGE,
                    " Intent to authenticate needs to be expressed during initial connect request."
                );
                return;
            }

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.VerifyRequest, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(apiName);
                paramsList.AddParameter(apiVersion);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_VERIFYREQUEST);
        }

        public void VerifyMessage(string apiData)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support verification message sending."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.VerifyMessage, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(apiData);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_VERIFYMESSAGE);
        }

        public void VerifyAndAuthRequest(string apiName, string apiVersion, string opaqueIsvKey)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING_AUTH,
                    " It does not support verification request."
                )
            )
                return;
            if (!ExtraAuth)
            {
                ReportError(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    ClientErrors.FAIL_SEND_VERIFYANDAUTHMESSAGE,
                    " Intent to authenticate needs to be expressed during initial connect request."
                );
                return;
            }

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.VerifyAndAuthRequest, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(apiName);
                paramsList.AddParameter(apiVersion);
                paramsList.AddParameter(opaqueIsvKey);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_VERIFYANDAUTHREQUEST);
        }

        public void VerifyAndAuthMessage(string apiData, string xyzResponse)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING_AUTH,
                    " It does not support verification message sending."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.VerifyAndAuthMessage, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(apiData);
                paramsList.AddParameter(xyzResponse);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_VERIFYANDAUTHMESSAGE);
        }

        public void QueryDisplayGroups(int requestId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support queryDisplayGroups request."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.QueryDisplayGroups, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_QUERYDISPLAYGROUPS);
        }

        public void SubscribeToGroupEvents(int requestId, int groupId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support subscribeToGroupEvents request."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.SubscribeToGroupEvents, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(groupId);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_SUBSCRIBETOGROUPEVENTS);
        }

        public void UpdateDisplayGroup(int requestId, string contractInfo)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support updateDisplayGroup request."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.UpdateDisplayGroup, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(contractInfo);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_UPDATEDISPLAYGROUP
            );
        }

        public void UnsubscribeFromGroupEvents(int requestId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.LINKING,
                    " It does not support unsubscribeFromGroupEvents request."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.UnsubscribeFromGroupEvents, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_UNSUBSCRIBEFROMGROUPEVENTS);
        }

        public void ReqPositionsMulti(int requestId, string account, string modelCode)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MODELS_SUPPORT,
                    " It does not support positions multi requests."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestPositionsMulti, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(account);
                paramsList.AddParameter(modelCode);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_REQPOSITIONSMULTI
            );
        }

        public void CancelPositionsMulti(int requestId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MODELS_SUPPORT,
                    " It does not support positions multi cancellation."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPositionsMulti, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(requestId);
            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_CANPOSITIONSMULTI
            );
        }

        public void ReqAccountUpdatesMulti(
            int requestId,
            string account,
            string modelCode,
            bool ledgerAndNLV
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MODELS_SUPPORT,
                    " It does not support account updates multi requests."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestAccountUpdatesMulti, ServerVersion);
                paramsList.AddParameter(VERSION);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(account);
                paramsList.AddParameter(modelCode);
                paramsList.AddParameter(ledgerAndNLV);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_REQACCOUNTUPDATESMULTI
            );
        }

        public void CancelAccountUpdatesMulti(int requestId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MODELS_SUPPORT,
                    " It does not support account updates multi cancellation."
                )
            )
                return;

            const int VERSION = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelAccountUpdatesMulti, ServerVersion);
            paramsList.AddParameter(VERSION);
            paramsList.AddParameter(requestId);
            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_CANACCOUNTUPDATESMULTI
            );
        }

        public void ReqSecDefOptParams(
            int reqId,
            string underlyingSymbol,
            string futFopExchange,
            string underlyingSecType,
            int underlyingConId
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.SEC_DEF_OPT_PARAMS_REQ,
                    " It does not support security definition option parameters."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(
                    OutgoingMessages.RequestSecurityDefinitionOptionalParameters,
                    ServerVersion
                );
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(underlyingSymbol);
                paramsList.AddParameter(futFopExchange);
                paramsList.AddParameter(underlyingSecType);
                paramsList.AddParameter(underlyingConId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQSECDEFOPTPARAMS);
        }

        public void ReqSoftDollarTiers(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.SOFT_DOLLAR_TIER,
                    " It does not support soft dollar tier."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestSoftDollarTiers, ServerVersion);
            paramsList.AddParameter(reqId);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQSOFTDOLLARTIERS);
        }

        public void ReqFamilyCodes()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_FAMILY_CODES,
                    " It does not support family codes requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestFamilyCodes, ServerVersion);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQFAMILYCODES);
        }

        public void ReqMatchingSymbols(int reqId, string pattern)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_MATCHING_SYMBOLS,
                    " It does not support matching symbols requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestMatchingSymbols, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(pattern);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMATCHINGSYMBOLS);
        }

        public void ReqMktDepthExchanges()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_MKT_DEPTH_EXCHANGES,
                    " It does not support market depth exchanges requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMktDepthExchanges, ServerVersion);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMKTDEPTHEXCHANGES);
        }

        public void ReqSmartComponents(int reqId, string bboExchange)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_MKT_DEPTH_EXCHANGES,
                    " It does not support smart components request."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestSmartComponents, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(bboExchange);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQSMARTCOMPONENTS);
        }

        public void ReqNewsProviders()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_NEWS_PROVIDERS,
                    " It does not support news providers requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestNewsProviders, ServerVersion);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQNEWSPROVIDERS);
        }

        public void ReqNewsArticle(
            int requestId,
            string providerCode,
            string articleId,
            List<TagValue> newsArticleOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_NEWS_ARTICLE,
                    " It does not support news article requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestNewsArticle, ServerVersion);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(providerCode);
                paramsList.AddParameter(articleId);
                if (ServerVersion >= MinServerVer.NEWS_QUERY_ORIGINS)
                    paramsList.AddParameter(newsArticleOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(requestId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQNEWSARTICLE);
        }

        public void ReqHistoricalNews(
            int requestId,
            int conId,
            string providerCodes,
            string startDateTime,
            string endDateTime,
            int totalResults,
            List<TagValue> historicalNewsOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_HISTORICAL_NEWS,
                    " It does not support historical news requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestHistoricalNews, ServerVersion);
                paramsList.AddParameter(requestId);
                paramsList.AddParameter(conId);
                paramsList.AddParameter(providerCodes);
                paramsList.AddParameter(startDateTime);
                paramsList.AddParameter(endDateTime);
                paramsList.AddParameter(totalResults);
                if (ServerVersion >= MinServerVer.NEWS_QUERY_ORIGINS)
                    paramsList.AddParameter(historicalNewsOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    requestId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(
                requestId,
                paramsList,
                lengthPos,
                ClientErrors.FAIL_SEND_REQHISTORICALNEWS
            );
        }

        public void ReqHeadTimestamp(
            int tickerId,
            Contract contract,
            string whatToShow,
            int useRTH,
            int formatDate
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_HEAD_TIMESTAMP,
                    " It does not support head time stamp requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestHeadTimestamp, ServerVersion);
                paramsList.AddParameter(tickerId);
                paramsList.AddParameter(contract);
                paramsList.AddParameter(useRTH);
                paramsList.AddParameter(whatToShow);
                paramsList.AddParameter(formatDate);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQHEADTIMESTAMP);
        }

        public void CancelHeadTimestamp(int tickerId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.CANCEL_HEADTIMESTAMP,
                    " It does not support head time stamp requests canceling."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelHeadTimestamp, ServerVersion);
            paramsList.AddParameter(tickerId);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CANCELHEADTIMESTAMP);
        }

        public void ReqHistogramData(int tickerId, Contract contract, bool useRTH, string period)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_HISTOGRAM_DATA,
                    " It does not support histogram data requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.RequestHistogramData, ServerVersion);
                paramsList.AddParameter(tickerId);
                paramsList.AddParameter(contract);
                paramsList.AddParameter(useRTH);
                paramsList.AddParameter(period);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    tickerId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(tickerId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQHISTOGRAMDATA);
        }

        public void CancelHistogramData(int tickerId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.REQ_HISTOGRAM_DATA,
                    " It does not support histogram data requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelHistogramData, ServerVersion);
            paramsList.AddParameter(tickerId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CANCELHISTOGRAMDATA);
        }

        public void ReqMarketRule(int marketRuleId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MARKET_RULES,
                    " It does not support market rule requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketRule, ServerVersion);
            paramsList.AddParameter(marketRuleId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQMARKETRULE);
        }

        public void ReqPnL(int reqId, string account, string modelCode)
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(MinServerVer.PNL, "  It does not support PnL requests."))
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqPnL, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(account);
                paramsList.AddParameter(modelCode);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQPNL);
        }

        public void CancelPnL(int reqId)
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(MinServerVer.PNL, "  It does not support PnL requests."))
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPnL, ServerVersion);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CANCELPNL);
        }

        public void ReqPnLSingle(int reqId, string account, string modelCode, int conId)
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(MinServerVer.PNL, "  It does not support PnL requests."))
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqPnLSingle, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(account);
                paramsList.AddParameter(modelCode);
                paramsList.AddParameter(conId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQPNLSINGLE);
        }

        public void CancelPnLSingle(int reqId)
        {
            if (!CheckConnection())
                return;
            if (!CheckServerVersion(MinServerVer.PNL, "  It does not support PnL requests."))
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPnLSingle, ServerVersion);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQPNLSINGLE);
        }

        public void ReqHistoricalTicks(
            int reqId,
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            string whatToShow,
            int useRth,
            bool ignoreSize,
            List<TagValue> miscOptions
        )
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.HISTORICAL_TICKS,
                    "  It does not support historical ticks request."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqHistoricalTicks, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(contract);
                paramsList.AddParameter(startDateTime);
                paramsList.AddParameter(endDateTime);
                paramsList.AddParameter(numberOfTicks);
                paramsList.AddParameter(whatToShow);
                paramsList.AddParameter(useRth);
                paramsList.AddParameter(ignoreSize);
                paramsList.AddParameter(miscOptions);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQHISTORICALTICKS);
        }

        public void ReqWshMetaData(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.WSHE_CALENDAR,
                    "  It does not support WSHE Calendar API."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqWshMetaData, ServerVersion);
                paramsList.AddParameter(reqId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQ_WSH_META_DATA);
        }

        public void CancelWshMetaData(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.WSHE_CALENDAR,
                    "  It does not support WSHE Calendar API."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelWshMetaData, ServerVersion);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CAN_WSH_META_DATA);
        }

        public void ReqWshEventData(int reqId, WshEventData wshEventData)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.WSHE_CALENDAR,
                    "  It does not support WSHE Calendar API."
                )
            )
                return;

            if (ServerVersion < MinServerVer.MIN_SERVER_VER_WSH_EVENT_DATA_FILTERS)
            {
                if (
                    !IsEmpty(wshEventData.Filter)
                    || wshEventData.FillWatchlist
                    || wshEventData.FillPortfolio
                    || wshEventData.FillCompetitors
                )
                {
                    ReportError(
                        reqId,
                        Util.CurrentTimeMillis(),
                        ClientErrors.UPDATE_TWS,
                        "  It does not support WSH event data filters."
                    );
                    return;
                }
            }

            if (ServerVersion < MinServerVer.MIN_SERVER_VER_WSH_EVENT_DATA_FILTERS_DATE)
            {
                if (
                    !IsEmpty(wshEventData.StartDate)
                    || !IsEmpty(wshEventData.EndDate)
                    || wshEventData.TotalLimit != int.MaxValue
                )
                {
                    ReportError(
                        reqId,
                        Util.CurrentTimeMillis(),
                        ClientErrors.UPDATE_TWS,
                        "  It does not support WSH event data date filters."
                    );
                    return;
                }
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqWshEventData, ServerVersion);
                paramsList.AddParameter(reqId);
                paramsList.AddParameter(wshEventData.ConId);

                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_WSH_EVENT_DATA_FILTERS)
                {
                    paramsList.AddParameter(wshEventData.Filter);
                    paramsList.AddParameter(wshEventData.FillWatchlist);
                    paramsList.AddParameter(wshEventData.FillPortfolio);
                    paramsList.AddParameter(wshEventData.FillCompetitors);
                }

                if (ServerVersion >= MinServerVer.MIN_SERVER_VER_WSH_EVENT_DATA_FILTERS_DATE)
                {
                    paramsList.AddParameter(wshEventData.StartDate);
                    paramsList.AddParameter(wshEventData.EndDate);
                    paramsList.AddParameter(wshEventData.TotalLimit);
                }
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQ_WSH_EVENT_DATA);
        }

        public void CancelWshEventData(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.WSHE_CALENDAR,
                    "  It does not support WSHE Calendar API."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelWshEventData, ServerVersion);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_CAN_WSH_EVENT_DATA);
        }

        public void ReqUserInfo(int reqId)
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.USER_INFO,
                    " It does not support user info requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(OutgoingMessages.ReqUserInfo, ServerVersion);
                paramsList.AddParameter(reqId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            CloseAndSend(reqId, paramsList, lengthPos, ClientErrors.FAIL_SEND_REQ_USER_INFO);
        }

        public void ReqCurrentTimeInMillis()
        {
            if (!CheckConnection())
                return;
            if (
                !CheckServerVersion(
                    MinServerVer.MIN_SERVER_VER_CURRENT_TIME_IN_MILLIS,
                    " It does not support current time in millis requests."
                )
            )
                return;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestCurrentTimeInMillis, ServerVersion);
            CloseAndSend(paramsList, lengthPos, ClientErrors.FAIL_SEND_REQCURRTIMEINMILLIS);
        }

        protected bool CheckServerVersion(int requiredVersion) =>
            CheckServerVersion(requiredVersion, "");

        protected bool CheckServerVersion(int requestId, int requiredVersion) =>
            CheckServerVersion(requestId, requiredVersion, "");

        protected bool CheckServerVersion(int requiredVersion, string updatetail) =>
            CheckServerVersion(IncomingMessage.NotValid, requiredVersion, updatetail);

        protected bool CheckServerVersion(int tickerId, int requiredVersion, string updatetail)
        {
            if (ServerVersion >= requiredVersion)
                return true;
            ReportUpdateTWS(tickerId, updatetail);
            return false;
        }

        protected void CloseAndSend(BinaryWriter paramsList, uint lengthPos, CodeMsgPair error) =>
            CloseAndSend(IncomingMessage.NotValid, paramsList, lengthPos, error);

        protected void CloseAndSend(
            int reqId,
            BinaryWriter paramsList,
            uint lengthPos,
            CodeMsgPair error
        )
        {
            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(reqId, Util.CurrentTimeMillis(), error.Code, error.Message, "");
                Close();
            }
        }

        protected bool CheckConnection()
        {
            if (IsConnected)
                return true;
            Wrapper.Error(
                IncomingMessage.NotValid,
                Util.CurrentTimeMillis(),
                ClientErrors.NOT_CONNECTED.Code,
                ClientErrors.NOT_CONNECTED.Message,
                ""
            );
            return false;
        }

        protected void ReportError(int reqId, long errorTime, CodeMsgPair error, string tail) =>
            ReportError(reqId, errorTime, error.Code, error.Message + tail);

        protected void ReportUpdateTWS(int reqId, long errorTime, string tail) =>
            ReportError(
                reqId,
                errorTime,
                ClientErrors.UPDATE_TWS.Code,
                ClientErrors.UPDATE_TWS.Message + tail
            );

        protected void ReportUpdateTWS(long errorTime, string tail) =>
            ReportError(
                IncomingMessage.NotValid,
                errorTime,
                ClientErrors.UPDATE_TWS.Code,
                ClientErrors.UPDATE_TWS.Message + tail
            );

        protected void ReportError(int reqId, long errorTime, int code, string message) =>
            Wrapper.Error(reqId, errorTime, code, message, "");

        protected void SendCancelRequest(
            OutgoingMessages msgType,
            int version,
            int reqId,
            CodeMsgPair errorMessage,
            int serverVersion
        )
        {
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(msgType, serverVersion);
                paramsList.AddParameter(version);
                paramsList.AddParameter(reqId);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(
                    reqId,
                    Util.CurrentTimeMillis(),
                    errorMessage.Code,
                    errorMessage.Message,
                    ""
                );
                Close();
            }
        }

        protected void SendCancelRequest(
            OutgoingMessages msgType,
            int version,
            CodeMsgPair errorMessage,
            int serverVersion
        )
        {
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            try
            {
                paramsList.AddParameter(msgType, serverVersion);
                paramsList.AddParameter(version);
            }
            catch (ClientException e)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    e.Err.Code,
                    e.Err.Message + e.Text,
                    ""
                );
                return;
            }

            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(
                    IncomingMessage.NotValid,
                    Util.CurrentTimeMillis(),
                    errorMessage.Code,
                    errorMessage.Message,
                    ""
                );
                Close();
            }
        }

        protected bool VerifyOrderContract(Contract contract, int id)
        {
            if (ServerVersion < MinServerVer.SSHORT_COMBO_LEGS)
            {
                if (contract.ComboLegs.Count > 0)
                {
                    ComboLeg comboLeg;
                    for (var i = 0; i < contract.ComboLegs.Count; ++i)
                    {
                        comboLeg = contract.ComboLegs[i];
                        if (comboLeg.ShortSaleSlot == 0 && IsEmpty(comboLeg.DesignatedLocation))
                            continue;
                        ReportError(
                            id,
                            Util.CurrentTimeMillis(),
                            ClientErrors.UPDATE_TWS,
                            "  It does not support SSHORT flag for combo legs."
                        );
                        return false;
                    }
                }
            }

            if (ServerVersion < MinServerVer.DELTA_NEUTRAL && contract.DeltaNeutralContract != null)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support delta-neutral orders."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.PLACE_ORDER_CONID && contract.ConId > 0)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support conId parameter."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.SEC_ID_TYPE
                && (!IsEmpty(contract.SecIdType) || !IsEmpty(contract.SecId))
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support secIdType and secId parameters."
                );
                return false;
            }
            if (ServerVersion < MinServerVer.SSHORTX && contract.ComboLegs.Count > 0)
            {
                ComboLeg comboLeg;
                for (var i = 0; i < contract.ComboLegs.Count; ++i)
                {
                    comboLeg = contract.ComboLegs[i];
                    if (comboLeg.ExemptCode == -1)
                        continue;
                    ReportError(
                        id,
                        Util.CurrentTimeMillis(),
                        ClientErrors.UPDATE_TWS,
                        "  It does not support exemptCode parameter."
                    );
                    return false;
                }
            }
            if (ServerVersion < MinServerVer.TRADING_CLASS && !IsEmpty(contract.TradingClass))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support tradingClass parameters in placeOrder."
                );
                return false;
            }
            return true;
        }

        protected bool VerifyOrder(Order order, int id, bool isBagOrder)
        {
            if (
                ServerVersion < MinServerVer.SCALE_ORDERS
                && (
                    order.ScaleInitLevelSize != int.MaxValue
                    || order.ScalePriceIncrement != double.MaxValue
                )
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support Scale orders."
                );
                return false;
            }
            if (ServerVersion < MinServerVer.WHAT_IF_ORDERS && order.WhatIf)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support what-if orders."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.SCALE_ORDERS2
                && order.ScaleSubsLevelSize != int.MaxValue
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support Subsequent Level Size for Scale orders."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.ALGO_ORDERS && !IsEmpty(order.AlgoStrategy))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support algo orders."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.NOT_HELD && order.NotHeld)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support notHeld parameter."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.SSHORTX && order.ExemptCode != -1)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support exemptCode parameter."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.HEDGE_ORDERS && !IsEmpty(order.HedgeType))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support hedge orders."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.OPT_OUT_SMART_ROUTING && order.OptOutSmartRouting)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support optOutSmartRouting parameter."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.DELTA_NEUTRAL_CONID
                && (
                    order.DeltaNeutralConId > 0
                    || !IsEmpty(order.DeltaNeutralSettlingFirm)
                    || !IsEmpty(order.DeltaNeutralClearingAccount)
                    || !IsEmpty(order.DeltaNeutralClearingIntent)
                )
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support deltaNeutral parameters: ConId, SettlingFirm, ClearingAccount, ClearingIntent"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.DELTA_NEUTRAL_OPEN_CLOSE
                && (
                    !IsEmpty(order.DeltaNeutralOpenClose)
                    || order.DeltaNeutralShortSale
                    || order.DeltaNeutralShortSaleSlot > 0
                    || !IsEmpty(order.DeltaNeutralDesignatedLocation)
                )
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support deltaNeutral parameters: OpenClose, ShortSale, ShortSaleSlot, DesignatedLocation"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.SCALE_ORDERS3
                && order.ScalePriceIncrement > 0
                && order.ScalePriceIncrement != double.MaxValue
            )
            {
                if (
                    order.ScalePriceAdjustValue != double.MaxValue
                    || order.ScalePriceAdjustInterval != int.MaxValue
                    || order.ScaleProfitOffset != double.MaxValue
                    || order.ScaleAutoReset
                    || order.ScaleInitPosition != int.MaxValue
                    || order.ScaleInitFillQty != int.MaxValue
                    || order.ScaleRandomPercent
                )
                {
                    ReportError(
                        id,
                        Util.CurrentTimeMillis(),
                        ClientErrors.UPDATE_TWS,
                        "  It does not support Scale order parameters: PriceAdjustValue, PriceAdjustInterval, ProfitOffset, AutoReset, InitPosition, InitFillQty and RandomPercent"
                    );
                    return false;
                }
            }

            if (
                ServerVersion < MinServerVer.ORDER_COMBO_LEGS_PRICE
                && isBagOrder
                && order.OrderComboLegs.Count > 0
            )
            {
                OrderComboLeg orderComboLeg;
                for (var i = 0; i < order.OrderComboLegs.Count; ++i)
                {
                    orderComboLeg = order.OrderComboLegs[i];
                    if (orderComboLeg.Price == double.MaxValue)
                        continue;
                    ReportError(
                        id,
                        Util.CurrentTimeMillis(),
                        ClientErrors.UPDATE_TWS,
                        "  It does not support per-leg prices for order combo legs."
                    );
                    return false;
                }
            }

            if (
                ServerVersion < MinServerVer.TRAILING_PERCENT
                && order.TrailingPercent != double.MaxValue
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support trailing percent parameter."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.ALGO_ID && !IsEmpty(order.AlgoId))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support algoId parameter"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.SCALE_TABLE
                && (
                    !IsEmpty(order.ScaleTable)
                    || !IsEmpty(order.ActiveStartTime)
                    || !IsEmpty(order.ActiveStopTime)
                )
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support scaleTable, activeStartTime nor activeStopTime parameters."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.EXT_OPERATOR && !IsEmpty(order.ExtOperator))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support extOperator parameter"
                );
                return false;
            }

            if (ServerVersion < MinServerVer.CASH_QTY && order.CashQty != double.MaxValue)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support cashQty parameter"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.DECISION_MAKER
                && (!IsEmpty(order.Mifid2DecisionMaker) || !IsEmpty(order.Mifid2DecisionAlgo))
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support MIFID II decision maker parameters"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.DECISION_MAKER
                && (!IsEmpty(order.Mifid2ExecutionTrader) || !IsEmpty(order.Mifid2ExecutionAlgo))
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support MIFID II execution parameters"
                );
                return false;
            }

            if (ServerVersion < MinServerVer.AUTO_PRICE_FOR_HEDGE && order.DontUseAutoPriceForHedge)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support don't use auto price for hedge parameter"
                );
                return false;
            }

            if (ServerVersion < MinServerVer.ORDER_CONTAINER && order.IsOmsContainer)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support oms container parameter."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.D_PEG_ORDERS && order.DiscretionaryUpToLimitPrice)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support D-Peg orders."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.PRICE_MGMT_ALGO && order.UsePriceMgmtAlgo.HasValue)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support Use Price Management Algo requests."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.DURATION && order.Duration != int.MaxValue)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support duration attribute."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.POST_TO_ATS && order.PostToAts != int.MaxValue)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support postToAts attribute."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.AUTO_CANCEL_PARENT && order.AutoCancelParent)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support autoCancelParent attribute."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.ADVANCED_ORDER_REJECT
                && !IsEmpty(order.AdvancedErrorOverride)
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support advanced error override attribute."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.MANUAL_ORDER_TIME && !IsEmpty(order.ManualOrderTime))
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support manual order time attribute."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.PEGBEST_PEGMID_OFFSETS
                && (
                    order.MinTradeQty != int.MaxValue
                    || order.MinCompeteSize != int.MaxValue
                    || order.CompeteAgainstBestOffset != double.MaxValue
                    || order.MidOffsetAtWhole != double.MaxValue
                    || order.MidOffsetAtHalf != double.MaxValue
                )
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    "  It does not support PEG BEST / PEG MID order parameters: minTradeQty, minCompeteSize, competeAgainstBestOffset, midOffsetAtWhole and midOffsetAtHalf"
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.MIN_SERVER_VER_CUSTOMER_ACCOUNT
                && !IsEmpty(order.CustomerAccount)
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support customer account parameter."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.MIN_SERVER_VER_PROFESSIONAL_CUSTOMER
                && order.ProfessionalCustomer
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support professional customer parameter."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.MIN_SERVER_VER_INCLUDE_OVERNIGHT
                && order.IncludeOvernight
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support include overnight parameter."
                );
                return false;
            }

            if (
                ServerVersion < MinServerVer.MIN_SERVER_VER_CME_TAGGING_FIELDS
                && order.ManualOrderIndicator != int.MaxValue
            )
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support manual order indicator parameter."
                );
                return false;
            }

            if (ServerVersion < MinServerVer.MIN_SERVER_VER_IMBALANCE_ONLY && order.ImbalanceOnly)
            {
                ReportError(
                    id,
                    Util.CurrentTimeMillis(),
                    ClientErrors.UPDATE_TWS,
                    " It does not support imbalance only parameter."
                );
                return false;
            }

            return true;
        }

        private bool IsEmpty(string str) => Util.StringIsEmpty(str);

        private bool StringsAreEqual(string a, string b) => string.Compare(a, b, true) == 0;

        public bool IsDataAvailable()
        {
            if (!IsConnected)
                return false;
            return !(TcpStream is NetworkStream networkStream) || networkStream.DataAvailable;
        }

        public int ReadInt() =>
            IPAddress.NetworkToHostOrder(new BinaryReader(TcpStream).ReadInt32());

        public byte[] ReadAtLeastNBytes(int msgSize)
        {
            var buf = new byte[msgSize];
            return buf.Take(TcpStream.Read(buf, 0, msgSize)).ToArray();
        }

        public byte[] ReadByteArray(int msgSize) => new BinaryReader(TcpStream).ReadBytes(msgSize);
    }
}
