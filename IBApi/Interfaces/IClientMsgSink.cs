namespace IBApi.Interfaces
{
    public interface IClientMsgSink
    {
        void serverVersion(int version, string time);
        void redirect(string host);
    }
}
