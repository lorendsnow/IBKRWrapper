namespace SimpleBroker
{
    internal static class MappingExtensions
    {
        internal static Contract ToBrokerContract(this IBApi.Contract contract)
        {
            return new Contract(
                contract.Symbol,
                contract.SecType,
                contract.Currency,
                contract.Exchange
            )
            {
                ConId = contract.ConId,
                LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth,
                Strike = contract.Strike,
                Right = contract.Right,
                Multiplier = contract.Multiplier,
                LocalSymbol = contract.LocalSymbol,
                PrimaryExch = contract.PrimaryExch,
                TradingClass = contract.TradingClass,
                IncludeExpired = contract.IncludeExpired,
                SecIdType = contract.SecIdType,
                SecId = contract.SecId,
                Description = contract.Description,
                IssuerId = contract.IssuerId,
                ComboLegsDescription = contract.ComboLegsDescription,
                ComboLegs = contract.ComboLegs?.Select(x => x.ToBrokerComboLeg()).ToList(),
                DeltaNeutralContract = contract.DeltaNeutralContract?.ToBrokerDeltaNeutralContract()
            };
        }

        internal static IBApi.Contract ToIBKRContract(this Contract contract)
        {
            return new()
            {
                ConId = contract.ConId,
                Symbol = contract.Symbol,
                SecType = contract.SecType,
                LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth,
                Strike = contract.Strike,
                Right = contract.Right,
                Multiplier = contract.Multiplier,
                Exchange = contract.Exchange,
                Currency = contract.Currency,
                LocalSymbol = contract.LocalSymbol,
                PrimaryExch = contract.PrimaryExch,
                TradingClass = contract.TradingClass,
                IncludeExpired = contract.IncludeExpired,
                SecIdType = contract.SecIdType,
                SecId = contract.SecId,
                Description = contract.Description,
                IssuerId = contract.IssuerId,
                ComboLegsDescription = contract.ComboLegsDescription,
                ComboLegs = contract.ComboLegs?.Select(x => x.ToIBKRComboLeg()).ToList(),
                DeltaNeutralContract = contract.DeltaNeutralContract?.ToIBKRDeltaNeutralContract()
            };
        }

        internal static ComboLeg ToBrokerComboLeg(this IBApi.ComboLeg comboLeg)
        {
            return new()
            {
                ConId = comboLeg.ConId,
                Ratio = comboLeg.Ratio,
                Action = comboLeg.Action,
                Exchange = comboLeg.Exchange,
                ShortSaleSlot = comboLeg.ShortSaleSlot,
                DesignatedLocation = comboLeg.DesignatedLocation,
                ExemptCode = comboLeg.ExemptCode,
            };
        }

        internal static IBApi.ComboLeg ToIBKRComboLeg(this ComboLeg comboLeg)
        {
            return new()
            {
                ConId = comboLeg.ConId,
                Ratio = comboLeg.Ratio,
                Action = comboLeg.Action,
                Exchange = comboLeg.Exchange,
                ShortSaleSlot = comboLeg.ShortSaleSlot,
                DesignatedLocation = comboLeg.DesignatedLocation,
                ExemptCode = comboLeg.ExemptCode,
            };
        }

        internal static DeltaNeutralContract ToBrokerDeltaNeutralContract(
            this IBApi.DeltaNeutralContract deltaNeutralContract
        )
        {
            return new()
            {
                ConId = deltaNeutralContract.ConId,
                Delta = deltaNeutralContract.Delta,
                Price = deltaNeutralContract.Price,
            };
        }

        internal static IBApi.DeltaNeutralContract ToIBKRDeltaNeutralContract(
            this DeltaNeutralContract deltaNeutralContract
        )
        {
            return new()
            {
                ConId = deltaNeutralContract.ConId,
                Delta = deltaNeutralContract.Delta,
                Price = deltaNeutralContract.Price,
            };
        }

        internal static OptionGreeks ToBrokerOptionGreeks(
            this IBKRWrapper.Models.OptionGreeks greeks
        )
        {
            return new()
            {
                ReqId = greeks.ReqId,
                Field = (StandardTickIds)greeks.Field,
                TickAttribute = (OptionTickAttribute)greeks.TickAttrib,
                ImpliedVolatility = greeks.ImpliedVolatility,
                Delta = greeks.Delta,
                OptionPrice = greeks.OptionPrice,
                PresentValueDividend = greeks.PresentValueDividend,
                Gamma = greeks.Gamma,
                Vega = greeks.Vega,
                Theta = greeks.Theta,
                UnderlyingPrice = greeks.UnderlyingPrice
            };
        }

        internal static IBKRWrapper.Models.OptionGreeks ToIBKROptionGreeks(this OptionGreeks greeks)
        {
            return new(
                greeks.ReqId,
                (int)greeks.Field,
                (int)greeks.TickAttribute,
                greeks.ImpliedVolatility,
                greeks.Delta,
                greeks.OptionPrice,
                greeks.PresentValueDividend,
                greeks.Gamma,
                greeks.Vega,
                greeks.Theta,
                greeks.UnderlyingPrice
            );
        }

        internal static PortfolioPosition ToBrokerPortfolioPosition(
            this IBKRWrapper.Models.PortfolioPosition position
        )
        {
            return new()
            {
                Ticker = position.Ticker,
                SecType = position.SecType,
                Exchange = position.Exchange,
                PrimaryExch = position.PrimaryExch,
                Quantity = position.Quantity,
                MarketPrice = position.MarketPrice,
                MarketValue = position.MarketValue,
                AverageCost = position.AverageCost,
                UnrealizedPNL = position.UnrealizedPNL,
                RealizedPNL = position.RealizedPNL,
                AccountName = position.AccountName
            };
        }

        internal static Position ToBrokerPosition(this IBKRWrapper.Models.Position position)
        {
            return new()
            {
                ConId = position.ConId,
                Symbol = position.Symbol,
                SecType = position.SecType,
                Exchange = position.Exchange,
                Currency = position.Currency,
                PrimaryExch = position.PrimaryExch,
                Positions = position.Positions,
                AvgCost = position.AvgCost,
                Account = position.Account
            };
        }
    }
}
