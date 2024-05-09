using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public LiveMarketData RequestMarketData(Contract contract)
        {
            if (LastMarketDataTypeRequested != MarketDataType.Live)
            {
                SetMarketDataLive();
            }
            
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            LiveMarketData data = new(reqId, contract);

            StringMarketDataEvent += data.UpdateMarketData;
            DecimalMarketDataEvent += data.UpdateMarketData;
            DoubleMarketDataEvent += data.UpdateMarketData;
            OptionGreeksMarketDataEvent += data.UpdateMarketData;

            clientSocket.reqMktData(reqId, contract, "", false, false, null);

            return data;
        }

        public FrozenMarketData RequestFrozenMarketData(Contract contract)
        {
            if (LastMarketDataTypeRequested != MarketDataType.Frozen)
            {
                SetMarketDataFrozen();
            }
        
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            FrozenMarketData data = new(reqId, contract);

            StringMarketDataEvent += data.UpdateMarketData;
            DecimalMarketDataEvent += data.UpdateMarketData;
            DoubleMarketDataEvent += data.UpdateMarketData;
            OptionGreeksMarketDataEvent += data.UpdateMarketData;

            clientSocket.reqMktData(reqId, contract, "", false, false, null);

            return data;
        }

        public Task<OptionGreeks> CalculateOptionIV(Contract contract, double optionPrice, double underlyingPrice)
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            TaskCompletionSource<OptionGreeks> tcs = new();

            EventHandler<MarketDataEventArgs<OptionGreeks>> handler = (sender, e) =>
            {
                if (e.Data.ReqId == reqId) tcs.SetResult(e.Data.Value);
            };

            OptionGreeksMarketDataEvent += handler;

            tcs.Task.ContinueWith(t =>
            {
                OptionGreeksMarketDataEvent -= handler;
            });

            clientSocket.calculateImpliedVolatility(reqId, contract, optionPrice, underlyingPrice, null);

            return tcs.Task;
        }

        public void SetMarketDataLive() => { clientSocket.reqMarketDataType(1); LastMarketDataTypeRequested = MarketDataType.Live; }
        public void SetMarketDataFrozen() => { clientSocket.reqMarketDataType(2); LastMarketDataTypeRequested = MarketDataType.Frozen; }
        public void SetMarketDataDelayed() => { clientSocket.reqMarketDataType(3); LastMarketDataTypeRequested = MarketDataType.Delayed; }
        public void SetMarketDataDelayedFrozen() => { clientSocket.reqMarketDataType(4); LastMarketDataTypeRequested = MarketDataType.DelayedFrozen; }

        public event EventHandler<MarketDataEventArgs<double>>? DoubleMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<decimal>>? DecimalMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<string>>? StringMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<OptionGreeks>>? OptionGreeksMarketDataEvent;

        public void tickGeneric(int tickerId, int field, double value)
        {
            if (field == 49)
            {
                MarketData<double> data = new(tickerId, field, value);
                DoubleMarketDataEvent?.Invoke(this, new MarketDataEventArgs<double>(data));
            }
        }

        public void tickString(int tickerId, int tickType, string value)
        {
            MarketData<string> data = new(tickerId, tickType, value);

            StringMarketDataEvent?.Invoke(this, new MarketDataEventArgs<string>(data));
        }

        public void tickSize(int tickerId, int field, decimal size)
        {
            MarketData<decimal> data = new(tickerId, field, size);

            DecimalMarketDataEvent?.Invoke(this, new MarketDataEventArgs<decimal>(data));
        }

        public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            MarketData<double> data = new(tickerId, field, price);

            DoubleMarketDataEvent?.Invoke(this, new MarketDataEventArgs<double>(data));
        }

        public void tickOptionComputation(
            int tickerId,
            int field,
            int tickAttrib,
            double impliedVolatility,
            double delta,
            double optPrice,
            double pvDividend,
            double gamma,
            double vega,
            double theta,
            double undPrice
        )
        {
            OptionGreeks greeks =
                new(
                    tickerId,
                    field,
                    tickAttrib,
                    impliedVolatility,
                    delta,
                    optPrice,
                    pvDividend,
                    gamma,
                    vega,
                    theta,
                    undPrice
                );

            MarketData<OptionGreeks> data = new(tickerId, field, greeks);

            OptionGreeksMarketDataEvent?.Invoke(this, new MarketDataEventArgs<OptionGreeks>(data));
        }
    }
}
