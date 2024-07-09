using IBApi;

namespace IBKRWrapper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE1006:Naming Styles",
        Justification = "We need to use IBKR naming conventions which we don't control"
    )]
    public interface IClientSocket
    {
        public EClientSocket clientSocket { get; set; }

        void calculateImpliedVolatility(
            int reqId,
            Contract contract,
            double optionPrice,
            double underPrice,
            List<TagValue> impliedVolatilityOptions
        );
        void cancelOrder(int orderId, string manualOrderCancelTime);
        void cancelScannerSubscription(int tickerId);
        void eConnect(string host, int port, int clientId);
        void eDisconnect();
        bool IsConnected();
        void placeOrder(int id, Contract contract, Order order);
        void reqAccountUpdates(bool subscribe, string account);
        void reqAllOpenOrders();
        void reqContractDetails(int reqId, Contract contract);
        void reqHeadTimestamp(
            int tickerId,
            Contract contract,
            string whatToShow,
            int useRTH,
            int formatDate
        );
        void reqHistoricalData(
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
        );
        void reqHistoricalTicks(
            int reqId,
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            string whatToShow,
            int useRth,
            bool ignoreSize,
            List<TagValue> miscOptions
        );
        void reqIds(int numIds);
        void reqMarketDataType(int marketDataType);
        void reqMktData(
            int tickerId,
            Contract contract,
            string genericTickList,
            bool snapshot,
            bool regulatorySnaphsot,
            List<TagValue> mktDataOptions
        );
        void reqPositions();
        void reqRealTimeBars(
            int tickerId,
            Contract contract,
            int barSize,
            string whatToShow,
            bool useRTH,
            List<TagValue> realTimeBarsOptions
        );
        void reqScannerParameters();
        void reqScannerSubscription(
            int reqId,
            ScannerSubscription subscription,
            List<TagValue> scannerSubscriptionOptions,
            List<TagValue> scannerSubscriptionFilterOptions
        );
        void reqSecDefOptParams(
            int reqId,
            string underlyingSymbol,
            string futFopExchange,
            string underlyingSecType,
            int underlyingConId
        );
        void reqTickByTickData(
            int requestId,
            Contract contract,
            string tickType,
            int numberOfTicks,
            bool ignoreSize
        );
    }
}
