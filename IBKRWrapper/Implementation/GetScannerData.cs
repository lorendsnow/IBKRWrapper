using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private TaskCompletionSource<string>? _paramsTcs = null;

        /// <summary>
        /// Emits the req ID, rank and <see cref="IBApi.Contract"/> for scanner data results received from IBKR.
        /// </summary>
        public event EventHandler<IBKRScannerDataEventArgs>? IBKRScannerDataEvent;

        /// <summary>
        /// Request all of the valid scanner parameters.
        /// </summary>
        /// <returns>XML list of valid scanner parameters.</returns>
        public Task<string> GetScannerParametersAsync()
        {
            _paramsTcs = new();
            clientSocket.reqScannerParameters();
            return _paramsTcs.Task;
        }

        /// <summary>
        /// Request a scanner subscription.
        /// </summary>
        /// <param name="instrument">Instrument type to use (ScanParameterResponse::InstrumentList::Instrument::type).</param>
        /// <param name="location">Country or region for scanner to search (ScanParameterResponse::LocationTree::Location::LocationTree::Location::locationCode).</param>
        /// <param name="scanCode">Value for scanner to sort by (ScanParameterResponse::ScanTypeList::ScanType::scanCode).</param>
        /// <param name="filterOptions">Dictionary of options to filter the results(ScanParameterResponse::FilterList::RangeFilter::AbstractField::code).</param>
        /// <returns><see cref="ScannerData"/> object which holds the request details and will receive scan results as received from IBKR.</returns>
        public ScannerData GetScannerData(
            string instrument,
            string location,
            string scanCode,
            Dictionary<string, string>? filterOptions
        )
        {
            int reqId = _reqId++;
            ScannerSubscription scannerSub = MakeScannerSubscription(
                instrument,
                location,
                scanCode
            );
            List<TagValue> tagValues =
                filterOptions != null ? MakeFilterOptions(filterOptions) : [];

            ScannerData scannerData = new(reqId, scannerSub, tagValues);
            IBKRScannerDataEvent += scannerData.HandleScannerData;

            clientSocket.reqScannerSubscription(reqId, scannerSub, null, tagValues);

            return scannerData;
        }

        /// <summary>
        /// Cancel a scanner subscription.
        /// </summary>
        /// <param name="scannerData"></param>
        public void CancelScannerSubscription(ScannerData scannerData)
        {
            IBKRScannerDataEvent -= scannerData.HandleScannerData;
            clientSocket.cancelScannerSubscription(scannerData.ReqId);
        }

        private List<TagValue> MakeFilterOptions(Dictionary<string, string> filterOptions)
        {
            List<TagValue> tagValues = [];
            foreach (KeyValuePair<string, string> option in filterOptions)
            {
                tagValues.Append(new TagValue() { Tag = option.Key, Value = option.Value });
            }
            return tagValues;
        }

        private ScannerSubscription MakeScannerSubscription(
            string instrument,
            string location,
            string scanCode
        )
        {
            return new()
            {
                Instrument = instrument,
                LocationCode = location,
                ScanCode = scanCode,
            };
        }

        public void scannerParameters(string xml)
        {
            if (_paramsTcs == null)
                return;

            _paramsTcs.SetResult(xml);
            _paramsTcs = null;
        }

        public virtual void scannerData(
            int reqId,
            int rank,
            ContractDetails contractDetails,
            string distance,
            string benchmark,
            string projection,
            string legsStr
        )
        {
            OnScannerData(reqId, rank, contractDetails.Contract);
        }

        private void OnScannerData(int reqId, int rank, Contract contract)
        {
            IBKRScannerDataEvent?.Invoke(this, new IBKRScannerDataEventArgs(reqId, rank, contract));
        }
    }
}
