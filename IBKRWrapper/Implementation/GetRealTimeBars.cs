using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<IBKRrtBarEventArgs>? IBKRrtBarEvent;

        /// <summary>
        /// Get streaming real-time (5-second) bars for a given contract.
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="barSize"></param>
        /// <param name="whatToShow"></param>
        /// <param name="useRTH"></param>
        /// <returns><see cref="RealTimeBarList"/> - holds the request information and a list of bars received.</returns>
        public RealTimeBarList GetRealTimeBars(
            Contract contract,
            int barSize,
            string whatToShow,
            bool useRTH
        )
        {
            int reqId = _reqId++;
            RealTimeBarList rtBars =
                new()
                {
                    ReqId = reqId,
                    Contract = contract,
                    BarSize = barSize,
                    WhatToShow = whatToShow,
                    UseRTH = useRTH
                };

            IBKRrtBarEvent += rtBars.HandleNewBar;

            clientSocket.reqRealTimeBars(reqId, contract, barSize, whatToShow, useRTH, null);

            return rtBars;
        }

        public virtual void realtimeBar(
            int reqId,
            long time,
            double open,
            double high,
            double low,
            double close,
            decimal volume,
            decimal WAP,
            int count
        )
        {
            DateTimeOffset convertedTime = DateTimeOffset.FromUnixTimeSeconds(time);
            RealTimeBar bar =
                new()
                {
                    TimeOffset = convertedTime,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume,
                    WAP = WAP,
                    Count = count
                };

            IBKRrtBarEventArgs e = new(reqId, bar);
            OnIBKRrtBarEvent(e);
        }

        public void OnIBKRrtBarEvent(IBKRrtBarEventArgs e)
        {
            IBKRrtBarEvent?.Invoke(this, e);
        }
    }
}
