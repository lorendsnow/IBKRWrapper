using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private TaskCompletionSource<string>? _paramsTcs = null;

        public event EventHandler<IBKRScannerDataEventArgs>? IBKRScannerDataEvent;

        public Task<string> GetScannerParametersAsync()
        {
            _paramsTcs = new();
            clientSocket.reqScannerParameters();
            return _paramsTcs.Task;
        }

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

        public void CancelScannerSubscription(ScannerData scannerData)
        {
            IBKRScannerDataEvent -= scannerData.HandleScannerData;
            clientSocket.cancelScannerSubscription(scannerData.ReqId);
        }

        private static List<TagValue> MakeFilterOptions(Dictionary<string, string> filterOptions)
        {
            List<TagValue> tagValues = [];
            foreach (KeyValuePair<string, string> option in filterOptions)
            {
                tagValues.Add(new TagValue() { Tag = option.Key, Value = option.Value });
            }
            return tagValues;
        }

        private static ScannerSubscription MakeScannerSubscription(
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
