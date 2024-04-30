using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private readonly object _scannerParamLock = new();

        public Task<string> GetScannerParametersAsync()
        {
            lock (_scannerParamLock)
            {
                TaskCompletionSource<string> tcs = new();

                EventHandler<ScannerParametersEventArgs> handler =
                    HandlerFactory.MakeScannerParametersHandler(tcs);

                ScannerParametersEvent += handler;

                tcs.Task.ContinueWith(t =>
                {
                    ScannerParametersEvent -= handler;
                });

                clientSocket.reqScannerParameters();

                return tcs.Task;
            }
        }

        public ScannerData GetScannerData(
            string instrument,
            string location,
            string scanCode,
            Dictionary<string, string>? filterOptions
        )
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            ScannerSubscription scannerSub = MakeScannerSubscription(
                instrument,
                location,
                scanCode
            );
            List<TagValue> tagValues =
                filterOptions != null ? MakeFilterOptions(filterOptions) : [];

            ScannerData scannerData = new(reqId, scannerSub, tagValues);
            ScannerDataEvent += scannerData.HandleScannerData;
            ScannerDataEndEvent += scannerData.HandleScannerDataEnd;

            clientSocket.reqScannerSubscription(reqId, scannerSub, null, tagValues);

            return scannerData;
        }

        public void CancelScannerSubscription(ScannerData scannerData)
        {
            ScannerDataEvent -= scannerData.HandleScannerData;
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

        public event EventHandler<ScannerDataEventArgs>? ScannerDataEvent;

        public event EventHandler<ScannerParametersEventArgs>? ScannerParametersEvent;

        public event EventHandler<ScannerDataEndArgs>? ScannerDataEndEvent;

        public void scannerParameters(string xml)
        {
            ScannerParametersEvent?.Invoke(this, new ScannerParametersEventArgs(xml));
        }

        public void scannerData(
            int reqId,
            int rank,
            ContractDetails contractDetails,
            string distance,
            string benchmark,
            string projection,
            string legsStr
        )
        {
            ScannerDataEvent?.Invoke(
                this,
                new ScannerDataEventArgs(reqId, rank, contractDetails.Contract)
            );
        }

        public void scannerDataEnd(int reqId)
        {
            ScannerDataEndEvent?.Invoke(this, new ScannerDataEndArgs(reqId));
        }
    }
}
