﻿using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class ScannerData(int reqId, ScannerSubscription sub, List<TagValue> filterOptions)
    {
        public int ReqId { get; init; } = reqId;
        public ScannerSubscription Subscription { get; init; } = sub;
        public List<TagValue> FilterOptions { get; init; } = filterOptions;
        public Dictionary<int, Contract> ScannerResults { get; private set; } = [];
        public bool DoneScanning { get; private set; } = false;

        public void HandleScannerData(object? sender, ScannerDataEventArgs e)
        {
            if (e.ReqId == ReqId)
            {
                ScannerResults.TryAdd(e.Rank, e.Contract);
            }
        }

        public void HandleScannerDataEnd(object? sender, ScannerDataEndArgs e)
        {
            if (e.ReqId == ReqId)
            {
                DoneScanning = true;
            }
        }

        public string ResultsString()
        {
            return string.Join(
                "\n",
                ScannerResults.Select(x => $"Ticker: {x.Value.Symbol}  |   Rank: {x.Key}").ToArray()
            );
        }
    }
}
