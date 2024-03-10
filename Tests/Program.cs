using System.Globalization;
using System.Xml;
using IBApi;
using IBKRWrapper;
using IBKRWrapper.Models;
using IBKRWrapper.Utils;

namespace Testing
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Wrapper wrapper = new();
            wrapper.Connect("192.168.50.200", 4001, 0);

            ScannerData data = wrapper.GetScannerData(
                "ETF.EQ.US",
                "ETF.EQ.US.MAJOR",
                "TOP_PERC_GAIN",
                null
            );

            await Task.Delay(5000);

            foreach (KeyValuePair<int, Contract> result in data.ScannerResults)
            {
                Console.WriteLine($"Rank: {result.Key}  |  Ticker: {result.Value.Symbol}");
            }

            wrapper.Disconnect();

            //List<Contract> contractList = await wrapper.GetQualifiedStockContractAsync("AAPL");
            //Contract contract = contractList.First();

            //List<HistoricalTickLast> historicalTicksLast =
            //    await wrapper.GetHistoricalTicksLastAsync(
            //        contract,
            //        "",
            //        "20240308 11:00:00",
            //        1000,
            //        0
            //    );

            //foreach (HistoricalTickLast tick in historicalTicksLast)
            //{
            //    DateTimeOffset time = IbDateParser.ParseIBDateTime(tick.Time.ToString());
            //    string price = tick.Price.ToString(".00", CultureInfo.InvariantCulture);
            //    string size = tick.Size.ToString();
            //    string exchange = tick.Exchange;
            //    string specialConditions = tick.SpecialConditions;

            //    Console.WriteLine(
            //        $"Time: {time}  |  Price: {price}  |  Size: {size}  |  Exchange: {exchange}  |  Special Conditions: {specialConditions}"
            //    );
            //}

            //List<Bar> bars = await wrapper.GetHistoricalBarsAsync(
            //    contract,
            //    "5 D",
            //    "5 mins",
            //    "TRADES"
            //);

            //foreach (Bar bar in bars)
            //{
            //    DateTimeOffset time = IbDateParser.ParseIBDateTime(bar.Time);
            //    string open = bar.Open.ToString(".00", CultureInfo.InvariantCulture);
            //    string high = bar.High.ToString(".00", CultureInfo.InvariantCulture);
            //    string low = bar.Low.ToString(".00", CultureInfo.InvariantCulture);
            //    string close = bar.Close.ToString(".00", CultureInfo.InvariantCulture);
            //    string volume = bar.Volume.ToString();

            //    Console.WriteLine(
            //        $"Time: {time}  |  Open: {open}  |  High: {high}  |  Low: {low}  |  Close: {close}  |  Volume: {volume}"
            //    );
        }
    }
}
