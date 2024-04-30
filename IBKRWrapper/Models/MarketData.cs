namespace IBKRWrapper.Models
{
    public record MarketData<T>(int ReqId, int Field, T Value);
}
