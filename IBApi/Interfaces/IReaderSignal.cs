namespace IBApi.Interfaces
{
    public interface IReaderSignal
    {
        void IssueSignal();
        void WaitForSignal();
    }
}
