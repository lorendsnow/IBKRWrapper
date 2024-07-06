using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Receives and holds scanner data requested from IBKR
    /// </summary>
    /// <param name="reqId"></param>
    /// <param name="sub"></param>
    /// <param name="filterOptions"></param>
    public class ScannerData(int reqId, ScannerSubscription sub, List<TagValue> filterOptions)
    {
        /// <summary>
        /// The ID sent to IBKR with the initial request
        /// </summary>
        public int ReqId { get; init; } = reqId;

        /// <summary>
        /// The subscription used to request the scanner data
        /// </summary>
        public ScannerSubscription Subscription { get; init; } = sub;

        /// <summary>
        /// The filter options used to request the scanner data
        /// </summary>
        public List<TagValue> FilterOptions { get; init; } = filterOptions;

        /// <summary>
        /// The results of the scanner request
        /// </summary>
        public Dictionary<int, Contract> ScannerResults { get; private set; } = [];

        /// <summary>
        /// Whether or not the scanner has finished scanning
        /// </summary>
        public bool DoneScanning { get; private set; } = false;

        /// <summary>
        /// Handles incoming scanner data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleScannerData(object? sender, ScannerDataEventArgs e)
        {
            if (e.ReqId == ReqId)
            {
                ScannerResults.TryAdd(e.Rank, e.Contract);
            }
        }

        /// <summary>
        /// Handles the end of the scanner data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleScannerDataEnd(object? sender, ScannerDataEndArgs e)
        {
            if (e.ReqId == ReqId)
            {
                DoneScanning = true;
            }
        }

        /// <summary>
        /// Returns the results of the scanner request as a string
        /// </summary>
        /// <returns>The scan results, in <see cref="string"/> format</returns>
        public string ResultsString()
        {
            return string.Join(
                "\n",
                ScannerResults.Select(x => $"Ticker: {x.Value.Symbol}  |   Rank: {x.Key}").ToArray()
            );
        }
    }
}
