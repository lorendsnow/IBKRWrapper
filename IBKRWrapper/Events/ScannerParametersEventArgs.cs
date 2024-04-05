namespace IBKRWrapper.Events
{
    public class ScannerParametersEventArgs(string xml) : EventArgs
    {
        public string XML { get; private set; } = xml;
    }
}
