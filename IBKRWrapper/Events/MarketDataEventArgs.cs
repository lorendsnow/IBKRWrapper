using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class MarketDataEventArgs<T>(MarketData<T> data) : EventArgs
    {
        public MarketData<T> Data { get; private set; } = data;
    }
}
