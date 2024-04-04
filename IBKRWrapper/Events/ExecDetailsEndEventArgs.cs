namespace IBKRWrapper.Events
{
    public class ExecDetailsEndEventArgs(int reqId) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
    }
}
