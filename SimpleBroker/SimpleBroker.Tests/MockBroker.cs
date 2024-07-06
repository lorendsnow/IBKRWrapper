using IBApi;
using IBKRWrapper;

namespace SimpleBroker.Tests
{
    public class MockClient(EWrapper wrapper, EReaderSignal eReaderSignal) : IClientSocket
    {
        public EClientSocket clientSocket { get; set; } = new(wrapper, eReaderSignal);

        public static Contract AAPLContract
        {
            get =>
                new()
                {
                    ConId = 1,
                    Symbol = "AAPL",
                    SecType = "STK",
                    Exchange = "SMART",
                    PrimaryExch = "SMART",
                    Currency = "USD"
                };
        }

        public static List<Contract> TSLAOptContracts
        {
            get =>
                [
                    new()
                    {
                        ConId = 4,
                        Symbol = "TSLA",
                        SecType = "OPT",
                        Exchange = "SMART",
                        PrimaryExch = "SMART",
                        Currency = "USD",
                        Strike = 90,
                        Right = "P",
                        LastTradeDateOrContractMonth = "20240101",
                    },
                    new()
                    {
                        ConId = 5,
                        Symbol = "TSLA",
                        SecType = "OPT",
                        Exchange = "SMART",
                        PrimaryExch = "SMART",
                        Currency = "USD",
                        Strike = 100,
                        Right = "P",
                        LastTradeDateOrContractMonth = "20240101",
                    },
                ];
        }

        public static Contract TSLAContract
        {
            get =>
                new()
                {
                    ConId = 2,
                    Symbol = "TSLA",
                    SecType = "STK",
                    Exchange = "SMART",
                    PrimaryExch = "SMART",
                    Currency = "USD"
                };
        }

        public static Contract MSFTContract
        {
            get =>
                new()
                {
                    ConId = 3,
                    Symbol = "MSFT",
                    SecType = "STK",
                    Exchange = "SMART",
                    PrimaryExch = "SMART",
                    Currency = "USD"
                };
        }

        public void calculateImpliedVolatility(
            int reqId,
            Contract contract,
            double optionPrice,
            double underPrice,
            List<TagValue> impliedVolatilityOptions
        )
        {
            return;
        }

        public void cancelOrder(int orderId, string manualOrderCancelTime)
        {
            return;
        }

        public void cancelScannerSubscription(int tickerId)
        {
            return;
        }

        public void eConnect(string host, int port, int clientId)
        {
            return;
        }

        public void eDisconnect()
        {
            return;
        }

        public bool IsConnected()
        {
            return true;
        }

        public void placeOrder(int id, Contract contract, Order order)
        {
            wrapper.orderStatus(id, "PendingSubmit", 0m, 100m, 0, 1, 1, 0, 0, "", 100);
        }

        public async void reqAccountUpdates(bool subscribe, string account)
        {
            if (!subscribe)
            {
                return;
            }

            await SimulatePortfolioPositions();
            await SimulateAccountValues();

            wrapper.accountDownloadEnd("123456");
        }

        public void reqAllOpenOrders()
        {
            wrapper.openOrder(
                1,
                MSFTContract,
                new()
                {
                    Account = "123456",
                    Action = "BUY",
                    OrderType = "LMT",
                    TotalQuantity = 100,
                    LmtPrice = 100
                },
                new() { Status = "PendingSubmit" }
            );
            wrapper.openOrder(
                2,
                AAPLContract,
                new()
                {
                    Account = "123456",
                    Action = "BUY",
                    OrderType = "LMT",
                    TotalQuantity = 100,
                    LmtPrice = 100
                },
                new() { Status = "PendingSubmit" }
            );
            wrapper.openOrderEnd();
        }

        public void reqContractDetails(int reqId, Contract contract)
        {
            switch (contract.Symbol)
            {
                case "AAPL":
                    // Return a stock contract.
                    wrapper.contractDetails(
                        reqId,
                        new(
                            AAPLContract,
                            "Apple",
                            0.01,
                            "ALL",
                            "ALL",
                            0,
                            "Apple Stock",
                            "",
                            "Tech",
                            "Tech",
                            "Tech",
                            "New York",
                            "9:30-4:00",
                            "9:30-4:00",
                            "None",
                            1,
                            1
                        )
                    );
                    break;
                case "TSLA":
                    // Return options contracts.
                    wrapper.contractDetails(
                        reqId,
                        new(
                            TSLAOptContracts[0],
                            "Tesla",
                            0.01,
                            "ALL",
                            "ALL",
                            0,
                            "Tesla options",
                            "",
                            "Tech",
                            "Tech",
                            "Tech",
                            "New York",
                            "9:30-4:00",
                            "9:30-4:00",
                            "None",
                            1,
                            1
                        )
                    );
                    wrapper.contractDetails(
                        reqId,
                        new(
                            TSLAOptContracts[1],
                            "Tesla",
                            0.01,
                            "ALL",
                            "ALL",
                            0,
                            "Tesla options",
                            "",
                            "Tech",
                            "Tech",
                            "Tech",
                            "New York",
                            "9:30-4:00",
                            "9:30-4:00",
                            "None",
                            1,
                            1
                        )
                    );
                    break;
                default:
                    break;
            }

            Thread.Sleep(100);
            wrapper.contractDetailsEnd(reqId);
        }

        public void reqHeadTimestamp(
            int reqId,
            Contract contract,
            string whatToShow,
            int useRTH,
            int formatDate
        )
        {
            wrapper.headTimestamp(reqId, "20240101 09:30:00 America/New_York");
        }

        public void reqHistoricalData(
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
            wrapper.historicalData(
                tickerId,
                new("20240101", 99, 100, 98, 99.50, 1000m, 1000, 99.50m)
            );
            wrapper.historicalData(
                tickerId,
                new("20240102", 98, 101, 99, 101, 1000m, 1000, 99.50m)
            );
            wrapper.historicalData(
                tickerId,
                new("20240103", 99.5, 105, 95, 99.50, 1000m, 1000, 99.50m)
            );
            wrapper.historicalDataEnd(tickerId, "20240101", "20240103");
        }

        public void reqHistoricalTicks(
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
            switch (whatToShow)
            {
                case "TRADES":
                    wrapper.historicalTicksLast(
                        reqId,
                        [
                            new(1234567, new(), 100, 1000m, "SMART", ""),
                            new(1234568, new(), 101, 1000m, "SMART", ""),
                            new(1234569, new(), 102, 1000m, "SMART", "")
                        ],
                        true
                    );
                    break;
                case "BID_ASK":
                    wrapper.historicalTicksBidAsk(
                        reqId,
                        [
                            new(1234567, new(), 100, 101, 1000m, 1000m),
                            new(1234568, new(), 101, 102, 1000m, 1000m),
                            new(1234569, new(), 102, 103, 1000m, 1000m)
                        ],
                        true
                    );
                    break;
                case "MIDPOINT":
                    wrapper.historicalTicks(
                        reqId,
                        [
                            new(12345678, 100, 1000m),
                            new(1234568, 101, 1000m),
                            new(1234569, 102, 1000m)
                        ],
                        true
                    );
                    break;
                default:
                    break;
            }
        }

        public void reqMarketDataType(int marketDataType)
        {
            return;
        }

        public void reqMktData(
            int tickerId,
            Contract contract,
            string genericTickList,
            bool snapshot,
            bool regulatorySnaphsot,
            List<TagValue> mktDataOptions
        )
        {
            return;
        }

        public void reqPositions()
        {
            wrapper.position("1234567", MSFTContract, 100m, 100);
            wrapper.position("1234567", AAPLContract, 100m, 100);
            wrapper.positionEnd();
        }

        public void reqRealTimeBars(
            int tickerId,
            Contract contract,
            int barSize,
            string whatToShow,
            bool useRTH,
            List<TagValue> realTimeBarsOptions
        )
        {
            return;
        }

        public void reqScannerParameters()
        {
            wrapper.scannerParameters("test string");
        }

        public void reqScannerSubscription(
            int reqId,
            ScannerSubscription subscription,
            List<TagValue> scannerSubscriptionOptions,
            List<TagValue> scannerSubscriptionFilterOptions
        )
        {
            return;
        }

        public void reqSecDefOptParams(
            int reqId,
            string underlyingSymbol,
            string futFopExchange,
            string underlyingSecType,
            int underlyingConId
        )
        {
            switch (underlyingSymbol)
            {
                case "AAPL":
                    wrapper.securityDefinitionOptionParameter(
                        reqId,
                        futFopExchange,
                        underlyingConId,
                        "",
                        "100",
                        new(["20240101", "20240201", "20240301"]),
                        new([5, 10, 15, 20, 25])
                    );
                    break;
                default:
                    break;
            }

            Thread.Sleep(100);
            wrapper.securityDefinitionOptionParameterEnd(reqId);
        }

        public void reqTickByTickData(
            int requestId,
            Contract contract,
            string tickType,
            int numberOfTicks,
            bool ignoreSize
        )
        {
            return;
        }

        public async Task SimulatePortfolioPositions()
        {
            wrapper.updatePortfolio(AAPLContract, 100m, 10, 1000, 9, 100, 0, "123456");
            await Task.Delay(100);
            wrapper.updatePortfolio(TSLAContract, -100m, 10, -1000, 9, -100, 0, "123456");
            await Task.Delay(100);
            wrapper.updatePortfolio(MSFTContract, 100m, 10, 1000, 9, 100, 0, "123456");
            await Task.Delay(100);
        }

        public async Task SimulateAccountValues()
        {
            wrapper.updateAccountValue("netLiq", "100000.54", "USD", "123456");
            await Task.Delay(100);
            wrapper.updateAccountValue("margin", "100000.54", "USD", "123456");
            await Task.Delay(100);
            wrapper.updateAccountValue("cash", "987.54", "USD", "123456");
            await Task.Delay(100);
        }
    }
}
