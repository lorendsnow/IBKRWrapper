namespace IBApi.Interfaces
{
    public interface IClientMsgSink
    {
        void ServerVersion(int version, string time);
        void Redirect(string host);
    }
}
