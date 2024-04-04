using IBApi;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private readonly Dictionary<int, List<Bar>> _barsResults = [];
        private readonly Dictionary<int, TaskCompletionSource<List<Bar>>> _barsTcs = [];
        private readonly Dictionary<int, TaskCompletionSource<DateTimeOffset>> _earliestDataTcs =
        [];
        private readonly Dictionary<
            int,
            TaskCompletionSource<List<HistoricalTickLast>>
        > _lastTickTcs = [];
        private readonly Dictionary<
            int,
            TaskCompletionSource<List<HistoricalTickBidAsk>>
        > _bidAskTcs = [];
        private readonly Dictionary<int, TaskCompletionSource<List<HistoricalTick>>> _midTcs = [];
        private readonly Dictionary<int, List<HistoricalTickLast>> _lastTickResults = [];
        private readonly Dictionary<int, List<HistoricalTickBidAsk>> _bidAskResults = [];
        private readonly Dictionary<int, List<HistoricalTick>> _midResults = [];

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

        public void historicalData(int reqId, Bar bar) => _barsResults[reqId].Add(bar);

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
