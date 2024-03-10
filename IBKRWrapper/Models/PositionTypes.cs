using IBApi;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a position with PnL and market value information.
    /// </summary>
    public record PortfolioPosition
    {
        /// <summary>
        /// Constructor for the PortfolioPosition record type.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="symbol"></param>
        /// <param name="securityType"></param>
        /// <param name="exchange"></param>
        /// <param name="currency"></param>
        /// <param name="primaryExchange"></param>
        /// <param name="positions"></param>
        /// <param name="averageCost"></param>
        /// <param name="marketPrice"></param>
        /// <param name="marketValue"></param>
        /// <param name="unrealizedPNL"></param>
        /// <param name="realizedPNL"></param>
        /// <param name="account"></param>
        public PortfolioPosition(
            int contractId,
            string symbol,
            string securityType,
            string exchange,
            string currency,
            string primaryExchange,
            decimal positions,
            double averageCost,
            double marketPrice,
            double marketValue,
            double unrealizedPNL,
            double realizedPNL,
            string account
        )
        {
            ConId = contractId;
            Symbol = symbol;
            SecType = securityType;
            Exchange = exchange;
            Currency = currency;
            PrimaryExch = primaryExchange;
            Positions = positions;
            AvgCost = averageCost;
            MarketPrice = marketPrice;
            MarketValue = marketValue;
            UnrealizedPNL = unrealizedPNL;
            RealizedPNL = realizedPNL;
            Account = account;
        }

        /// <summary>
        /// The IBKR contract ID of the position.
        /// </summary>
        public int ConId { get; init; }

        /// <summary>
        /// The position's symbol.
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The position's security type.
        /// </summary>
        public string SecType { get; init; }

        /// <summary>
        /// The position's exchange.
        /// </summary>
        public string Exchange { get; init; }

        /// <summary>
        /// The position's currency.
        /// </summary>
        public string Currency { get; init; }

        /// <summary>
        /// The position's primary exchange.
        /// </summary>
        public string PrimaryExch { get; init; }

        /// <summary>
        /// The number of positions (e.g., shares, contracts, etc.).
        /// </summary>
        public decimal Positions { get; init; }

        /// <summary>
        /// The average cost of the position.
        /// </summary>
        public double AvgCost { get; init; }

        /// <summary>
        /// The instrument's unitary price.
        /// </summary>
        public double MarketPrice { get; init; }

        /// <summary>
        /// The instrument's total market value.
        /// </summary>
        public double MarketValue { get; init; }

        /// <summary>
        /// The instrument's unrealized profit and loss.
        /// </summary>
        public double UnrealizedPNL { get; init; }

        /// <summary>
        /// The instrument's realized profit and loss.
        /// </summary>
        public double RealizedPNL { get; init; }

        /// <summary>
        /// The account to which the position belongs.
        /// </summary>
        public string Account { get; init; }
    }

    /// <summary>
    /// Record type to represent a basic position as returned by the GetPosition method.
    /// </summary>
    public record Position
    {
        /// <summary>
        /// Constructor for the Position record type.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="symbol"></param>
        /// <param name="securityType"></param>
        /// <param name="exchange"></param>
        /// <param name="currency"></param>
        /// <param name="primaryExchange"></param>
        /// <param name="positions"></param>
        /// <param name="averageCost"></param>
        /// <param name="account"></param>
        public Position(
            int contractId,
            string symbol,
            string securityType,
            string exchange,
            string currency,
            string primaryExchange,
            decimal positions,
            double averageCost,
            string account
        )
        {
            ConId = contractId;
            Symbol = symbol;
            SecType = securityType;
            Exchange = exchange;
            Currency = currency;
            PrimaryExch = primaryExchange;
            Positions = positions;
            AvgCost = averageCost;
            Account = account;
        }

        /// <summary>
        /// The IBKR contract ID of the position.
        /// </summary>
        public int ConId { get; init; }

        /// <summary>
        /// The position's symbol.
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The position's security type.
        /// </summary>
        public string SecType { get; init; }

        /// <summary>
        /// The position's exchange.
        /// </summary>
        public string Exchange { get; init; }

        /// <summary>
        /// The position's currency.
        /// </summary>
        public string Currency { get; init; }

        /// <summary>
        /// The position's primary exchange.
        /// </summary>
        public string PrimaryExch { get; init; }

        /// <summary>
        /// The number of positions (e.g., shares, contracts, etc.).
        /// </summary>
        public decimal Positions { get; init; }

        /// <summary>
        /// The average cost of the position.
        /// </summary>
        public double AvgCost { get; init; }

        /// <summary>
        /// The account to which the position belongs.
        /// </summary>
        public string Account { get; init; }
    }

    /// <summary>
    /// Convenience class to build Position records with differen parameters
    /// </summary>
    public static class PositionBuilder
    {
        /// <summary>
        /// Build a position record from a Contract object and other parameters.
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="positions"></param>
        /// <param name="avgCost"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Position Build(
            Contract contract,
            decimal positions,
            double avgCost,
            string account
        )
        {
            return new Position(
                contract.ConId,
                contract.Symbol,
                contract.SecType,
                contract.Exchange,
                contract.Currency,
                contract.PrimaryExch,
                positions,
                avgCost,
                account
            );
        }

        /// <summary>
        /// Build a new portfolio position record from a Contract object and other parameters.
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="positions"></param>
        /// <param name="marketPrice"></param>
        /// <param name="marketValue"></param>
        /// <param name="avgCost"></param>
        /// <param name="unrealizedPNL"></param>
        /// <param name="realizedPNL"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static PortfolioPosition Build(
            Contract contract,
            decimal positions,
            double marketPrice,
            double marketValue,
            double avgCost,
            double unrealizedPNL,
            double realizedPNL,
            string account
        )
        {
            return new PortfolioPosition(
                contract.ConId,
                contract.Symbol,
                contract.SecType,
                contract.Exchange,
                contract.Currency,
                contract.PrimaryExch,
                positions,
                avgCost,
                marketPrice,
                marketValue,
                unrealizedPNL,
                realizedPNL,
                account
            );
        }
    }
}
