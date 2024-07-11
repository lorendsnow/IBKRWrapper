## SimpleBroker

SimpleBroker's API is exposed via the `Broker` class. Since this is somewhat of a hobby project, all documentation is in the source code in the form of XML tags. Those will (should) show up in your IDE's intellisense or equivalent as well. The data modeling, particularly the "Trade" object, will look familiar to anyone who has used the ib-insync package in python.

Quickstart example use:

```
using SimpleBroker;

namespace YourProgram
{
    internal class Program
    {
        static async Task Main()
        {
            Broker broker = new();

            // Connect to IBKR
            broker.Connect("127.0.0.1", 4001, 0)

            // Create a Contract object
            Contract contract = new("MSFT", "STK", "USD", "SMART");

            // Get historical bars for MSFT
            List<Bar> bars = await broker.GetHistoricalBars(contract, "1 Y", "1 day", "TRADES");

            // Make and place a market order for 100 shares of MSFT
            MarketOrder order = new("BUY", "DAY", "U123456", 100);

            Trade trade = await broker.PlaceOrder(contract, order);

            await Task.Delay(1000);

            if (trade.IsDone())
            {
                Console.WriteLine($"Trade Completed - Filled at {Trade.OrderStatus.AvgFillPrice.ToString("C")}");
            }

            // Disconnect from IBKR
            broker.Disconnect();
        }
    }
}
```
