using IBApi;
using IBKRWrapper;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Models;
using SimpleBroker.EventHandlers;
using SimpleBroker.LockObjects;

namespace SimpleBroker
{
    /// <summary>
    /// Wraps an IBKR EWrapper implementation to provide a more straightforward public API.
    /// </summary>
    public class Broker
    {
        private readonly Wrapper _wrapper;
        private readonly Locks _locks;

        /// <summary>
        /// Indicates whether the wrapper has a connection to the IBKR TWS/Gateway.
        /// </summary>
        public bool IsConnected
        {
            get => _wrapper.IsConnected;
        }

        /// <summary>
        /// Public constructor for the <see cref="Broker"/> class.
        /// </summary>
        public Broker()
        {
            _wrapper = new Wrapper();
            _locks = new Locks();
        }

        private Broker(Wrapper wrapper)
        {
            _wrapper = wrapper;
            _locks = new Locks();
        }

        #region Connection Methods

        /// <summary>
        /// Initiates a connection to the IBKR TWS/Gateway.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="clientId"></param>
        public void Connect(string host, int port, int clientId)
        {
            _wrapper.Connect(host, port, clientId);
        }

        /// <summary>
        /// Disconnects from the IBKR TWS/Gateway.
        /// </summary>
        public void Disconnect()
        {
            _wrapper.Disconnect();
        }

        #endregion

        #region Account and Position Methods

        /// <summary>
        /// Gets the values for a given account.
        /// </summary>
        /// <param name="account">The IBKR account number to get positions for</param>
        /// <returns>A dictionary of <see cref="string"/> name/value pairs</returns>
        public Task<Dictionary<string, string>> GetAccountValues(string account)
        {
            TaskCompletionSource<Dictionary<string, string>> tcs = new();
            lock (_locks.accountValuesLock)
            {
                Dictionary<string, string> accountValues = [];

                var updateAccountValueHandler = Handlers.UpdateAccountValueHandler(
                    account,
                    accountValues
                );
                var accountDownloadEndHandler = Handlers.AccountDownloadEndHandler(
                    account,
                    tcs,
                    accountValues
                );

                _wrapper.UpdateAccountValueEvent += updateAccountValueHandler;
                _wrapper.AccountDownloadEndEvent += accountDownloadEndHandler;

                tcs.Task.ContinueWith(t =>
                {
                    _wrapper.UpdateAccountValueEvent -= updateAccountValueHandler;
                    _wrapper.AccountDownloadEndEvent -= accountDownloadEndHandler;
                });

                _wrapper.ClientSocket.reqAccountUpdates(true, account);
            }
            return tcs.Task;
        }

        /// <summary>
        /// Gets a portfolio's positions for a given account.
        /// </summary>
        /// <param name="account">The IBKR account number to get positions for</param>
        /// <returns>A list of <see cref="PortfolioPosition"/> objects representing the account's positions</returns>
        /// <remarks>
        ///     If you have multiple accounts and want all positions across all accounts, use <see cref="GetPositions"/> instead.
        /// </remarks>
        public Task<List<PortfolioPosition>> GetPortfolioPositions(string account)
        {
            List<PortfolioPosition> positions = [];
            TaskCompletionSource<List<PortfolioPosition>> tcs = new();

            lock (_locks.accountValuesLock)
            {
                var updatePortfolioHandler = Handlers.UpdatePortfolioHandler(positions);
                var accountDownloadEndHandler = Handlers.AccountDownloadEndHandler(
                    account,
                    tcs,
                    positions
                );

                _wrapper.UpdatePortfolioEvent += updatePortfolioHandler;
                _wrapper.AccountDownloadEndEvent += accountDownloadEndHandler;

                tcs.Task.ContinueWith(t =>
                {
                    _wrapper.UpdatePortfolioEvent -= updatePortfolioHandler;
                    _wrapper.AccountDownloadEndEvent -= accountDownloadEndHandler;
                });

                _wrapper.ClientSocket.reqAccountUpdates(true, account);
            }
            return tcs.Task;
        }

        /// <summary>
        /// Gets the account's positions.
        /// </summary>
        /// <returns>A list of <see cref="Position"/> objects</returns>
        /// <remarks>
        ///     The primary reason to call this method instead of <see cref="GetPortfolioPositions"/> is to get a quick list of positions for all accounts. <see cref="GetPortfolioPositions"/> makes you enter an account number and will return positions only for that account, and also includes PnL information on each position.
        /// </remarks>
        public Task<List<Position>> GetPositions()
        {
            TaskCompletionSource<List<Position>> tcs = new();

            lock (_locks.positionLock)
            {
                List<Position> positions = [];

                var positionHandler = Handlers.PositionHandler(positions);
                var positionEnd = Handlers.PositionEndHandler(positions, tcs);

                _wrapper.PositionEvent += positionHandler;
                _wrapper.PositionEndEvent += positionEnd;

                tcs.Task.ContinueWith(t =>
                {
                    _wrapper.PositionEvent -= positionHandler;
                    _wrapper.PositionEndEvent -= positionEnd;
                });

                _wrapper.ClientSocket.reqPositions();

                return tcs.Task;
            }
        }
        #endregion

        #region Contract Methods

        /// <summary>
        /// Gets the fully defined contracts (i.e., with all details included) for a given symbol.
        /// </summary>
        /// <param name="symbol">The instrument's ticker symbol</param>
        /// <param name="securityType">The instrument's security type</param>
        /// <param name="exchange">Optional parameter to specify an exchange (defaults to SMART)</param>
        /// <param name="currency">Optional parameter to specify a currency (defaults to USD)</param>
        /// <returns>A list of <see cref="Contract"/> objects</returns>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public Task<List<Contract>> GetFullyDefinedContracts(
            string symbol,
            string securityType,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            TaskCompletionSource<List<Contract>> tcs = new();
            List<Contract> contracts = [];

            IBApi.Contract contract =
                new()
                {
                    Symbol = symbol,
                    SecType = securityType,
                    Exchange = exchange,
                    Currency = currency
                };

            var contractDetailsHandler = Handlers.ContractDetailsHandler(contracts, reqId);
            var contractDetailsEndHandler = Handlers.ContractDetailsEndHandler(
                contracts,
                reqId,
                tcs
            );

            _wrapper.ContractDetailsEvent += contractDetailsHandler;
            _wrapper.ContractDetailsEndEvent += contractDetailsEndHandler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.ContractDetailsEvent -= contractDetailsHandler;
                _wrapper.ContractDetailsEndEvent -= contractDetailsEndHandler;
            });

            _wrapper.ClientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        /// <summary>
        /// Gets the fully defined contracts (i.e., with all details included) for an option on a specific date.
        /// </summary>
        /// <param name="symbol">The underlying's symbol</param>
        /// <param name="date">The option expiration date, in the format "YYYYMMDD"</param>
        /// <param name="right">The option's right, entered as "PUT", "P", "CALL", or "C"</param>
        /// <param name="exchange">The option's exchange</param>
        /// <param name="currency">The option's currency</param>
        /// <returns>A list of <see cref="Contract"/> objects.</returns>
        /// <remarks>
        ///     <para>Valid values for the "right" parameter are "P", "PUT", "C" or "CALL"</para>
        ///     <para>The date should passed in the format "YYYYMMDD"</para>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public Task<List<Contract>> GetFullyDefinedOptionContractsByDate(
            string symbol,
            string date,
            string right,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            TaskCompletionSource<List<Contract>> tcs = new();
            List<Contract> contracts = [];

            IBApi.Contract contract =
                new()
                {
                    Symbol = symbol,
                    SecType = "OPT",
                    LastTradeDateOrContractMonth = date,
                    Right = right,
                    Exchange = exchange,
                    Currency = currency
                };

            var contractDetailsHandler = Handlers.ContractDetailsHandler(contracts, reqId);
            var contractDetailsEndHandler = Handlers.ContractDetailsEndHandler(
                contracts,
                reqId,
                tcs
            );

            _wrapper.ContractDetailsEvent += contractDetailsHandler;
            _wrapper.ContractDetailsEndEvent += contractDetailsEndHandler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.ContractDetailsEvent -= contractDetailsHandler;
                _wrapper.ContractDetailsEndEvent -= contractDetailsEndHandler;
            });

            _wrapper.ClientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        /// <summary>
        /// Gets the fully defined contracts (i.e., with all details included) for an option on a specific date.
        /// </summary>
        /// <param name="symbol">The underlying's symbol</param>
        /// <param name="date">The option expiration date</param>
        /// <param name="right">The option's right, entered as "PUT", "P", "CALL", or "C"</param>
        /// <param name="exchange">The option's exchange</param>
        /// <param name="currency">The option's currency</param>
        /// <returns>A list of <see cref="Contract"/> objects.</returns>
        /// <remarks>
        ///     <para>Valid values for the "right" parameter are "P", "PUT", "C" or "CALL"</para>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public async Task<List<Contract>> GetFullyDefinedOptionContractsByDate(
            string symbol,
            DateTime date,
            string right,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            return await GetFullyDefinedOptionContractsByDate(
                symbol,
                date.ToString("yyyyMMdd"),
                right,
                exchange,
                currency
            );
        }

        /// <summary>
        /// Gets the fully defined contracts (i.e., with all details included) for an option on a specific date.
        /// </summary>
        /// <param name="symbol">The underlying's symbol</param>
        /// <param name="date">The option expiration date</param>
        /// <param name="right">The option's right, entered as "PUT", "P", "CALL", or "C"</param>
        /// <param name="exchange">The option's exchange</param>
        /// <param name="currency">The option's currency</param>
        /// <returns>A list of <see cref="Contract"/> objects.</returns>
        /// <remarks>
        ///     <para>Valid values for the "right" parameter are "P", "PUT", "C" or "CALL"</para>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public async Task<List<Contract>> GetFullyDefinedOptionContractsByDate(
            string symbol,
            DateOnly date,
            string right,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            return await GetFullyDefinedOptionContractsByDate(
                symbol,
                date.ToString("yyyyMMdd"),
                right,
                exchange,
                currency
            );
        }

        /// <summary>
        /// Gets the fully defined contracts (i.e., with all details included) for an option on a specific date.
        /// </summary>
        /// <param name="symbol">The underlying's symbol</param>
        /// <param name="date">The option expiration date</param>
        /// <param name="right">The option's right, entered as "PUT", "P", "CALL", or "C"</param>
        /// <param name="exchange">The option's exchange</param>
        /// <param name="currency">The option's currency</param>
        /// <returns>A list of <see cref="Contract"/> objects.</returns>
        /// <remarks>
        ///     <para>Valid values for the "right" parameter are "P", "PUT", "C" or "CALL"</para>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public async Task<List<Contract>> GetFullyDefinedOptionContractsByDate(
            string symbol,
            DateTimeOffset date,
            string right,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            return await GetFullyDefinedOptionContractsByDate(
                symbol,
                date.ToString("yyyyMMdd"),
                right,
                exchange,
                currency
            );
        }

        /// <summary>
        /// Gets the option chain (i.e., all available expirations and strikes) for a given security.
        /// </summary>
        /// <param name="symbol">The security's ticker symbol</param>
        /// <param name="securityType">The underlying security's type</param>
        /// <param name="exchange">The underlying security's exchange</param>
        /// <param name="currency">The underlying security's currency</param>
        /// <returns>An <see cref="OptionsChain"/> object which will receive the strikes and expirations from IBKR and save them in a hashset</returns>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public Task<OptionsChain> GetOptionsChain(
            string symbol,
            string securityType,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            TaskCompletionSource<OptionsChain> tcs = new();
            OptionsChain result = new() { ReqId = reqId, Symbol = symbol };
            var handler = Handlers.OptionsChainEndHandler(tcs, result);
            List<Contract> cons = GetFullyDefinedContracts(
                symbol,
                securityType,
                exchange,
                currency
            ).Result;
            int conId = cons[0].ConId;

            _wrapper.OptionsChainEvent += result.HandleOptionsChainData;
            _wrapper.OptionsChainEndEvent += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.OptionsChainEvent -= result.HandleOptionsChainData;
                _wrapper.OptionsChainEndEvent -= handler;
            });

            _wrapper.ClientSocket.reqSecDefOptParams(reqId, symbol, "", "STK", conId);

            return tcs.Task;
        }

        #endregion

        #region Historical Data Methods

        /// <summary>
        ///     <para>Get historical bars from IBKR.</para>
        ///     <example><i>Example use: get hourly bars for the last 5 days for AAPL:</i>
        ///         <code>
        ///             Contract contract = new() { Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD" };
        ///             var bars = await GetHistoricalBars(contract, "5 D", "1 hr", "TRADES");
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="contract">the underlying IBKR <see cref="Contract"/></param>
        /// <param name="durationString">The amount of time to go back from the request's given end date and time</param>
        /// <param name="barSizeSetting">The data's granularity / bar size</param>
        /// <param name="whatToShow">The type of data to receive</param>
        /// <param name="useRTH">Whether to use real time hours only (true) or not (false)</param>
        /// <param name="formatDate">Format of the date returned by IBKR</param>
        /// <param name="endDateTime">The request's end date and time (empty string indicates current moment)</param>
        /// <returns>A list of <see cref="Bar"/> objects.</returns>
        /// <remarks>
        ///     <para><b>Parameter Notes:</b></para>
        ///     <list type="table">
        ///         <listheader><description><u>Valid <paramref name="durationString"/> Options</u>:</description></listheader>
        ///         <item>
        ///             <term>S</term><description> Seconds</description>
        ///         </item>
        ///         <item>
        ///             <term>D</term><description> Day</description>
        ///         </item>
        ///         <item>
        ///             <term>W</term><description> Week</description>
        ///         </item>
        ///         <item>
        ///             <term>M</term><description> Month</description>
        ///         </item>
        ///         <item>
        ///             <term>Y</term><description> Year</description>
        ///         </item>
        ///     </list>
        ///     <list type="table">
        ///         <listheader><description><u>Valid <paramref name="barSizeSetting"/> Options (Bar Unit - Bar Sizes)</u>:</description></listheader>
        ///         <item>
        ///             <term>sec</term><description> 1, 5, 10, 15, 30</description>
        ///         </item>
        ///         <item>
        ///             <term>min</term><description> 1, 2, 3, 5, 10, 15, 20, 30</description>
        ///         </item>
        ///         <item>
        ///             <term>hr</term><description> 1, 2, 3, 4, 8</description>
        ///         </item>
        ///         <item>
        ///             <term>day</term><description> 1</description>
        ///         </item>
        ///         <item>
        ///             <term>week</term><description> 1</description>
        ///         </item>
        ///         <item>
        ///             <term>month</term><description> 1</description>
        ///         </item>
        ///     </list>
        ///     <listheader><description><u>Valid <paramref name="whatToShow"/> Options</u>:</description></listheader>
        ///     <list type="bullet">
        ///         <item>
        ///             <description> AGGTRADES (cryptocurrency only)</description>
        ///         </item>
        ///         <item>
        ///             <description> ASK</description>
        ///         </item>
        ///         <item>
        ///             <description> BID</description>
        ///         </item>
        ///         <item>
        ///             <description> BID_ASK</description>
        ///         </item>
        ///         <item>
        ///             <description> FEE_RATE (stocks and ETFs only)</description>
        ///         </item>
        ///         <item>
        ///             <description> HISTORICAL_VOLATILITY (ETFs, indices and stocks only)</description>
        ///         </item>
        ///         <item>
        ///             <description> MIDPOINT</description>
        ///         </item>
        ///         <item>
        ///             <description> OPTION_IMPLIED_VOLATILITY (ETFs, indices and stocks only)</description>
        ///         </item>
        ///         <item>
        ///             <description> SCHEDULE</description>
        ///         </item>
        ///         <item>
        ///             <description> TRADES (note that trades is adjusted for splits but not dividends)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_ASK (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_BID (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_BID_ASK (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_LAST (corporate bonds only)</description>
        ///         </item>
        ///     </list>
        ///     <list type="table">
        ///         <listheader><description><u><paramref name="formatDate"/> Options</u>:</description></listheader>
        ///         <item>
        ///             <term>1</term><description> String Time Zone Date (e.g., “20231019 16:11:48 America/New_York”)</description>
        ///         </item>
        ///         <item>
        ///             <term>2</term><description> Epoch Date (e.g., 1697746308)</description>
        ///         </item>
        ///         <item>
        ///             <term>3</term><description> Day and Time Date (e.g., “1019 16:11:48 America/New_York”)</description>
        ///         </item>
        ///     </list>
        ///     <para><u><paramref name="endDateTime"/> format</u>: "YYYYMMDD HH:mm:ss". Empty string indicates current present moment.</para>
        /// </remarks>
        public Task<List<Bar>> GetHistoricalBars(
            Contract contract,
            string durationString,
            string barSizeSetting,
            string whatToShow,
            bool useRTH = true,
            int formatDate = 1,
            string endDateTime = ""
        )
        {
            int reqId;
            List<Bar> bars = [];
            TaskCompletionSource<List<Bar>> tcs = new();

            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            var handler = Handlers.HistoricalDataHandler(bars, reqId);

            var endHandler = Handlers.HistoricalDataEndHandler(bars, reqId, tcs);

            var errorHandler = Handlers.ErrorHandler(reqId, tcs);

            _wrapper.HistoricalData += handler;
            _wrapper.HistoricalDataEnd += endHandler;
            _wrapper.ErrorEvent += errorHandler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.HistoricalData -= handler;
                _wrapper.HistoricalDataEnd -= endHandler;
                _wrapper.ErrorEvent -= errorHandler;
            });

            int rth = useRTH ? 1 : 0;

            _wrapper.ClientSocket.reqHistoricalData(
                reqId,
                contract.ToIBKRContract(),
                endDateTime,
                durationString,
                barSizeSetting,
                whatToShow,
                rth,
                formatDate,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        ///     <para>Get historical "last" ticks from IBKR.</para>
        ///     <example><i>Example use: get 500 "last" ticks, beginning at market open on January 8, 2024:</i>
        ///         <code>
        ///             Contract contract = new() { Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD" };
        ///             var bars = await GetHistoricalTicksLast(contract, "20240108 09:30:00", "", 500, true);
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="contract">the underlying IBKR <see cref="Contract"/></param>
        /// <param name="startDateTime">The request's start date and time</param>
        /// <param name="endDateTime">The request's end date and time</param>
        /// <param name="numberOfTicks">Total number of distinct data points (max is 1000)</param>
        /// <param name="useRth">Whether to use real time hours only (true) or not (false)</param>
        /// <returns>A list of <see cref="HistoricalTickLast"/> objects.</returns>
        /// <remarks>
        ///     <para><b>Parameter Notes:</b></para>
        ///     <para>Exactly one of <paramref name="startDateTime"/> or <paramref name="endDateTime"/> must be defined in the form "YYYYMMDD HH:mm:ss". Pass an empty string for the other.</para>
        ///     <para>Note that the max <paramref name="numberOfTicks"/> is 1000 per request.</para>
        /// </remarks>
        public Task<List<HistoricalTickLast>> GetHistoricalTicksLast(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            bool useRth
        )
        {
            int reqId;
            List<HistoricalTickLast> ticks = [];
            TaskCompletionSource<List<HistoricalTickLast>> tcs = new();

            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            var handler = Handlers.HistoricalTicksLastHandler(ticks, reqId, tcs);

            _wrapper.HistoricalTicksLast += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.HistoricalTicksLast -= handler;
            });

            int rth = useRth ? 1 : 0;

            _wrapper.ClientSocket.reqHistoricalTicks(
                reqId,
                contract.ToIBKRContract(),
                startDateTime,
                endDateTime,
                numberOfTicks,
                "TRADES",
                rth,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        ///     <para>Get historical bid/ask ticks from IBKR.</para>
        ///     <example><i>Example use: get 500 ticks, beginning at market open on January 8, 2024:</i>
        ///         <code>
        ///             Contract contract = new() { Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD" };
        ///             var bars = await GetHistoricalTicksBidAsk(contract, "20240108 09:30:00", "", 500, true, false);
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="contract">the underlying IBKR <see cref="Contract"/></param>
        /// <param name="startDateTime">The request's start date and time</param>
        /// <param name="endDateTime">The request's end date and time</param>
        /// <param name="numberOfTicks">Total number of distinct data points (max is 1000)</param>
        /// <param name="useRth">Whether to use real time hours only (true) or not (false)</param>
        /// <param name="ignoreSize">Whether to ignore the size of the bid/ask (true) or not (false)</param>
        /// <returns>A list of <see cref="HistoricalTickBidAsk"/> objects.</returns>
        /// <remarks>
        ///     <para><b>Parameter Notes:</b></para>
        ///     <para>Exactly one of <paramref name="startDateTime"/> or <paramref name="endDateTime"/> must be defined in the form "YYYYMMDD HH:mm:ss". Pass an empty string for the other.</para>
        ///     <para>Note that the max <paramref name="numberOfTicks"/> is 1000 per request.</para>
        /// </remarks>
        public Task<List<HistoricalTickBidAsk>> GetHistoricalTicksBidAsk(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            bool useRth,
            bool ignoreSize
        )
        {
            int reqId;
            List<HistoricalTickBidAsk> ticks = [];
            TaskCompletionSource<List<HistoricalTickBidAsk>> tcs = new();

            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            var handler = Handlers.HistoricalTicksBidAskHandler(ticks, reqId, tcs);

            _wrapper.HistoricalTicksBidAsk += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.HistoricalTicksBidAsk -= handler;
            });

            int rth = useRth ? 1 : 0;
            _wrapper.ClientSocket.reqHistoricalTicks(
                reqId,
                contract.ToIBKRContract(),
                startDateTime,
                endDateTime,
                numberOfTicks,
                "BID_ASK",
                rth,
                ignoreSize,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        ///     <para>Get historical midpoint ticks from IBKR.</para>
        ///     <example><i>Example use: get 500 ticks, beginning at market open on January 8, 2024:</i>
        ///         <code>
        ///             Contract contract = new() { Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD" };
        ///             var bars = await GetHistoricalTicksMid(contract, "20240108 09:30:00", "", 500, true);
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="contract">the underlying IBKR <see cref="Contract"/></param>
        /// <param name="startDateTime">The request's start date and time</param>
        /// <param name="endDateTime">The request's end date and time</param>
        /// <param name="numberOfTicks">Total number of distinct data points (max is 1000)</param>
        /// <param name="useRth">Whether to use real time hours only (true) or not (false)</param>
        /// <returns>A list of <see cref="HistoricalTick"/> objects.</returns>
        /// <remarks>
        ///     <para><b>Parameter Notes:</b></para>
        ///     <para>Exactly one of <paramref name="startDateTime"/> or <paramref name="endDateTime"/> must be defined in the form "YYYYMMDD HH:mm:ss". Pass an empty string for the other.</para>
        ///     <para>Note that the max <paramref name="numberOfTicks"/> is 1000 per request.</para>
        /// </remarks>
        public Task<List<HistoricalTick>> GetHistoricalTicksMid(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            bool useRth
        )
        {
            int reqId;
            List<HistoricalTick> ticks = [];
            TaskCompletionSource<List<HistoricalTick>> tcs = new();

            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            var handler = Handlers.HistoricalTicksMidHandler(ticks, reqId, tcs);

            _wrapper.HistoricalTicksMid += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.HistoricalTicksMid -= handler;
            });

            int rth = useRth ? 1 : 0;
            _wrapper.ClientSocket.reqHistoricalTicks(
                reqId,
                contract.ToIBKRContract(),
                startDateTime,
                endDateTime,
                numberOfTicks,
                "MIDPOINT",
                rth,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>Get the earliest historical data timestamp for a given contract.</summary>
        /// <param name="contract">the underlying IBKR <see cref="Contract"/></param>
        /// <param name="whatToShow">The type of data to receive</param>
        /// <param name="useRTH">Whether to use real time hours only (true) or not (false)</param>
        /// <param name="formatDate">Format of the date returned by IBKR</param>
        /// <returns>A <see cref="DateTimeOffset"/> indicating the timestamp.</returns>
        /// <remarks>
        ///     <para><b>Parameter Notes:</b></para>
        ///     <listheader><description><u>Valid <paramref name="whatToShow"/> Options</u>:</description></listheader>
        ///     <list type="bullet">
        ///         <item>
        ///             <description> AGGTRADES (cryptocurrency only)</description>
        ///         </item>
        ///         <item>
        ///             <description> ASK</description>
        ///         </item>
        ///         <item>
        ///             <description> BID</description>
        ///         </item>
        ///         <item>
        ///             <description> BID_ASK</description>
        ///         </item>
        ///         <item>
        ///             <description> FEE_RATE (stocks and ETFs only)</description>
        ///         </item>
        ///         <item>
        ///             <description> HISTORICAL_VOLATILITY (ETFs, indices and stocks only)</description>
        ///         </item>
        ///         <item>
        ///             <description> MIDPOINT</description>
        ///         </item>
        ///         <item>
        ///             <description> OPTION_IMPLIED_VOLATILITY (ETFs, indices and stocks only)</description>
        ///         </item>
        ///         <item>
        ///             <description> SCHEDULE</description>
        ///         </item>
        ///         <item>
        ///             <description> TRADES (note that trades is adjusted for splits but not dividends)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_ASK (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_BID (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_BID_ASK (corporate bonds only)</description>
        ///         </item>
        ///         <item>
        ///             <description> YIELD_LAST (corporate bonds only)</description>
        ///         </item>
        ///     </list>
        ///     <list type="table">
        ///         <listheader><description><u><paramref name="formatDate"/> Options</u>:</description></listheader>
        ///         <item>
        ///             <term>1</term><description> String Time Zone Date (e.g., “20231019 16:11:48 America/New_York”)</description>
        ///         </item>
        ///         <item>
        ///             <term>2</term><description> Epoch Date (e.g., 1697746308)</description>
        ///         </item>
        ///         <item>
        ///             <term>3</term><description> Day and Time Date (e.g., “1019 16:11:48 America/New_York”)</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public Task<DateTimeOffset> GetHeadTimestamp(
            Contract contract,
            string whatToShow,
            bool useRTH,
            int formatDate
        )
        {
            int reqId;
            TaskCompletionSource<DateTimeOffset> tcs = new();

            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            var handler = Handlers.HeadTimestampHandler(tcs, reqId);

            _wrapper.HeadTimestamp += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.HeadTimestamp -= handler;
            });

            int rth = useRTH ? 1 : 0;
            _wrapper.ClientSocket.reqHeadTimestamp(
                reqId,
                contract.ToIBKRContract(),
                whatToShow,
                rth,
                formatDate
            );

            return tcs.Task;
        }

        #endregion

        #region Market Data Methods

        /// <summary>
        /// Subscribe to real-time market data for a given contract.
        /// </summary>
        /// <param name="contract">The underlying contract</param>
        /// <param name="whatToShow">Type of data to retrieve. Available values include "TRADES", "MIDPOINT", "BID" and "ASK"</param>
        /// <param name="useRTH">Whether to use only real-time data or not</param>
        /// <returns>A <see cref="RealTimeBarList"/> object, which receives and stores real-time bars from IBKR, and includes an event to emit bars upon receipt</returns>
        /// <remarks>
        ///     <para><u>Notes from IBKR</u>:</para>
        ///     <para>Only 5 seconds bars are provided. This request is subject to the same pacing as any historical data request: no more than 60 API queries in more than 600 seconds.</para>
        ///     <para>Real time bars subscriptions are also included in the calculation of the number of Level 1 market data subscriptions allowed in an account.</para>
        /// </remarks>
        public RealTimeBarList GetRealTimeBars(Contract contract, string whatToShow, bool useRTH)
        {
            // The barSize parameter in IBKR's reqRealTimeBars method is currently ignored and bar size is fixed at 5 seconds, so we just pass 5 to the method.
            const int BARSIZE = 5;

            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            RealTimeBarList rtBars =
                new(reqId, contract.ToIBKRContract(), BARSIZE, whatToShow, useRTH);

            _wrapper.RealTimeBarEvent += rtBars.HandleNewBar;

            _wrapper.ClientSocket.reqRealTimeBars(
                reqId,
                contract.ToIBKRContract(),
                BARSIZE,
                whatToShow,
                useRTH,
                []
            );

            return rtBars;
        }

        /// <summary>
        /// Request real-time watchlist market data for a contract.
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> for the instrument for which data is being requested</param>
        /// <returns>A <see cref="LiveMarketData"/> object which receives and stores data emitted by IBKR</returns>
        public LiveMarketData RequestMarketData(Contract contract)
        {
            if (_wrapper.LastMarketDataTypeRequested != MarketDataType.Live)
            {
                _wrapper.SetMarketDataLive();
            }

            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            LiveMarketData data = new(reqId, contract);

            _wrapper.StringMarketDataEvent += data.UpdateMarketData;
            _wrapper.DecimalMarketDataEvent += data.UpdateMarketData;
            _wrapper.DoubleMarketDataEvent += data.UpdateMarketData;
            _wrapper.OptionGreeksMarketDataEvent += data.UpdateMarketData;

            _wrapper.ClientSocket.reqMktData(
                reqId,
                contract.ToIBKRContract(),
                "",
                false,
                false,
                []
            );

            return data;
        }

        /// <summary>
        /// Request frozen market data (i.e., the last data recorded, or the data recorded at market close if the request is made after market hours) for a contract.
        /// </summary>
        /// <param name="contract">Contract for the instrument for which data is being requested</param>
        /// <returns>A <see cref="FrozenMarketData"/> object which will receive and store the data sent by IBKR</returns>
        /// <remarks>
        /// IBKR emits each data point as a separate event, and there is no event or other method to indicate that all data has been emitted, so the <see cref="FrozenMarketData"/> object will receive data after it returns from this method. Generally the data is received and processed very quickly, but you will likely need to add a minor wait time before accessing the <see cref="FrozenMarketData"/> object's properties after calling this method.
        /// </remarks>
        public FrozenMarketData RequestFrozenMarketData(Contract contract)
        {
            if (_wrapper.LastMarketDataTypeRequested != MarketDataType.Frozen)
            {
                _wrapper.SetMarketDataFrozen();
            }

            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            FrozenMarketData data = new(reqId, contract.ToIBKRContract());

            _wrapper.StringMarketDataEvent += data.UpdateMarketData;
            _wrapper.DecimalMarketDataEvent += data.UpdateMarketData;
            _wrapper.DoubleMarketDataEvent += data.UpdateMarketData;
            _wrapper.OptionGreeksMarketDataEvent += data.UpdateMarketData;

            _wrapper.ClientSocket.reqMktData(
                reqId,
                contract.ToIBKRContract(),
                "",
                false,
                false,
                []
            );

            return data;
        }

        /// <summary>
        /// Calculates an option's implied volatility based on its price and the price of the underlying.
        /// </summary>
        /// <param name="contract">An option contract</param>
        /// <param name="optionPrice">The hypothetical option price</param>
        /// <param name="underlyingPrice">The hypothetical price of the underlying</param>
        /// <returns>an <see cref="OptionGreeks"/> object holding the value sent by IBKR</returns>
        /// <remarks>
        /// Note that the value returned is calculated by IBKR on their end.
        /// </remarks>
        public Task<OptionGreeks> CalculateOptionIV(
            Contract contract,
            double optionPrice,
            double underlyingPrice
        )
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            TaskCompletionSource<OptionGreeks> tcs = new();

            void handler(object? sender, MarketDataEventArgs<IBKRWrapper.Models.OptionGreeks> e)
            {
                if (e.Data.ReqId == reqId)
                    tcs.SetResult(e.Data.Value.ToBrokerOptionGreeks());
            }

            _wrapper.OptionGreeksMarketDataEvent += handler;

            tcs.Task.ContinueWith(t =>
            {
                _wrapper.OptionGreeksMarketDataEvent -= handler;
            });

            _wrapper.ClientSocket.calculateImpliedVolatility(
                reqId,
                contract.ToIBKRContract(),
                optionPrice,
                underlyingPrice,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        /// Gets tick by tick data for a contract.
        /// </summary>
        /// <param name="contract">The underlying contract</param>
        /// <param name="tickType">The type of data requested. Valid inputs include "Last", "AllLast", "BidAsk" or "Midpoint"</param>
        /// <returns>A <see cref="TickDataList"/> object, which receives and stores the tick data sent by IBKR</returns>
        /// <remarks>
        ///     <para><u>Notes from IBKR</u>:</para>
        ///     <para>The maximum number of simultaneous tick-by-tick subscriptions allowed for a user is determined by the same formula used to calculate maximum number of market depth subscriptions Limitations. For some securities, getting tick-by-tick data requires Level 2 data bundles.</para>
        ///     <list type="bullet">
        ///         <item>
        ///             Real time tick-by-tick data is currently not available for options. Historical tick-by-tick data is                    available.
        ///         </item>
        ///         <item>The tick type field is case sensitive. It must be BidAsk, Last, AllLast, MidPoint. AllLast has additional trade types such as combos, derivatives, and average price trades which are not included in Last.</item>
        ///         <item>
        ///             Tick-by-tick data for options is currently only available historically and not in real time.
        ///         </item>
        ///         <item>
        ///             Tick-by-tick data for indices is only provided for indices which are on CME.
        ///         </item>
        ///         <item>
        ///             Tick-by-tick data is not available for combos.
        ///         </item>
        ///         <item>
        ///             No more than 1 tick-by-tick request can be made for the same instrument within 15 seconds.
        ///         </item>
        ///         <item>
        ///             Time and Sales data requires a Level 1, Top Of Book market data subscription.
        ///         </item>
        ///     </list>
        /// </remarks>
        public TickDataList GetTickByTickData(Contract contract, string tickType)
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            TickDataList tickDataList = new(reqId, contract.ToIBKRContract(), tickType);

            _wrapper.TickByTickEvent += tickDataList.HandleNewTick;

            _wrapper.ClientSocket.reqTickByTickData(
                reqId,
                contract.ToIBKRContract(),
                tickType,
                0,
                false
            );

            return tickDataList;
        }

        #endregion

        #region Order Methods

        /// <summary>
        /// Place an order with IBKR.
        /// </summary>
        /// <param name="contract">A <see cref="Contract"/> for the instrument</param>
        /// <param name="order">An <see cref="Order"/> object</param>
        /// <returns>A <see cref="Trade"/> object, which receives and holds information on the order status and fills</returns>
        public Task<Trade> PlaceOrder(Contract contract, Order order)
        {
            TaskCompletionSource<Trade> tcs = new();
            int orderId;

            lock (_locks.orderIdLock)
            {
                orderId = _wrapper.NextOrderId++;
            }

            Trade trade = Trade.New(orderId, contract.ToIBKRContract(), order, null);

            // The trade subscribes to the order status, execution, and commission events for updates.
            _wrapper.OrderStatusEvent += trade.HandleOrderStatus;
            _wrapper.ExecDetailsEvent += trade.HandleExecution;
            _wrapper.CommissionEvent += trade.HandleCommission;

            var statusUpdate = Handlers.OrderStatusHandler(orderId, tcs, trade);

            _wrapper.OrderStatusEvent += statusUpdate;

            tcs.Task.ContinueWith(t => _wrapper.OrderStatusEvent -= statusUpdate);

            _wrapper.ClientSocket.placeOrder(orderId, contract.ToIBKRContract(), order);

            return tcs.Task;
        }

        /// <summary>
        /// Cancels an order placed with IBKR
        /// </summary>
        /// <param name="order">The order to be canceled</param>
        /// <remarks>
        ///     Note - this method passes the current time to IBKR as the "manualOrderCancelTime" parameter.
        /// </remarks>
        public void CancelOrder(Order order)
        {
            _wrapper.ClientSocket.cancelOrder(
                order.OrderId,
                DateTimeOffset.Now.ToString("yyyyMMdd HH:mm:ss") + " PST"
            );
        }

        /// <summary>
        /// Gets a list of open orders for the account.
        /// </summary>
        /// <returns>A list of <see cref="Trade"/> objects representing the open orders</returns>
        public Task<List<Trade>> GetOpenOrders()
        {
            lock (_locks.openOrderLock)
            {
                List<Trade> trades = [];
                TaskCompletionSource<List<Trade>> tcs = new();
                var orderHandler = Handlers.OpenOrderHandler(trades);
                var orderEndHandler = Handlers.OpenOrderEndHandler(trades, tcs);

                _wrapper.OpenOrderEvent += orderHandler;
                _wrapper.OpenOrderEndEvent += orderEndHandler;

                tcs.Task.ContinueWith(t =>
                {
                    _wrapper.OpenOrderEvent -= orderHandler;
                    _wrapper.OpenOrderEndEvent -= orderEndHandler;
                });

                _wrapper.ClientSocket.reqAllOpenOrders();

                return tcs.Task;
            }
        }

        #endregion

        #region Scanner Methods

        /// <summary>
        /// Get an XML list of all scanner parameters available in TWS.
        /// </summary>
        /// <returns>A <see cref="string"/> containing all XML-formatted parameters</returns>
        public Task<string> GetScannerParameters()
        {
            lock (_locks.scannerParamLock)
            {
                TaskCompletionSource<string> tcs = new();

                var handler = Handlers.ScannerParametersHandler(tcs);

                _wrapper.ScannerParametersEvent += handler;

                tcs.Task.ContinueWith(t =>
                {
                    _wrapper.ScannerParametersEvent -= handler;
                });

                _wrapper.ClientSocket.reqScannerParameters();

                return tcs.Task;
            }
        }

        /// <summary>
        /// Request a market scan from IBKR
        /// </summary>
        /// <param name="instrument">
        ///     <para>Instrument type to use</para>
        ///     <list type="table">
        ///         <listheader><description>Instrument Type Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="location">Country or region for scanner to search</param>
        /// <param name="scanCode">Value for scanner to sort by</param>
        /// <param name="filterOptions">Filter option key/value pairs to use in the scanner</param>
        /// <returns>A <see cref="ScannerData"/> object which receives and stores the results sent by IBKR</returns>
        /// <remarks>
        ///     For more information on how to construct scanner requests, see IBKR's documentation at <see href="https://ibkrcampus.com/ibkr-api-page/twsapi-doc/#market-scanner"/>
        /// </remarks>
        public ScannerData GetScannerData(
            string instrument,
            string location,
            string scanCode,
            Dictionary<string, string>? filterOptions
        )
        {
            int reqId;
            lock (_locks.reqIdLock)
            {
                reqId = _wrapper.ReqId++;
            }

            ScannerSubscription scannerSub = Wrapper.MakeScannerSubscription(
                instrument,
                location,
                scanCode
            );
            List<TagValue> tagValues =
                filterOptions != null ? Wrapper.MakeFilterOptions(filterOptions) : [];

            ScannerData scannerData = new(reqId, scannerSub, tagValues);
            _wrapper.ScannerDataEvent += scannerData.HandleScannerData;
            _wrapper.ScannerDataEndEvent += scannerData.HandleScannerDataEnd;

            _wrapper.ClientSocket.reqScannerSubscription(reqId, scannerSub, [], tagValues);

            return scannerData;
        }

        /// <summary>
        /// Cancels the scanner data subscription
        /// </summary>
        /// <param name="scannerData"></param>
        public void CancelScannerSubscription(ScannerData scannerData)
        {
            _wrapper.ScannerDataEvent -= scannerData.HandleScannerData;
            _wrapper.ClientSocket.cancelScannerSubscription(scannerData.ReqId);
        }

        #endregion

        /// <summary>
        /// Internal method to create a new instance of the <see cref="Broker"/> class with an external <see cref="Wrapper"/> instance for testing/mocking purposes.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns>A new <see cref="Broker"/> instance</returns>
        public static Broker CreateNew(Wrapper wrapper)
        {
            return new Broker(wrapper);
        }
    }
}
