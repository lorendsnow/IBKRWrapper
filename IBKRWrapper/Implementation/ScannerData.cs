using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public static List<TagValue> MakeFilterOptions(Dictionary<string, string> filterOptions)
        {
            List<TagValue> tagValues = [];
            foreach (KeyValuePair<string, string> option in filterOptions)
            {
                tagValues.Add(new TagValue() { Tag = option.Key, Value = option.Value });
            }
            return tagValues;
        }

        public static ScannerSubscription MakeScannerSubscription(
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
