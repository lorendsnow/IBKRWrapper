namespace IBApi.Interfaces
{
    public interface IClient
    {
        public IWrapper Wrapper { get; }
        public bool AllowRedirect { get; set; }
        public int ServerVersion { get; }
        public string ServerTime { get; }
        public string OptionalCapabilities { get; set; }
        public bool AsyncEConnect { get; set; }

        public void SetConnectOptions(string connectOptions);
        public void SetOptionalCapabilities(string optionalCapabilities);
        public void DisableUseV100Plus();
        public bool IsConnected();
        public bool UseProtoBuf(int serverVersion, OutgoingMessages outgoingMessage);
        public void StartApi();
        public void Close();
        public void Disconnect(bool resetState);
        public void ReqCompleteOrders(bool apiOnly);
        public void CancelTickByTickData(int requestId);
        public void ReqTickByTickData(int requestId, Contract contract, string tickType, int numberOfTicks, bool ignoreSize);
        public void CancelHistoricalData(int reqId);
        public void CalculateImpliedVolatility(int reqId, Contract contract, double optionPrice, double underPrice, List<TagValue> impliedVolatilityOptions);
        public void CalculateOptionPrice(int reqId, Contract contract, double volatility, double underPrice, List<TagValue> optionPriceOptions);
        public void CancelAccountSummary(int reqId);
        public void CancelCalculateImpliedVolatility(int reqId);
        public void CancelCalculateOptionPrice(int reqId);
        public void CancelFundamentalData(int reqId);
        public void CancelMktData(int tickerId);
        public void CancelMktDepth(int tickerId, bool isSmartDepth);
        public void CancelNewsBulletin();
        public void CancelOrder(int orderId, OrderCancel orderCancel);
        public void CancelPositions();
        public void CancelRealTimeBars(int tickerId);
        public void CancelScannerSubscription(int tickerId);
        public void ExerciseOptions(int tickerId, Contract contract, int exerciseAction, int exerciseQuantity, string account, int ovrd, string manualOrderTime, string customerAccount, bool professionalCustomer);
        public void PlaceOrder(int id, Contract contract, Order order);
        public void ReplaceFA(int reqId, int faDataType, string xml);
        public void RequestFA(int faDataType);
        public void ReqAccountSummary(int reqId, string group, string tags);
        public void ReqAccountUpdates(bool subscribe, string acctCode);
        public void ReqAllOpenOrders();
        public void ReqAutoOpenOrders(bool autoBind);
        public void ReqContractDetails(int reqId, Contract contract);
        public void ReqCurrentTime();
        public void ReqExecutions(int reqId, ExecutionFilter filter);
        public void ReqExecutionsNonProtoBuf(int reqId, ExecutionFilter filter);
        public void ReqExecutionsProtoBuf(int reqId, ExecutionFilter filter);
        public void ReqFundamentalData(int reqId, Contract contract, string reportType, List<TagValue> fundamentalDataOptions);
        public void ReqGlobalCancel(OrderCancel orderCancel);
        public void ReqHistoricalData(int tickerId, Contract contract, string endDateTime, string durationStr, string barSizeSetting, string whatToShow, int useRTH, int formatDate, bool keepUpToDate, List<TagValue> chartOptions);
        public void ReqIds(int numIds);
        public void ReqManagedAccts();
        public void ReqMktData(int tickerId, Contract contract, string genericTickList, bool snapshot, bool regulatorySnaphsot, List<TagValue> mktDataOptions);
        public void ReqMarketDataType(int marketDataType);
        public void ReqMarketDepth(int tickerId, Contract contract, int numRows, bool isSmartDepth, List<TagValue> mktDepthOptions);
        public void ReqNewsBulletins(bool allMessages);
        public void ReqOpenOrders();
        public void ReqPositions();
        public void ReqRealTimeBars(int tickerId, Contract contract, int barSize, string whatToShow, bool useRTH, List<TagValue> realTimeBarsOptions);
        public void ReqScannerParameters();
        public void ReqScannerSubscription(int reqId, ScannerSubscription subscription, List<TagValue> scannerSubscriptionOptions, List<TagValue> scannerSubscriptionFilterOptions);
        public void ReqScannerSubscription(int reqId, ScannerSubscription subscription, string scannerSubscriptionOptions, string scannerSubscriptionFilterOptions);
        public void SetServerLogLevel(int logLevel);
        public void VerifyRequest(string apiName, string apiVersion);
        public void VerifyMessage(string apiData);
        public void VerifyAndAuthRequest(string apiName, string apiVersion, string opaqueIsvKey);
        public void VerifyAndAuthMessage(string apiData, string xyzResponse);
        public void QueryDisplayGroups(int requestId);
        public void SubscribeToGroupEvents(int requestId, int groupId);
        public void UpdateDisplayGroup(int requestId, string contractInfo);
        public void UnsubscribeFromGroupEvents(int requestId);
        public void ReqPositionsMulti(int requestId, string account, string modelCode);
        public void CancelPositionsMulti(int requestId);
        public void ReqAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNLV);
        public void CancelAccountUpdatesMulti(int requestId);
        public void ReqSecDefOptParams(int reqId, string underlyingSymbol, string futFopExchange, string underlyingSecType, int underlyingConId);
        public void ReqSoftDollarTiers(int reqId);
        public void ReqFamilyCodes();
        public void ReqMatchingSymbols(int reqId, string pattern);
        public void ReqMktDepthExchanges();
        public void ReqSmartComponents(int reqId, string bboExchange);
        public void ReqNewsProviders();
        public void ReqNewsArticle(int requestId, string providerCode, string articleId, List<TagValue> newsArticleOptions);
        public void ReqHistoricalNews(int requestId, int conId, string providerCodes, string startDateTime, string endDateTime, int totalResults, List<TagValue> historicalNewsOptions);
        public void ReqHeadTimestamp(int tickerId, Contract contract, string whatToShow, int useRTH, int formatDate);
        public void CancelHeadTimestamp(int tickerId);
        public void ReqHistogramData(int tickerId, Contract contract, bool useRTH, string period);
        public void CancelHistogramData(int tickerId);
        public void ReqMarketRule(int marketRuleId);
        public void ReqPnL(int reqId, string account, string modelCode);
        public void CancelPnL(int reqId);
        public void ReqPnLSingle(int reqId, string account, string modelCode, int conId);
        public void CancelPnLSingle(int reqId);
        public void ReqHistoricalTicks(int reqId, Contract contract, string startDateTime,
                                       string endDateTime, int numberOfTicks, string whatToShow, int useRth, bool ignoreSize,
                                       List<TagValue> miscOptions);
        public void ReqWshMetaData(int reqId);
        public void CancelWshMetaData(int reqId);
        public void ReqWshEventData(int reqId, WshEventData wshEventData);
        public void CancelWshEventData(int reqId);
        public void ReqUserInfo(int reqId);
        public void ReqCurrentTimeInMillis();
        public bool IsDataAvailable();
        public int ReadInt();
        public byte[] ReadAtLeastNBytes(int msgSize);
        public byte[] ReadByteArray();
    }
}
