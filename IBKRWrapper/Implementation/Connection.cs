using IBApi;
using IBKRWrapper.Constants;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private int _nextOrderId;
        IClientSocket clientSocket;
        public readonly EReaderSignal Signal;
        public EReader? reader;
        private int _reqId;
        private MarketDataType _marketDataTypeRequested = MarketDataType.Live;

        public Wrapper()
        {
            Signal = new EReaderMonitorSignal();
            clientSocket = new ClientSocket(this, Signal);
        }

        public IClientSocket ClientSocket
        {
            get => clientSocket;
            set => clientSocket = value;
        }

        public int NextOrderId
        {
            get => _nextOrderId;
            set => _nextOrderId = value;
        }

        public int ReqId
        {
            get => _reqId;
            set => _reqId = value;
        }

        public bool IsConnected => clientSocket.IsConnected();

        public MarketDataType LastMarketDataTypeRequested
        {
            get => _marketDataTypeRequested;
            private set => _marketDataTypeRequested = value;
        }

        public void nextValidId(int orderId)
        {
            _nextOrderId = orderId;
        }

        public void Connect(string host, int port, int clientId)
        {
            clientSocket.eConnect(host, port, clientId);

            EReader reader = new(ClientSocket.clientSocket, Signal);
            reader.Start();

            new Thread(() =>
            {
                while (clientSocket.IsConnected())
                {
                    Signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        public void Disconnect()
        {
            clientSocket.eDisconnect();
        }
    }
}
