using IBApi;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public int nextOrderId;
        EClientSocket clientSocket;
        public readonly EReaderSignal Signal;
        public EReader? reader;
        private int _reqId;

        public Wrapper()
        {
            Signal = new EReaderMonitorSignal();
            clientSocket = new EClientSocket(this, Signal);
        }

        /// <summary>
        /// TWS/Gateway client class This client class contains all the available methods to communicate with IB.
        /// </summary>
        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            set { clientSocket = value; }
        }

        /// <summary>
        /// Next valid order ID, as received from the TWS server.s
        /// </summary>
        public int NextOrderId
        {
            get { return nextOrderId; }
            set { nextOrderId = value; }
        }

        public bool OrderIdIsValid { get; set; }

        /// <summary>
        /// Receives next valid order id. Will be invoked automatically upon successfull API client connection, or after call to EClient::reqIds
        /// </summary>
        /// <param name="orderId"></param>
        public virtual void nextValidId(int orderId)
        {
            NextOrderId = orderId;
            OrderIdIsValid = true;
        }

        /// <summary>
        /// Connect to the TWS/Gateway client.
        /// </summary>
        /// <param name="host">IP Address or localhost.</param>
        /// <param name="port">Port number.</param>
        /// <param name="clientId">Client ID.</param>
        public void Connect(string host, int port, int clientId)
        {
            clientSocket.eConnect(host, port, clientId);

            EReader reader = new(clientSocket, Signal);
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

        /// <summary>
        /// Disconnect from the TWS/Gateway client.
        /// </summary>
        public void Disconnect()
        {
            clientSocket.eDisconnect();
        }
    }
}
