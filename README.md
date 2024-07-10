# IBKRWrapper

IBKRWrapper is two projects in one, and aims to make it easier to use Interactive Brokers in C#/dotnet. IBKRWrapper is an event-based partial implementation of the Interactive Brokers API, and SimpleBroker uses IBKRWrapper to expose a simple and easy-to-use public API. You can use IBKRWrapper as a starting point to implement your own custom API, or just use SimpleBroker as a "batteries-included" API to begin using the Interactive Brokers API immediately.

A few caveats and things you should know before using:

- I'm a retail trader, so the implementation is aimed at a typical retail trader. In particular, SimpleBroker's Order object omits several fields from the IBKR Order object that are used by institutional clients only.
- I generally trade US equities and options, so I haven't done any testing using foreign currencies, futures, bonds, etc.
- Interactive Brokers doesn't have a test API, so my testing is limited to what I can do in my personal and paper trading accounts.

If you're not already familiar with the underlying IBKR API, it would be worth spending some time looking through their [API documentation](https://ibkrcampus.com/ibkr-api-page/twsapi-doc/)

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

## IBKRWrapper

IBKRWrapper partially implements Interactive Brokers' EWrapper class, with the implemented methods generally triggering events which emit the values sent to the method by IBKR.

### License

Copyright © 2024 [Loren Snow](https://github.com/lorendsnow)

This project is [MIT](https://github.com/lorendsnow/IBKRWrapper/blob/master/LICENSE.txt) licensed.
