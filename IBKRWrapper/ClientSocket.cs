using IBApi;

namespace IBKRWrapper
{
    public class ClientSocket(EWrapper wrapper, EReaderSignal eReaderSignal) : IClientSocket
    {
        public EClientSocket clientSocket { get; set; } = new(wrapper, eReaderSignal);

        public void calculateImpliedVolatility(
            int reqId,
            Contract contract,
            double optionPrice,
            double underPrice,
            List<TagValue> impliedVolatilityOptions
        )
        {
            clientSocket.calculateImpliedVolatility(
                reqId,
                contract,
                optionPrice,
                underPrice,
                impliedVolatilityOptions
            );
        }

        public void cancelOrder(int orderId, string manualOrderCancelTime)
        {
            clientSocket.cancelOrder(orderId, manualOrderCancelTime);
        }

        public void cancelScannerSubscription(int tickerId)
        {
            clientSocket.cancelScannerSubscription(tickerId);
        }

        public void placeOrder(int id, Contract contract, Order order)
        {
            clientSocket.placeOrder(id, contract, order);
        }

        public void reqAccountUpdates(bool subscribe, string account)
        {
            clientSocket.reqAccountUpdates(subscribe, account);
        }

        public void reqAllOpenOrders()
        {
            clientSocket.reqAllOpenOrders();
        }

        public void reqContractDetails(int reqId, Contract contract)
        {
            clientSocket.reqContractDetails(reqId, contract);
        }

        public void reqHeadTimestamp(
            int tickerId,
            Contract contract,
            string whatToShow,
            int useRTH,
            int formatDate
        )
        {
            clientSocket.reqHeadTimestamp(tickerId, contract, whatToShow, useRTH, formatDate);
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
            clientSocket.reqHistoricalData(
                tickerId,
                contract,
                endDateTime,
                durationStr,
                barSizeSetting,
                whatToShow,
                useRTH,
                formatDate,
                keepUpToDate,
                chartOptions
            );
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
            clientSocket.reqHistoricalTicks(
                reqId,
                contract,
                startDateTime,
                endDateTime,
                numberOfTicks,
                whatToShow,
                useRth,
                ignoreSize,
                miscOptions
            );
        }

        public void reqMarketDataType(int marketDataType)
        {
            clientSocket.reqMarketDataType(marketDataType);
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
            clientSocket.reqMktData(
                tickerId,
                contract,
                genericTickList,
                snapshot,
                regulatorySnaphsot,
                mktDataOptions
            );
        }

        public void reqPositions()
        {
            clientSocket.reqPositions();
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
            clientSocket.reqRealTimeBars(
                tickerId,
                contract,
                barSize,
                whatToShow,
                useRTH,
                realTimeBarsOptions
            );
        }

        public void reqScannerParameters()
        {
            clientSocket.reqScannerParameters();
        }

        public void reqScannerSubscription(
            int reqId,
            ScannerSubscription subscription,
            List<TagValue> scannerSubscriptionOptions,
            List<TagValue> scannerSubscriptionFilterOptions
        )
        {
            clientSocket.reqScannerSubscription(
                reqId,
                subscription,
                scannerSubscriptionOptions,
                scannerSubscriptionFilterOptions
            );
        }

        public void reqSecDefOptParams(
            int reqId,
            string underlyingSymbol,
            string futFopExchange,
            string underlyingSecType,
            int underlyingConId
        )
        {
            clientSocket.reqSecDefOptParams(
                reqId,
                underlyingSymbol,
                futFopExchange,
                underlyingSecType,
                underlyingConId
            );
        }

        public void reqTickByTickData(
            int reqId,
            Contract contract,
            string tickType,
            int numberOfTicks,
            bool ignoreSize
        )
        {
            clientSocket.reqTickByTickData(reqId, contract, tickType, numberOfTicks, ignoreSize);
        }

        public void eConnect(string host, int port, int clientId)
        {
            clientSocket.eConnect(host, port, clientId);
        }

        public void eDisconnect()
        {
            clientSocket.eDisconnect();
        }

        public bool IsConnected()
        {
            return clientSocket.IsConnected();
        }
    }
}
