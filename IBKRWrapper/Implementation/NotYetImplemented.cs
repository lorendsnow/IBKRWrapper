using System.Text;
using IBApi;

namespace IBKRWrapper
{
    /// <summary>
    /// EWrapperImpl is a class that implements the EWrapper interface from the IBKR API.
    /// </summary>
    public partial class Wrapper : EWrapper
    {
        /// <summary>
        /// The BBO Exchange, as received from the TWS server in tickReqParams.
        /// </summary>
        public string BboExchange { get; private set; } = string.Empty;

        public virtual void connectionClosed()
        {
            Console.WriteLine("Connection closed.\n");
        }

        public virtual void currentTime(long time)
        {
            Console.WriteLine("Current Time: " + time + "\n");
        }

        //! [tickprice]
        public virtual void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            Console.WriteLine(
                "Tick Price. Ticker Id:"
                    + tickerId
                    + ", Field: "
                    + field
                    + ", Price: "
                    + Util.DoubleMaxString(price)
                    + ", CanAutoExecute: "
                    + attribs.CanAutoExecute
                    + ", PastLimit: "
                    + attribs.PastLimit
                    + ", PreOpen: "
                    + attribs.PreOpen
            );
        }

        //! [tickprice]

        //! [ticksize]
        public virtual void tickSize(int tickerId, int field, decimal size)
        {
            Console.WriteLine(
                "Tick Size. Ticker Id:"
                    + tickerId
                    + ", Field: "
                    + field
                    + ", Size: "
                    + Util.DecimalMaxString(size)
            );
        }

        //! [ticksize]

        //! [tickstring]
        public virtual void tickString(int tickerId, int tickType, string value)
        {
            Console.WriteLine(
                "Tick string. Ticker Id:" + tickerId + ", Type: " + tickType + ", Value: " + value
            );
        }

        //! [tickstring]

        //! [tickgeneric]
        public virtual void tickGeneric(int tickerId, int field, double value)
        {
            Console.WriteLine(
                "Tick Generic. Ticker Id:"
                    + tickerId
                    + ", Field: "
                    + field
                    + ", Value: "
                    + Util.DoubleMaxString(value)
            );
        }

        //! [tickgeneric]

        public virtual void tickEFP(
            int tickerId,
            int tickType,
            double basisPoints,
            string formattedBasisPoints,
            double impliedFuture,
            int holdDays,
            string futureLastTradeDate,
            double dividendImpact,
            double dividendsToLastTradeDate
        )
        {
            Console.WriteLine(
                "TickEFP. "
                    + tickerId
                    + ", Type: "
                    + tickType
                    + ", BasisPoints: "
                    + Util.DoubleMaxString(basisPoints)
                    + ", FormattedBasisPoints: "
                    + formattedBasisPoints
                    + ", ImpliedFuture: "
                    + Util.DoubleMaxString(impliedFuture)
                    + ", HoldDays: "
                    + Util.IntMaxString(holdDays)
                    + ", FutureLastTradeDate: "
                    + futureLastTradeDate
                    + ", DividendImpact: "
                    + Util.DoubleMaxString(dividendImpact)
                    + ", DividendsToLastTradeDate: "
                    + Util.DoubleMaxString(dividendsToLastTradeDate)
            );
        }

        //! [ticksnapshotend]
        public virtual void tickSnapshotEnd(int tickerId)
        {
            Console.WriteLine("TickSnapshotEnd: " + tickerId);
        }

        //! [ticksnapshotend]

        //! [deltaneutralvalidation]
        public virtual void deltaNeutralValidation(
            int reqId,
            DeltaNeutralContract deltaNeutralContract
        )
        {
            Console.WriteLine(
                "DeltaNeutralValidation. "
                    + reqId
                    + ", ConId: "
                    + deltaNeutralContract.ConId
                    + ", Delta: "
                    + Util.DoubleMaxString(deltaNeutralContract.Delta)
                    + ", Price: "
                    + Util.DoubleMaxString(deltaNeutralContract.Price)
            );
        }

        //! [deltaneutralvalidation]

        //! [managedaccounts]
        public virtual void managedAccounts(string accountsList)
        {
            Console.WriteLine("Account list: " + accountsList);
        }

        //! [managedaccounts]

        //! [tickoptioncomputation]
        public virtual void tickOptionComputation(
            int tickerId,
            int field,
            int tickAttrib,
            double impliedVolatility,
            double delta,
            double optPrice,
            double pvDividend,
            double gamma,
            double vega,
            double theta,
            double undPrice
        )
        {
            Console.WriteLine(
                "TickOptionComputation. TickerId: "
                    + tickerId
                    + ", field: "
                    + field
                    + ", TickAttrib: "
                    + Util.IntMaxString(tickAttrib)
                    + ", ImpliedVolatility: "
                    + Util.DoubleMaxString(impliedVolatility)
                    + ", Delta: "
                    + Util.DoubleMaxString(delta)
                    + ", OptionPrice: "
                    + Util.DoubleMaxString(optPrice)
                    + ", pvDividend: "
                    + Util.DoubleMaxString(pvDividend)
                    + ", Gamma: "
                    + Util.DoubleMaxString(gamma)
                    + ", Vega: "
                    + Util.DoubleMaxString(vega)
                    + ", Theta: "
                    + Util.DoubleMaxString(theta)
                    + ", UnderlyingPrice: "
                    + Util.DoubleMaxString(undPrice)
            );
        }

        //! [tickoptioncomputation]

        //! [accountsummary]
        public virtual void accountSummary(
            int reqId,
            string account,
            string tag,
            string value,
            string currency
        )
        {
            Console.WriteLine(
                "Acct Summary. ReqId: "
                    + reqId
                    + ", Acct: "
                    + account
                    + ", Tag: "
                    + tag
                    + ", Value: "
                    + value
                    + ", Currency: "
                    + currency
            );
        }

        //! [accountsummary]

        //! [accountsummaryend]
        public virtual void accountSummaryEnd(int reqId)
        {
            Console.WriteLine("AccountSummaryEnd. Req Id: " + reqId + "\n");
        }

        //! [accountsummaryend]

        //! [fundamentaldata]
        public virtual void fundamentalData(int reqId, string data)
        {
            Console.WriteLine("FundamentalData. " + reqId + "" + data + "\n");
        }

        //! [fundamentaldata]

        //! [marketdatatype]
        public virtual void marketDataType(int reqId, int marketDataType)
        {
            Console.WriteLine("MarketDataType. " + reqId + ", Type: " + marketDataType + "\n");
        }

        //! [marketdatatype]

        //! [updatemktdepth]
        public virtual void updateMktDepth(
            int tickerId,
            int position,
            int operation,
            int side,
            double price,
            decimal size
        )
        {
            Console.WriteLine(
                "UpdateMarketDepth. "
                    + tickerId
                    + " - Position: "
                    + position
                    + ", Operation: "
                    + operation
                    + ", Side: "
                    + side
                    + ", Price: "
                    + Util.DoubleMaxString(price)
                    + ", Size: "
                    + Util.DecimalMaxString(size)
            );
        }

        //! [updatemktdepth]

        //! [updatemktdepthl2]
        public virtual void updateMktDepthL2(
            int tickerId,
            int position,
            string marketMaker,
            int operation,
            int side,
            double price,
            decimal size,
            bool isSmartDepth
        )
        {
            Console.WriteLine(
                "UpdateMarketDepthL2. "
                    + tickerId
                    + " - Position: "
                    + position
                    + ", Operation: "
                    + operation
                    + ", Side: "
                    + side
                    + ", Price: "
                    + Util.DoubleMaxString(price)
                    + ", Size: "
                    + Util.DecimalMaxString(size)
                    + ", isSmartDepth: "
                    + isSmartDepth
            );
        }

        //! [updatemktdepthl2]

        //! [updatenewsbulletin]
        public virtual void updateNewsBulletin(
            int msgId,
            int msgType,
            String message,
            String origExchange
        )
        {
            Console.WriteLine(
                "News Bulletins. "
                    + msgId
                    + " - Type: "
                    + msgType
                    + ", Message: "
                    + message
                    + ", Exchange of Origin: "
                    + origExchange
                    + "\n"
            );
        }

        //! [updatenewsbulletin]

        //! [receivefa]
        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            Console.WriteLine("Receing FA: " + faDataType + " - " + faXmlData);
        }

        //! [receivefa]

        public virtual void bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            Console.WriteLine("BondContractDetails begin. ReqId: " + requestId);
            Console.WriteLine("BondContractDetails end. ReqId: " + requestId);
        }

        public virtual void verifyMessageAPI(string apiData)
        {
            Console.WriteLine("verifyMessageAPI: " + apiData);
        }

        public virtual void verifyCompleted(bool isSuccessful, string errorText)
        {
            Console.WriteLine(
                "verifyCompleted. IsSuccessfule: " + isSuccessful + " - Error: " + errorText
            );
        }

        public virtual void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            Console.WriteLine("verifyAndAuthMessageAPI: " + apiData + " " + xyzChallenge);
        }

        public virtual void verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            Console.WriteLine(
                "verifyAndAuthCompleted. IsSuccessful: " + isSuccessful + " - Error: " + errorText
            );
        }

        //! [displaygrouplist]
        public virtual void displayGroupList(int reqId, string groups)
        {
            Console.WriteLine("DisplayGroupList. Request: " + reqId + ", Groups" + groups);
        }

        //! [displaygrouplist]

        //! [displaygroupupdated]
        public virtual void displayGroupUpdated(int reqId, string contractInfo)
        {
            Console.WriteLine(
                "displayGroupUpdated. Request: " + reqId + ", ContractInfo: " + contractInfo
            );
        }

        //! [displaygroupupdated]

        //! [positionmulti]
        public virtual void positionMulti(
            int reqId,
            string account,
            string modelCode,
            Contract contract,
            decimal pos,
            double avgCost
        )
        {
            Console.WriteLine(
                "Position Multi. Request: "
                    + reqId
                    + ", Account: "
                    + account
                    + ", ModelCode: "
                    + modelCode
                    + ", Symbol: "
                    + contract.Symbol
                    + ", SecType: "
                    + contract.SecType
                    + ", Currency: "
                    + contract.Currency
                    + ", Position: "
                    + Util.DecimalMaxString(pos)
                    + ", Avg cost: "
                    + Util.DoubleMaxString(avgCost)
                    + "\n"
            );
        }

        //! [positionmulti]

        //! [positionmultiend]
        public virtual void positionMultiEnd(int reqId)
        {
            Console.WriteLine("Position Multi End. Request: " + reqId + "\n");
        }

        //! [positionmultiend]

        //! [accountupdatemulti]
        public virtual void accountUpdateMulti(
            int reqId,
            string account,
            string modelCode,
            string key,
            string value,
            string currency
        )
        {
            Console.WriteLine(
                "Account Update Multi. Request: "
                    + reqId
                    + ", Account: "
                    + account
                    + ", ModelCode: "
                    + modelCode
                    + ", Key: "
                    + key
                    + ", Value: "
                    + value
                    + ", Currency: "
                    + currency
                    + "\n"
            );
        }

        //! [accountupdatemulti]

        //! [accountupdatemultiend]
        public virtual void accountUpdateMultiEnd(int reqId)
        {
            Console.WriteLine("Account Update Multi End. Request: " + reqId + "\n");
        }

        //! [accountupdatemultiend]

        //! [connectack]
        public void connectAck()
        {
            if (ClientSocket.AsyncEConnect)
                ClientSocket.startApi();
        }

        //! [connectack]

        //! [softDollarTiers]
        public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            Console.WriteLine("Soft Dollar Tiers:");

            foreach (var tier in tiers)
            {
                Console.WriteLine(tier.DisplayName);
            }
        }

        //! [softDollarTiers]

        //! [familyCodes]
        public void familyCodes(FamilyCode[] familyCodes)
        {
            Console.WriteLine("Family Codes:");

            foreach (var familyCode in familyCodes)
            {
                Console.WriteLine(
                    "Account ID: {0}, Family Code Str: {1}",
                    familyCode.AccountID,
                    familyCode.FamilyCodeStr
                );
            }
        }

        //! [familyCodes]

        //! [symbolSamples]
        public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            string derivSecTypes;
            Console.WriteLine("Symbol Samples. Request Id: {0}", reqId);

            foreach (var contractDescription in contractDescriptions)
            {
                derivSecTypes = "";
                foreach (var derivSecType in contractDescription.DerivativeSecTypes)
                {
                    derivSecTypes += derivSecType;
                    derivSecTypes += " ";
                }
                Console.WriteLine(
                    "Contract: conId - {0}, symbol - {1}, secType - {2}, primExchange - {3}, currency - {4}, derivativeSecTypes - {5}, description - {6}, issuerId - {7}",
                    contractDescription.Contract.ConId,
                    contractDescription.Contract.Symbol,
                    contractDescription.Contract.SecType,
                    contractDescription.Contract.PrimaryExch,
                    contractDescription.Contract.Currency,
                    derivSecTypes,
                    contractDescription.Contract.Description,
                    contractDescription.Contract.IssuerId
                );
            }
        }

        //! [symbolSamples]

        //! [mktDepthExchanges]
        public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            Console.WriteLine("Market Depth Exchanges:");

            foreach (var depthMktDataDescription in depthMktDataDescriptions)
            {
                Console.WriteLine(
                    "Depth Market Data Description: Exchange: {0}, Security Type: {1}, Listing Exch: {2}, Service Data Type: {3}, Agg Group: {4}",
                    depthMktDataDescription.Exchange,
                    depthMktDataDescription.SecType,
                    depthMktDataDescription.ListingExch,
                    depthMktDataDescription.ServiceDataType,
                    Util.IntMaxString(depthMktDataDescription.AggGroup)
                );
            }
        }

        //! [mktDepthExchanges]

        //! [tickNews]
        public void tickNews(
            int tickerId,
            long timeStamp,
            string providerCode,
            string articleId,
            string headline,
            string extraData
        )
        {
            Console.WriteLine(
                "Tick News. Ticker Id: {0}, Time Stamp: {1}, Provider Code: {2}, Article Id: {3}, headline: {4}, extraData: {5}",
                tickerId,
                Util.LongMaxString(timeStamp),
                providerCode,
                articleId,
                headline,
                extraData
            );
        }

        //! [tickNews]

        //! [smartcomponents]
        public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            StringBuilder sb = new();

            sb.AppendFormat(
                "==== Smart Components Begin (total={0}) reqId = {1} ====\n",
                theMap.Count,
                reqId
            );

            foreach (var item in theMap)
            {
                sb.AppendFormat(
                    "bit number: {0}, exchange: {1}, exchange letter: {2}\n",
                    item.Key,
                    item.Value.Key,
                    item.Value.Value
                );
            }

            sb.AppendFormat(
                "==== Smart Components Begin (total={0}) reqId = {1} ====\n",
                theMap.Count,
                reqId
            );

            Console.WriteLine(sb);
        }

        //! [smartcomponents]

        //! [tickReqParams]
        /// <summary>
        /// Tick with BOO exchange and snapshot permissions.
        /// </summary>
        /// <param name="tickerId"></param>
        /// <param name="minTick"></param>
        /// <param name="bboExchange"></param>
        /// <param name="snapshotPermissions"></param>
        public void tickReqParams(
            int tickerId,
            double minTick,
            string bboExchange,
            int snapshotPermissions
        )
        {
            Console.WriteLine(
                "id={0} minTick = {1} bboExchange = {2} snapshotPermissions = {3}",
                tickerId,
                Util.DoubleMaxString(minTick),
                bboExchange,
                Util.IntMaxString(snapshotPermissions)
            );

            BboExchange = bboExchange;
        }

        //! [tickReqParams]

        //! [newsProviders]
        public void newsProviders(NewsProvider[] newsProviders)
        {
            Console.WriteLine("News Providers:");

            foreach (var newsProvider in newsProviders)
            {
                Console.WriteLine(
                    "News provider: providerCode - {0}, providerName - {1}",
                    newsProvider.ProviderCode,
                    newsProvider.ProviderName
                );
            }
        }

        //! [newsProviders]

        //! [newsArticle]
        public void newsArticle(int requestId, int articleType, string articleText)
        {
            Console.WriteLine(
                "News Article. Request Id: {0}, ArticleType: {1}",
                requestId,
                articleType
            );
            if (articleType == 0)
            {
                Console.WriteLine("News Article Text: {0}", articleText);
            }
            else if (articleType == 1)
            {
                Console.WriteLine(
                    "News Article Text: article text is binary/pdf and cannot be displayed"
                );
            }
        }

        //! [newsArticle]

        //! [historicalNews]
        public void historicalNews(
            int requestId,
            string time,
            string providerCode,
            string articleId,
            string headline
        )
        {
            Console.WriteLine(
                "Historical News. Request Id: {0}, Time: {1}, Provider Code: {2}, Article Id: {3}, headline: {4}",
                requestId,
                time,
                providerCode,
                articleId,
                headline
            );
        }

        //! [historicalNews]

        //! [historicalNewsEnd]
        public void historicalNewsEnd(int requestId, bool hasMore)
        {
            Console.WriteLine(
                "Historical News End. Request Id: {0}, Has More: {1}",
                requestId,
                hasMore
            );
        }

        //! [historicalNewsEnd]

        //! [histogramData]
        public void histogramData(int reqId, HistogramEntry[] data)
        {
            Console.WriteLine(
                "Histogram data. Request Id: {0}, data size: {1}",
                reqId,
                data.Length
            );
            data.ToList()
                .ForEach(i =>
                    Console.WriteLine(
                        "\tPrice: {0}, Size: {1}",
                        Util.DoubleMaxString(i.Price),
                        Util.DecimalMaxString(i.Size)
                    )
                );
        }

        //! [histogramData]

        //! [historicalDataUpdate]
        public void historicalDataUpdate(int reqId, Bar bar)
        {
            Console.WriteLine(
                "HistoricalDataUpdate. "
                    + reqId
                    + " - Time: "
                    + bar.Time
                    + ", Open: "
                    + Util.DoubleMaxString(bar.Open)
                    + ", High: "
                    + Util.DoubleMaxString(bar.High)
                    + ", Low: "
                    + Util.DoubleMaxString(bar.Low)
                    + ", Close: "
                    + Util.DoubleMaxString(bar.Close)
                    + ", Volume: "
                    + Util.DecimalMaxString(bar.Volume)
                    + ", Count: "
                    + Util.IntMaxString(bar.Count)
                    + ", WAP: "
                    + Util.DecimalMaxString(bar.WAP)
            );
        }

        //! [historicalDataUpdate]

        //! [rerouteMktDataReq]
        public void rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            Console.WriteLine(
                "Re-route market data request. Req Id: {0}, ConId: {1}, Exchange: {2}",
                reqId,
                conId,
                exchange
            );
        }

        //! [rerouteMktDataReq]

        //! [rerouteMktDepthReq]
        public void rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            Console.WriteLine(
                "Re-route market depth request. Req Id: {0}, ConId: {1}, Exchange: {2}",
                reqId,
                conId,
                exchange
            );
        }

        //! [rerouteMktDepthReq]

        //! [marketRule]
        public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            Console.WriteLine("Market Rule Id: " + marketRuleId);
            foreach (var priceIncrement in priceIncrements)
            {
                Console.WriteLine(
                    "Low Edge: {0}, Increment: {1}",
                    Util.DoubleMaxString(priceIncrement.LowEdge),
                    Util.DoubleMaxString(priceIncrement.Increment)
                );
            }
        }

        //! [marketRule]

        //! [pnl]
        public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            Console.WriteLine(
                "PnL. Request Id: {0}, Daily PnL: {1}, Unrealized PnL: {2}, Realized PnL: {3}",
                reqId,
                Util.DoubleMaxString(dailyPnL),
                Util.DoubleMaxString(unrealizedPnL),
                Util.DoubleMaxString(realizedPnL)
            );
        }

        //! [pnl]

        //! [pnlsingle]
        public void pnlSingle(
            int reqId,
            decimal pos,
            double dailyPnL,
            double unrealizedPnL,
            double realizedPnL,
            double value
        )
        {
            Console.WriteLine(
                "PnL Single. Request Id: {0}, Pos {1}, Daily PnL {2}, Unrealized PnL {3}, Realized PnL: {4}, Value: {5}",
                reqId,
                Util.DecimalMaxString(pos),
                Util.DoubleMaxString(dailyPnL),
                Util.DoubleMaxString(unrealizedPnL),
                Util.DoubleMaxString(realizedPnL),
                Util.DoubleMaxString(value)
            );
        }

        //! [pnlsingle]

        //! [orderbound]
        public void orderBound(long orderId, int apiClientId, int apiOrderId)
        {
            Console.WriteLine(
                "Order bound. Order Id: {0}, Api Client Id: {1}, Api Order Id: {2}",
                Util.LongMaxString(orderId),
                Util.IntMaxString(apiClientId),
                Util.IntMaxString(apiOrderId)
            );
        }

        //! [orderbound]

        //! [completedorder]
        public virtual void completedOrder(Contract contract, Order order, OrderState orderState)
        {
            Console.WriteLine(
                "CompletedOrder. PermID: "
                    + Util.IntMaxString(order.PermId)
                    + ", ParentPermId: "
                    + Util.LongMaxString(order.ParentPermId)
                    + ", Account: "
                    + order.Account
                    + ", Symbol: "
                    + contract.Symbol
                    + ", SecType: "
                    + contract.SecType
                    + " , Exchange: "
                    + contract.Exchange
                    + ", Action: "
                    + order.Action
                    + ", OrderType: "
                    + order.OrderType
                    + ", TotalQty: "
                    + Util.DecimalMaxString(order.TotalQuantity)
                    + ", CashQty: "
                    + Util.DoubleMaxString(order.CashQty)
                    + ", FilledQty: "
                    + Util.DecimalMaxString(order.FilledQuantity)
                    + ", LmtPrice: "
                    + Util.DoubleMaxString(order.LmtPrice)
                    + ", AuxPrice: "
                    + Util.DoubleMaxString(order.AuxPrice)
                    + ", Status: "
                    + orderState.Status
                    + ", CompletedTime: "
                    + orderState.CompletedTime
                    + ", CompletedStatus: "
                    + orderState.CompletedStatus
                    + ", MinTradeQty: "
                    + Util.IntMaxString(order.MinTradeQty)
                    + ", MinCompeteSize: "
                    + Util.IntMaxString(order.MinCompeteSize)
                    + ", CompeteAgainstBestOffset: "
                    + (
                        order.CompeteAgainstBestOffset
                        == Order.COMPETE_AGAINST_BEST_OFFSET_UP_TO_MID
                            ? "UpToMid"
                            : Util.DoubleMaxString(order.CompeteAgainstBestOffset)
                    )
                    + ", MidOffsetAtWhole: "
                    + Util.DoubleMaxString(order.MidOffsetAtWhole)
                    + ", MidOffsetAtHalf: "
                    + Util.DoubleMaxString(order.MidOffsetAtHalf)
            );
        }

        //! [completedorder]

        //! [completedordersend]
        public virtual void completedOrdersEnd()
        {
            Console.WriteLine("CompletedOrdersEnd");
        }

        //! [completedordersend]

        //! [replacefaend]
        public virtual void replaceFAEnd(int reqId, string text)
        {
            Console.WriteLine("Replace FA End. ReqId: " + reqId + ", Text: " + text + "\n");
        }

        //! [replacefaend]

        //! [wshMetaData]
        public void wshMetaData(int reqId, string dataJson)
        {
            Console.WriteLine($"WSH Meta Data. Request Id: {reqId}, Data JSON: {dataJson}\n");
        }

        //! [wshMetaData]

        //! [wshEventData]
        public void wshEventData(int reqId, string dataJson)
        {
            Console.WriteLine($"WSH Event Data. Request Id: {reqId}, Data JSON: {dataJson}\n");
        }

        //! [wshEventData]

        //! [historicalSchedule]
        public void historicalSchedule(
            int reqId,
            string startDateTime,
            string endDateTime,
            string timeZone,
            HistoricalSession[] sessions
        )
        {
            Console.WriteLine(
                $"Historical Schedule. ReqId: {reqId}, Start: {startDateTime}, End: {endDateTime}, Time Zone: {timeZone}"
            );

            foreach (var session in sessions)
            {
                Console.WriteLine(
                    $"\tSession. Start: {session.StartDateTime}, End: {session.EndDateTime}, Ref Date: {session.RefDate}"
                );
            }
        }

        //! [historicalSchedule]

        //! [userInfo]
        public void userInfo(int reqId, string whiteBrandingId)
        {
            Console.WriteLine($"User Info. ReqId: {reqId}, WhiteBrandingId: {whiteBrandingId}");
        }
        //! [userInfo]
    }
}
