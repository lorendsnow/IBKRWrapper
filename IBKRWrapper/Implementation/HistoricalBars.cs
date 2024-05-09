using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public async Task<List<Bar>> GetDailyHistoricalBarsAsync(
            Contract contract,
            BarDataType dataType,
            int numberOfDays
        )
        {
            string durationString = $"{numberOfDays} D";
            return await GetHistoricalBarsAsync(
                contract,
                durationString,
                "1 day",
                dataType.ToString()
            );
        }

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
            int reqId;
            List<Bar> bars = [];
            TaskCompletionSource<List<Bar>> tcs = new();

            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            EventHandler<HistoricalDataEventArgs> handler =
                HandlerFactory.MakeHistoricalDataHandler(bars, reqId);

            EventHandler<HistoricalDataEndEventArgs> endHandler =
                HandlerFactory.MakeHistoricalDataEndHandler(bars, reqId, tcs);

            EventHandler<Events.ErrorEventArgs> errorHandler = (sender, e) => { if (e.Id == reqId) tcs.SetException(new Exception(e.ErrorMsg)); };

            HistoricalData += handler;
            HistoricalDataEnd += endHandler;
            ErrorEvent += errorHandler;

            tcs.Task.ContinueWith(t =>
            {
                HistoricalData -= handler;
                HistoricalDataEnd -= endHandler;
                ErrorEvent -= errorHandler;
            });

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
            int reqId;
            List<HistoricalTickLast> ticks = [];
            TaskCompletionSource<List<HistoricalTickLast>> tcs = new();

            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            EventHandler<HistoricalTicksLastEventArgs> handler =
                HandlerFactory.MakeHistoricalTicksLastHandler(ticks, reqId, tcs);

            HistoricalTicksLast += handler;

            tcs.Task.ContinueWith(t =>
            {
                HistoricalTicksLast -= handler;
            });

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
            int reqId;
            List<HistoricalTickBidAsk> ticks = [];
            TaskCompletionSource<List<HistoricalTickBidAsk>> tcs = new();

            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            EventHandler<HistoricalTicksBidAskEventArgs> handler =
                HandlerFactory.MakeHistoricalTicksBidAskHandler(ticks, reqId, tcs);

            HistoricalTicksBidAsk += handler;

            tcs.Task.ContinueWith(t =>
            {
                HistoricalTicksBidAsk -= handler;
            });

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
            int reqId;
            List<HistoricalTick> ticks = [];
            TaskCompletionSource<List<HistoricalTick>> tcs = new();

            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            EventHandler<HistoricalTicksMidEventArgs> handler =
                HandlerFactory.MakeHistoricalTicksMidHandler(ticks, reqId, tcs);

            HistoricalTicksMid += handler;

            tcs.Task.ContinueWith(t =>
            {
                HistoricalTicksMid -= handler;
            });

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

        public Task<DateTimeOffset> GetHeadTimestampAsync(
            Contract contract,
            string whatToShow,
            int useRTH
        )
        {
            int reqId;
            TaskCompletionSource<DateTimeOffset> tcs = new();

            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            EventHandler<HeadTimestampEventArgs> handler = HandlerFactory.MakeHeadTimestampHandler(
                tcs,
                reqId
            );

            HeadTimestamp += handler;

            tcs.Task.ContinueWith(t =>
            {
                HeadTimestamp -= handler;
            });

            clientSocket.reqHeadTimestamp(reqId, contract, whatToShow, useRTH, 1);

            return tcs.Task;
        }

        public event EventHandler<HistoricalDataEventArgs>? HistoricalData;

        public void historicalData(int reqId, Bar bar)
        {
            HistoricalData?.Invoke(this, new HistoricalDataEventArgs(reqId, bar));
        }

        public event EventHandler<HistoricalDataEndEventArgs>? HistoricalDataEnd;

        public void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            HistoricalDataEnd?.Invoke(
                this,
                new HistoricalDataEndEventArgs(reqId, startDate, endDate)
            );
        }

        public event EventHandler<HeadTimestampEventArgs>? HeadTimestamp;

        public void headTimestamp(int reqId, string headTimestamp)
        {
            HeadTimestamp?.Invoke(this, new HeadTimestampEventArgs(reqId, headTimestamp));
        }

        public event EventHandler<HistoricalTicksLastEventArgs>? HistoricalTicksLast;

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            HistoricalTicksLast?.Invoke(this, new HistoricalTicksLastEventArgs(reqId, ticks, done));
        }

        public event EventHandler<HistoricalTicksBidAskEventArgs>? HistoricalTicksBidAsk;

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            HistoricalTicksBidAsk?.Invoke(
                this,
                new HistoricalTicksBidAskEventArgs(reqId, ticks, done)
            );
        }

        public event EventHandler<HistoricalTicksMidEventArgs>? HistoricalTicksMid;

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            HistoricalTicksMid?.Invoke(this, new HistoricalTicksMidEventArgs(reqId, ticks, done));
        }
    }
}
