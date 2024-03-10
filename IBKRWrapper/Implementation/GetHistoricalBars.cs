using IBApi;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private Dictionary<int, List<Bar>> _barsResults = [];
        private Dictionary<int, TaskCompletionSource<List<Bar>>> _barsTcs = [];
        private Dictionary<int, TaskCompletionSource<DateTimeOffset>> _earliestDataTcs = [];
        private Dictionary<int, TaskCompletionSource<List<HistoricalTickLast>>> _lastTickTcs = [];
        private Dictionary<int, TaskCompletionSource<List<HistoricalTickBidAsk>>> _bidAskTcs = [];
        private Dictionary<int, TaskCompletionSource<List<HistoricalTick>>> _midTcs = [];
        private Dictionary<int, List<HistoricalTickLast>> _lastTickResults = [];
        private Dictionary<int, List<HistoricalTickBidAsk>> _bidAskResults = [];
        private Dictionary<int, List<HistoricalTick>> _midResults = [];

        /// <summary>
        /// Requests historical data from the TWS server
        /// </summary>
        /// <param name="contract">A Contract object for the instrument for which the bars are being requested: </param>
        /// <param name="durationString">Total duration of all bars being requested:
        /// <list type="table">
        ///     <item>
        ///         <term>S</term>
        ///         <description>Seconds</description>
        ///     </item>
        ///     <item>
        ///         <term>D</term>
        ///         <description>Days</description>
        ///     </item>
        ///     <item>
        ///         <term>W</term>
        ///         <description>Weeks</description>
        ///     </item>
        ///     <item>
        ///         <term>M</term>
        ///         <description>Months</description>
        ///     </item>
        ///     <item>
        ///         <term>Y</term>
        ///         <description>Years</description>
        ///     </item>
        /// </list>
        /// <example> "1 D" for one day, "1 W" for one week, "1 M" for one month, "1 Y" for one year, etc.</example>
        /// </param>
        /// <param name="barSizeSetting">Size of bars being requested:
        ///     - 1 sec
        ///     - 5 secs
        ///     - 15 secs
        ///     - 30 secs
        ///     - 1 min
        ///     - 2 mins
        ///     - 3 mins
        ///     - 5 mins
        ///     - 15 mins
        ///     - 30 mins
        ///     - 1 hour
        ///     - 1 day
        /// </param>
        /// <param name="whatToShow">The kind of information being retrieved (see IBKR API documentation for details):
        ///     - TRADES
        ///     - MIDPOINT
        ///     - BID
        ///     - ASK
        ///     - BID_ASK
        ///     - HISTORICAL_VOLATILITY
        ///     - OPTION_IMPLIED_VOLATILITY
        ///     - FEE_RATE
        ///     - SCHEDULE
        /// </param>
        /// <param name="useRTH">Whether to use data generated only during Regular Trading Hours (1) or not (0).</param>
        /// <param name="formatDate">Set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds</param>
        /// <param name="endDateTime">Request's ending time with format yyyyMMdd HH:mm:ss, or only yyyyMMdd for daily and above, or leave as an empty string to indicate now/today.
        /// </param>
        public Task<List<Bar>> GetHistoricalBarsAsync(
            Contract contract,
            string durationString,
            string barSizeSetting,
            string whatToShow,
            int useRTH = 1,
            int formatDate = 1,
            string endDateTime = ""
        )
        {
            int reqId = _reqId++;
            _barsResults[reqId] = [];
            TaskCompletionSource<List<Bar>> tcs = new();
            _barsTcs.Add(reqId, tcs);

            clientSocket.reqHistoricalData(
                reqId,
                contract,
                endDateTime,
                durationString,
                barSizeSetting,
                whatToShow,
                useRTH,
                formatDate,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        /// Requests historical "Last" tick data from the TWS server
        /// </summary>
        /// <param name="contract">A Contract object for the instrument for which the ticks are being requested.</param>
        /// <param name="startDateTime">Start time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting endDateTime.</param>
        /// <param name="endDateTime">end time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting startDateTime.</param>
        /// <param name="numberOfTicks">Number of ticks being requested (maximum of 1,000).</param>
        /// <param name="useRTH">Whether to use data generated only during Regular Trading Hours (1) or not (0).</param>
        public Task<List<HistoricalTickLast>> GetHistoricalTicksLastAsync(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            int useRth
        )
        {
            int reqId = _reqId++;
            _lastTickResults[reqId] = [];
            TaskCompletionSource<List<HistoricalTickLast>> tcs = new();
            _lastTickTcs.Add(reqId, tcs);

            clientSocket.reqHistoricalTicks(
                reqId,
                contract,
                startDateTime,
                endDateTime,
                numberOfTicks,
                "TRADES",
                useRth,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        /// Requests historical "Bid-Ask" tick data from the TWS server
        /// </summary>
        /// <param name="contract">A Contract object for the instrument for which the ticks are being requested.</param>
        /// <param name="startDateTime">Start time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting endDateTime.</param>
        /// <param name="endDateTime">end time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting startDateTime.</param>
        /// <param name="numberOfTicks">Number of ticks being requested (maximum of 1,000).</param>
        /// <param name="useRTH">Whether to use data generated only during Regular Trading Hours (1) or not (0).</param>
        /// <param name="ignoreSize">Whether to ignore size when returning bid/ask data.</param>
        public Task<List<HistoricalTickBidAsk>> GetHistoricalTicksBidAskAsync(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            int useRth,
            bool ignoreSize
        )
        {
            int reqId = _reqId++;
            _bidAskResults[reqId] = [];
            TaskCompletionSource<List<HistoricalTickBidAsk>> tcs = new();
            _bidAskTcs.Add(reqId, tcs);

            clientSocket.reqHistoricalTicks(
                reqId,
                contract,
                startDateTime,
                endDateTime,
                numberOfTicks,
                "BID_ASK",
                useRth,
                ignoreSize,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        /// Requests historical "Mid" tick data from the TWS server
        /// </summary>
        /// <param name="contract">A Contract object for the instrument for which the ticks are being requested.</param>
        /// <param name="startDateTime">Start time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting endDateTime.</param>
        /// <param name="endDateTime">end time for ticks being requested (Note - only one of startDateTime and endDateTime should be defined).
        /// Format is yyyyMMdd HH:mm:ss; use "" if setting startDateTime.</param>
        /// <param name="numberOfTicks">Number of ticks being requested (maximum of 1,000).</param>
        /// <param name="useRTH">Whether to use data generated only during Regular Trading Hours (1) or not (0).</param>
        public Task<List<HistoricalTick>> GetHistoricalTicksMidAsync(
            Contract contract,
            string startDateTime,
            string endDateTime,
            int numberOfTicks,
            int useRth
        )
        {
            int reqId = _reqId++;
            _midResults[reqId] = [];
            TaskCompletionSource<List<HistoricalTick>> tcs = new();
            _midTcs.Add(reqId, tcs);

            clientSocket.reqHistoricalTicks(
                reqId,
                contract,
                startDateTime,
                endDateTime,
                numberOfTicks,
                "MIDPOINT",
                useRth,
                false,
                []
            );

            return tcs.Task;
        }

        /// <summary>
        /// Handles the historical data received from the TWS server.
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="bar"></param>
        public void historicalData(int reqId, Bar bar) => _barsResults[reqId].Add(bar);

        /// <summary>
        /// Handles the end of historical data message from the TWS server.
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            _barsTcs[reqId].SetResult(_barsResults[reqId]);
            _barsResults.Remove(reqId);
            _barsTcs.Remove(reqId);
        }

        public void headTimestamp(int reqId, string headTimestamp)
        {
            DateTimeOffset stamp = IbDateParser.ParseIBDateTime(headTimestamp);
            _earliestDataTcs[reqId].SetResult(stamp);
            _earliestDataTcs.Clear();
        }

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            _lastTickResults[reqId].AddRange(ticks);
            if (done)
            {
                _lastTickTcs[reqId].SetResult(_lastTickResults[reqId]);
                _lastTickResults.Remove(reqId);
                _lastTickTcs.Remove(reqId);
            }
        }

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            _bidAskResults[reqId].AddRange(ticks);
            if (done)
            {
                _bidAskTcs[reqId].SetResult(_bidAskResults[reqId]);
                _bidAskResults.Remove(reqId);
                _bidAskTcs.Remove(reqId);
            }
        }

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            _midResults[reqId].AddRange(ticks);
            if (done)
            {
                _midTcs[reqId].SetResult(_midResults[reqId]);
                _midResults.Remove(reqId);
                _midTcs.Remove(reqId);
            }
        }
    }
}
