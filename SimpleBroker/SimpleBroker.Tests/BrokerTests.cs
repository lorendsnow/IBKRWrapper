using IBApi;
using IBKRWrapper;
using IBKRWrapper.Models;

namespace SimpleBroker.Tests
{
    public class BrokerTests
    {
        [Fact]
        public async Task GetPortfolioPositions_PositionsReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<PortfolioPosition> positions = await broker.GetPortfolioPositions("123456");

            Assert.Equal(3, positions.Count);
            Assert.Equal("AAPL", positions[0].Ticker);
            Assert.Equal(-100, positions[1].Quantity);
        }

        [Fact]
        public async Task GetAccountValues_ValuesReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            Dictionary<string, string> accountValues = await broker.GetAccountValuesAsync("123456");

            Assert.Equal(3, accountValues.Count);
            Assert.Equal("100000.54", accountValues["netLiq"]);
            Assert.Equal("987.54", accountValues["cash"]);
        }

        [Fact]
        public async Task GetContracts_ReturnsContracts()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<Contract> contracts = await broker.GetFullyDefinedContracts("AAPL", "STK");

            Assert.Single(contracts);
            Assert.Equal("AAPL", contracts[0].Symbol);
        }

        [Fact]
        public async Task GetOptionsContracts_ReturnsContracts()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<Contract> contracts = await broker.GetfullyDefinedOptionContractsByDate(
                "TSLA",
                "20240101",
                "P"
            );

            Assert.Equal(2, contracts.Count);
            Assert.Equal("TSLA", contracts[0].Symbol);
            Assert.Equal("P", contracts[0].Right);
            Assert.Equal("TSLA", contracts[1].Symbol);
            Assert.Equal("P", contracts[1].Right);
        }

        [Fact]
        public async Task GetOptionsChain_ChainReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            OptionsChain chain = await broker.GetOptionsChain("AAPL", "STK");

            Assert.Equal(3, chain.Expirations?.Count);
            Assert.Equal(5, chain.Strikes?.Count);
        }

        [Fact]
        public async Task GetHistoricalBars_BarsReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<Bar> bars = await broker.GetHistoricalBars(
                MockClient.TSLAContract,
                "1 D",
                "1 day",
                "TRADES"
            );

            Assert.Equal(3, bars.Count);
        }

        [Fact]
        public async Task GetHistoricalTicksLast_TicksReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<HistoricalTickLast> ticks = await broker.GetHistoricalTicksLast(
                MockClient.TSLAContract,
                "20240101",
                "",
                3,
                true
            );

            Assert.Equal(3, ticks.Count);
        }

        [Fact]
        public async Task GetHistoricalTicksBidAsk_TicksReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<HistoricalTickBidAsk> ticks = await broker.GetHistoricalTicksBidAsk(
                MockClient.TSLAContract,
                "20240101",
                "",
                3,
                true,
                false
            );

            Assert.Equal(3, ticks.Count);
        }

        [Fact]
        public async Task GetHistoricalTicksMid_TicksReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<HistoricalTick> ticks = await broker.GetHistoricalTicksMid(
                MockClient.TSLAContract,
                "20240101",
                "",
                3,
                true
            );

            Assert.Equal(3, ticks.Count);
        }

        [Fact]
        public async Task GetHeadTimestamp_TimestampReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            DateTimeOffset timestamp = await broker.GetHeadTimestamp(
                MockClient.TSLAContract,
                "TRADES",
                true,
                1
            );

            Assert.Equal(new DateTime(2024, 1, 1, 9, 30, 0), timestamp.DateTime);
        }

        [Fact]
        public async Task PlaceOrder_TradeReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);
            Contract contract = MockClient.TSLAContract;
            Order order =
                new()
                {
                    Account = "123456",
                    Action = "BUY",
                    OrderType = "LMT",
                    TotalQuantity = 100,
                    LmtPrice = 100
                };

            Trade trade = await broker.PlaceOrder(contract, order);

            Assert.Equal("PendingSubmit", trade.Status);
        }

        [Fact]
        public async Task GetOpenOrders_TradesReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<Trade> trades = await broker.GetOpenOrders();

            Assert.Equal(2, trades.Count);
        }

        [Fact]
        public async Task GetPositions_PositionsReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            List<Position> positions = await broker.GetPositions();

            Assert.Equal(2, positions.Count);
        }

        [Fact]
        public void GetRealTimeBars_RealTimeBarListReceived()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            RealTimeBarList bars = broker.GetRealTimeBars(MockClient.TSLAContract, "TRADES", true);

            Assert.Equal("TSLA", bars.Contract.Symbol);
            Assert.Empty(bars.Bars);
        }

        [Fact]
        public async Task GetScannerParameters_ParametersReturned()
        {
            Wrapper wrapper = new();
            MockClient client = new(wrapper, wrapper.Signal);
            wrapper.ClientSocket = client;
            Broker broker = Broker.CreateNew(wrapper);

            string parameters = await broker.GetScannerParameters();

            Assert.Equal("test string", parameters);
        }
    }
}
