using IBApi.Interfaces;

namespace IBApi
{
    public class IBSocket : ITransport, IDisposable
    {
        private readonly BinaryWriter tcpWriter;
        private readonly object tcpWriterLock = new object();

        public IBSocket(Stream socketStream) => tcpWriter = new BinaryWriter(socketStream);

        public void Send(EMessage msg)
        {
            lock (tcpWriterLock)
            {
                tcpWriter.Write(msg.GetBuf());
            }
        }

        public void Dispose() => tcpWriter.Dispose();
    }
}
