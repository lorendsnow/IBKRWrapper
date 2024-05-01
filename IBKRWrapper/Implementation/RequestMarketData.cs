using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public LiveMarketData RequestMarketData(Contract contract)
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            LiveMarketData data = GetDataObject(reqId, contract);
            StringMarketDataEvent += data.UpdateMarketData;
            DecimalMarketDataEvent += data.UpdateMarketData;
            DoubleMarketDataEvent += data.UpdateMarketData;
            OptionGreeksMarketDataEvent += data.UpdateMarketData;

            clientSocket.reqMktData(reqId, contract, "", false, false, null);

            return data;
        }

        private static LiveMarketData GetDataObject(int reqId, Contract contract) =>
            contract.SecType switch
            {
                "STK" => new EquityMarketData(reqId, contract),
                "OPT" => new OptionMarketData(reqId, contract),
                _ => throw new NotImplementedException("Haven't implemented that type yet")
            };

        public event EventHandler<MarketDataEventArgs<double>>? DoubleMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<decimal>>? DecimalMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<string>>? StringMarketDataEvent;
        public event EventHandler<MarketDataEventArgs<OptionGreeks>>? OptionGreeksMarketDataEvent;

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
