﻿using IBApi;
using IBKRWrapper.Constants;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private int _nextOrderId;
        EClientSocket clientSocket;
        public readonly EReaderSignal Signal;
        public EReader? reader;
        private int _reqId;
        private readonly object _reqIdLock = new();
        private MarketDataType _marketDataTypeRequested = MarketDataType.Live;

        public Wrapper()
        {
            Signal = new EReaderMonitorSignal();
            clientSocket = new EClientSocket(this, Signal);
        }

        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            set { clientSocket = value; }
        }

        public int NextOrderId
        {
            get => _nextOrderId;
            set => _nextOrderId = value;
        }

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

        public void Disconnect()
        {
            clientSocket.eDisconnect();
        }
    }
}
