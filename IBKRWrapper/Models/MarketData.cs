namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a market data value received from IBKR
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ReqId"></param>
    /// <param name="Field"></param>
    /// <param name="Value"></param>
    public record MarketData<T>(int ReqId, int Field, T Value);
}
