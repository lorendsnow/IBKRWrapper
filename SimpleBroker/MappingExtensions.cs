using IBKRWrapper.Utils;

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

        internal static Bar ToBrokerBar(this IBApi.Bar bar, string symbol)
        {
            return new()
            {
                Symbol = symbol,
                Time = IbDateParser.ParseIBDateTime(bar.Time),
                Open = bar.Open,
                High = bar.High,
                Low = bar.Low,
                Close = bar.Close,
                Volume = bar.Volume,
                WAP = bar.WAP,
                Count = bar.Count
            };
        }

        internal static HistoricalTickLast ToBrokerHistoricalTickLast(
            this IBApi.HistoricalTickLast tick,
            string symbol
        )
        {
            return new()
            {
                Symbol = symbol,
                Time = IbDateParser.ParseIBDateTime(tick.Time.ToString()),
                Price = tick.Price,
                Size = tick.Size,
                Exchange = tick.Exchange is null ? string.Empty : tick.Exchange,
                SpecialConditions = tick.SpecialConditions is null
                    ? string.Empty
                    : tick.SpecialConditions
            };
        }

        internal static HistoricalTickBidAsk ToBrokerHistoricalTickBidAsk(
            this IBApi.HistoricalTickBidAsk tick,
            string symbol
        )
        {
            return new()
            {
                Symbol = symbol,
                Time = IbDateParser.ParseIBDateTime(tick.Time.ToString()),
                PriceBid = tick.PriceBid,
                PriceAsk = tick.PriceAsk,
                SizeBid = tick.SizeBid,
                SizeAsk = tick.SizeAsk
            };
        }

        internal static HistoricalTick ToBrokerHistoricalTick(
            this IBApi.HistoricalTick tick,
            string symbol
        )
        {
            return new()
            {
                Symbol = symbol,
                Time = IbDateParser.ParseIBDateTime(tick.Time.ToString()),
                Price = tick.Price,
                Size = tick.Size
            };
        }

        internal static IBApi.Order ToIBKROrder(this OrderBase order)
        {
            return new()
            {
                OrderId = order.OrderId,
                Solicited = order.Solicited,
                ClientId = order.ClientId,
                PermId = order.PermId,
                Action = order.Action,
                TotalQuantity = order.TotalQuantity,
                OrderType = order.OrderType,
                LmtPrice = order.LmtPrice,
                AuxPrice = order.AuxPrice,
                Tif = order.Tif,
                OcaGroup = order.OcaGroup,
                OcaType = order.OcaType,
                OrderRef = order.OrderRef,
                Transmit = order.Transmit,
                ParentId = order.ParentId,
                BlockOrder = order.BlockOrder,
                SweepToFill = order.SweepToFill,
                DisplaySize = order.DisplaySize,
                TriggerMethod = order.TriggerMethod,
                OutsideRth = order.OutsideRth,
                Hidden = order.Hidden,
                GoodAfterTime = order.GoodAfterTime,
                GoodTillDate = order.GoodTillDate,
                OverridePercentageConstraints = order.OverridePercentageConstraints,
                Rule80A = order.Rule80A,
                AllOrNone = order.AllOrNone,
                MinQty = order.MinQty,
                PercentOffset = order.PercentOffset,
                TrailStopPrice = order.TrailStopPrice,
                TrailingPercent = order.TrailingPercent,
                FaGroup = order.FaGroup,
                FaMethod = order.FaMethod,
                FaPercentage = order.FaPercentage,
                OpenClose = order.OpenClose,
                Origin = order.Origin,
                ShortSaleSlot = order.ShortSaleSlot,
                DesignatedLocation = order.DesignatedLocation,
                ExemptCode = order.ExemptCode,
                DiscretionaryAmt = order.DiscretionaryAmt,
                OptOutSmartRouting = order.OptOutSmartRouting,
                AuctionStrategy = order.AuctionStrategy,
                StartingPrice = order.StartingPrice,
                StockRefPrice = order.StockRefPrice,
                Delta = order.Delta,
                StockRangeLower = order.StockRangeLower,
                StockRangeUpper = order.StockRangeUpper,
                Volatility = order.Volatility,
                VolatilityType = order.VolatilityType,
                ContinuousUpdate = order.ContinuousUpdate,
                ReferencePriceType = order.ReferencePriceType,
                DeltaNeutralOrderType = order.DeltaNeutralOrderType,
                DeltaNeutralAuxPrice = order.DeltaNeutralAuxPrice,
                DeltaNeutralConId = order.DeltaNeutralConId,
                DeltaNeutralSettlingFirm = order.DeltaNeutralSettlingFirm,
                DeltaNeutralClearingAccount = order.DeltaNeutralClearingAccount,
                DeltaNeutralClearingIntent = order.DeltaNeutralClearingIntent,
                DeltaNeutralOpenClose = order.DeltaNeutralOpenClose,
                DeltaNeutralShortSale = order.DeltaNeutralShortSale,
                DeltaNeutralShortSaleSlot = order.DeltaNeutralShortSaleSlot,
                DeltaNeutralDesignatedLocation = order.DeltaNeutralDesignatedLocation,
                BasisPoints = order.BasisPoints,
                BasisPointsType = order.BasisPointsType,
                ScaleInitLevelSize = order.ScaleInitLevelSize,
                ScaleSubsLevelSize = order.ScaleSubsLevelSize,
                ScalePriceIncrement = order.ScalePriceIncrement,
                ScalePriceAdjustValue = order.ScalePriceAdjustValue,
                ScalePriceAdjustInterval = order.ScalePriceAdjustInterval,
                ScaleProfitOffset = order.ScaleProfitOffset,
                ScaleAutoReset = order.ScaleAutoReset,
                ScaleInitPosition = order.ScaleInitPosition,
                ScaleInitFillQty = order.ScaleInitFillQty,
                ScaleRandomPercent = order.ScaleRandomPercent,
                HedgeType = order.HedgeType,
                HedgeParam = order.HedgeParam,
                Account = order.Account,
                SettlingFirm = order.SettlingFirm,
                ClearingAccount = order.ClearingAccount,
                ClearingIntent = order.ClearingIntent,
                AlgoStrategy = order.AlgoStrategy,
                AlgoParams = order.AlgoParams?.Select(x => x.ToIBKRTagValue()).ToList(),
                WhatIf = order.WhatIf,
                AlgoId = order.AlgoId,
                NotHeld = order.NotHeld,
                SmartComboRoutingParams = order
                    .SmartComboRoutingParams?.Select(x => x.ToIBKRTagValue())
                    .ToList(),
                OrderComboLegs = order
                    .OrderComboLegs?.Select(x => x.ToIBKROrderComboLeg())
                    .ToList(),
                OrderMiscOptions = order.OrderMiscOptions?.Select(x => x.ToIBKRTagValue()).ToList(),
                ActiveStartTime = order.ActiveStartTime,
                ActiveStopTime = order.ActiveStopTime,
                ScaleTable = order.ScaleTable,
                ModelCode = order.ModelCode,
                ExtOperator = order.ExtOperator,
                CashQty = order.CashQty,
                Mifid2DecisionMaker = order.Mifid2DecisionMaker,
                Mifid2DecisionAlgo = order.Mifid2DecisionAlgo,
                Mifid2ExecutionTrader = order.Mifid2ExecutionTrader,
                Mifid2ExecutionAlgo = order.Mifid2ExecutionAlgo,
                DontUseAutoPriceForHedge = order.DontUseAutoPriceForHedge,
                AutoCancelDate = order.AutoCancelDate,
                FilledQuantity = order.FilledQuantity,
                RefFuturesConId = order.RefFuturesConId,
                AutoCancelParent = order.AutoCancelParent,
                Shareholder = order.Shareholder,
                ImbalanceOnly = order.ImbalanceOnly,
                RouteMarketableToBbo = order.RouteMarketableToBbo,
                ParentPermId = order.ParentPermId,
                AdvancedErrorOverride = order.AdvancedErrorOverride,
                ManualOrderTime = order.ManualOrderTime,
                MinTradeQty = order.MinTradeQty,
                MinCompeteSize = order.MinCompeteSize,
                CompeteAgainstBestOffset = order.CompeteAgainstBestOffset,
                MidOffsetAtWhole = order.MidOffsetAtWhole,
                MidOffsetAtHalf = order.MidOffsetAtHalf,
                RandomizeSize = order.RandomizeSize,
                RandomizePrice = order.RandomizePrice,
                ReferenceContractId = order.ReferenceContractId,
                IsPeggedChangeAmountDecrease = order.IsPeggedChangeAmountDecrease,
                PeggedChangeAmount = order.PeggedChangeAmount,
                ReferenceChangeAmount = order.ReferenceChangeAmount,
                ReferenceExchange = order.ReferenceExchange,
                AdjustedOrderType = order.AdjustedOrderType,
                TriggerPrice = order.TriggerPrice,
                LmtPriceOffset = order.LmtPriceOffset,
                AdjustedStopPrice = order.AdjustedStopPrice,
                AdjustedStopLimitPrice = order.AdjustedStopLimitPrice,
                AdjustedTrailingAmount = order.AdjustedTrailingAmount,
                AdjustableTrailingUnit = order.AdjustableTrailingUnit,
                ConditionsIgnoreRth = order.ConditionsIgnoreRth,
                ConditionsCancelOrder = order.ConditionsCancelOrder,
                IsOmsContainer = order.IsOmsContainer,
                DiscretionaryUpToLimitPrice = order.DiscretionaryUpToLimitPrice,
                UsePriceMgmtAlgo = order.UsePriceMgmtAlgo,
                Duration = order.Duration,
                PostToAts = order.PostToAts,
            };
        }

        internal static OrderBase ToBrokerOrder(this IBApi.Order order)
        {
            return new()
            {
                OrderId = order.OrderId,
                Solicited = order.Solicited,
                ClientId = order.ClientId,
                PermId = order.PermId,
                Action = order.Action,
                TotalQuantity = order.TotalQuantity,
                OrderType = order.OrderType,
                LmtPrice = order.LmtPrice,
                AuxPrice = order.AuxPrice,
                Tif = order.Tif,
                OcaGroup = order.OcaGroup,
                OcaType = order.OcaType,
                OrderRef = order.OrderRef,
                Transmit = order.Transmit,
                ParentId = order.ParentId,
                BlockOrder = order.BlockOrder,
                SweepToFill = order.SweepToFill,
                DisplaySize = order.DisplaySize,
                TriggerMethod = order.TriggerMethod,
                OutsideRth = order.OutsideRth,
                Hidden = order.Hidden,
                GoodAfterTime = order.GoodAfterTime,
                GoodTillDate = order.GoodTillDate,
                OverridePercentageConstraints = order.OverridePercentageConstraints,
                Rule80A = order.Rule80A,
                AllOrNone = order.AllOrNone,
                MinQty = order.MinQty,
                PercentOffset = order.PercentOffset,
                TrailStopPrice = order.TrailStopPrice,
                TrailingPercent = order.TrailingPercent,
                FaGroup = order.FaGroup,
                FaMethod = order.FaMethod,
                FaPercentage = order.FaPercentage,
                OpenClose = order.OpenClose,
                Origin = order.Origin,
                ShortSaleSlot = order.ShortSaleSlot,
                DesignatedLocation = order.DesignatedLocation,
                ExemptCode = order.ExemptCode,
                DiscretionaryAmt = order.DiscretionaryAmt,
                OptOutSmartRouting = order.OptOutSmartRouting,
                AuctionStrategy = order.AuctionStrategy,
                StartingPrice = order.StartingPrice,
                StockRefPrice = order.StockRefPrice,
                Delta = order.Delta,
                StockRangeLower = order.StockRangeLower,
                StockRangeUpper = order.StockRangeUpper,
                Volatility = order.Volatility,
                VolatilityType = order.VolatilityType,
                ContinuousUpdate = order.ContinuousUpdate,
                ReferencePriceType = order.ReferencePriceType,
                DeltaNeutralOrderType = order.DeltaNeutralOrderType,
                DeltaNeutralAuxPrice = order.DeltaNeutralAuxPrice,
                DeltaNeutralConId = order.DeltaNeutralConId,
                DeltaNeutralSettlingFirm = order.DeltaNeutralSettlingFirm,
                DeltaNeutralClearingAccount = order.DeltaNeutralClearingAccount,
                DeltaNeutralClearingIntent = order.DeltaNeutralClearingIntent,
                DeltaNeutralOpenClose = order.DeltaNeutralOpenClose,
                DeltaNeutralShortSale = order.DeltaNeutralShortSale,
                DeltaNeutralShortSaleSlot = order.DeltaNeutralShortSaleSlot,
                DeltaNeutralDesignatedLocation = order.DeltaNeutralDesignatedLocation,
                BasisPoints = order.BasisPoints,
                BasisPointsType = order.BasisPointsType,
                ScaleInitLevelSize = order.ScaleInitLevelSize,
                ScaleSubsLevelSize = order.ScaleSubsLevelSize,
                ScalePriceIncrement = order.ScalePriceIncrement,
                ScalePriceAdjustValue = order.ScalePriceAdjustValue,
                ScalePriceAdjustInterval = order.ScalePriceAdjustInterval,
                ScaleProfitOffset = order.ScaleProfitOffset,
                ScaleAutoReset = order.ScaleAutoReset,
                ScaleInitPosition = order.ScaleInitPosition,
                ScaleInitFillQty = order.ScaleInitFillQty,
                ScaleRandomPercent = order.ScaleRandomPercent,
                HedgeType = order.HedgeType,
                HedgeParam = order.HedgeParam,
                Account = order.Account,
                SettlingFirm = order.SettlingFirm,
                ClearingAccount = order.ClearingAccount,
                ClearingIntent = order.ClearingIntent,
                AlgoStrategy = order.AlgoStrategy,
                AlgoParams = order.AlgoParams?.Select(x => x.ToBrokerTagValue()).ToList(),
                WhatIf = order.WhatIf,
                AlgoId = order.AlgoId,
                NotHeld = order.NotHeld,
                SmartComboRoutingParams = order
                    .SmartComboRoutingParams?.Select(x => x.ToBrokerTagValue())
                    .ToList(),
                OrderComboLegs = order.OrderComboLegs.Select(x => x.ToBrokerComboLeg()).ToList(),
                OrderMiscOptions = order
                    .OrderMiscOptions.Select(x => x.ToBrokerTagValue())
                    .ToList(),
                ActiveStartTime = order.ActiveStartTime,
                ActiveStopTime = order.ActiveStopTime,
                ScaleTable = order.ScaleTable,
                ModelCode = order.ModelCode,
                ExtOperator = order.ExtOperator,
                CashQty = order.CashQty,
                Mifid2DecisionMaker = order.Mifid2DecisionMaker,
                Mifid2DecisionAlgo = order.Mifid2DecisionAlgo,
                Mifid2ExecutionTrader = order.Mifid2ExecutionTrader,
                Mifid2ExecutionAlgo = order.Mifid2ExecutionAlgo,
                DontUseAutoPriceForHedge = order.DontUseAutoPriceForHedge,
                AutoCancelDate = order.AutoCancelDate,
                FilledQuantity = order.FilledQuantity,
                RefFuturesConId = order.RefFuturesConId,
                AutoCancelParent = order.AutoCancelParent,
                Shareholder = order.Shareholder,
                ImbalanceOnly = order.ImbalanceOnly,
                RouteMarketableToBbo = order.RouteMarketableToBbo,
                ParentPermId = order.ParentPermId,
                AdvancedErrorOverride = order.AdvancedErrorOverride,
                ManualOrderTime = order.ManualOrderTime,
                MinTradeQty = order.MinTradeQty,
                MinCompeteSize = order.MinCompeteSize,
                CompeteAgainstBestOffset = order.CompeteAgainstBestOffset,
                MidOffsetAtWhole = order.MidOffsetAtWhole,
                MidOffsetAtHalf = order.MidOffsetAtHalf,
                RandomizeSize = order.RandomizeSize,
                RandomizePrice = order.RandomizePrice,
                ReferenceContractId = order.ReferenceContractId,
                IsPeggedChangeAmountDecrease = order.IsPeggedChangeAmountDecrease,
                PeggedChangeAmount = order.PeggedChangeAmount,
                ReferenceChangeAmount = order.ReferenceChangeAmount,
                ReferenceExchange = order.ReferenceExchange,
                AdjustedOrderType = order.AdjustedOrderType,
                TriggerPrice = order.TriggerPrice,
                LmtPriceOffset = order.LmtPriceOffset,
                AdjustedStopPrice = order.AdjustedStopPrice,
                AdjustedStopLimitPrice = order.AdjustedStopLimitPrice,
                AdjustedTrailingAmount = order.AdjustedTrailingAmount,
                AdjustableTrailingUnit = order.AdjustableTrailingUnit,
                ConditionsIgnoreRth = order.ConditionsIgnoreRth,
                ConditionsCancelOrder = order.ConditionsCancelOrder,
                IsOmsContainer = order.IsOmsContainer,
                DiscretionaryUpToLimitPrice = order.DiscretionaryUpToLimitPrice,
                UsePriceMgmtAlgo = order.UsePriceMgmtAlgo,
                Duration = order.Duration,
                PostToAts = order.PostToAts,
            };
        }

        internal static IBApi.TagValue ToIBKRTagValue(this TagValue tagValue)
        {
            return new() { Tag = tagValue.Tag, Value = tagValue.Value };
        }

        internal static TagValue ToBrokerTagValue(this IBApi.TagValue tv)
        {
            return new(tv.Tag, tv.Value);
        }

        internal static IBApi.OrderComboLeg ToIBKROrderComboLeg(this OrderComboLeg orderComboLeg)
        {
            return new() { Price = orderComboLeg.Price };
        }

        internal static OrderComboLeg ToBrokerComboLeg(this IBApi.OrderComboLeg orderComboLeg)
        {
            return new(orderComboLeg.Price);
        }

        internal static Execution ToBrokerExecution(this IBApi.Execution ex)
        {
            return new()
            {
                OrderId = ex.OrderId,
                ClientId = ex.ClientId,
                ExecId = ex.ExecId,
                Time = ex.Time,
                AcctNumber = ex.AcctNumber,
                Exchange = ex.Exchange,
                Side = ex.Side,
                Shares = ex.Shares,
                Price = ex.Price,
                PermId = ex.PermId,
                Liquidation = ex.Liquidation,
                CumQty = ex.CumQty,
                AvgPrice = ex.AvgPrice,
                OrderRef = ex.OrderRef,
                EvRule = ex.EvRule,
                EvMultiplier = ex.EvMultiplier,
                ModelCode = ex.ModelCode,
                LastLiquidity = ex.LastLiquidity.ToBrokerLiquidity()
            };
        }

        internal static Liquidity ToBrokerLiquidity(this IBApi.Liquidity liq)
        {
            return new(liq.Value);
        }

        internal static CommissionReport ToBrokerCommissionReport(this IBApi.CommissionReport cr)
        {
            return new()
            {
                ExecId = cr.ExecId,
                Commission = cr.Commission,
                Currency = cr.Currency,
                RealizedPNL = cr.RealizedPNL,
                Yield = cr.Yield,
                YieldRedemptionDate = DateOnly.FromDateTime(
                    IbDateParser.ParseIBDateTime(cr.YieldRedemptionDate.ToString()).Date
                )
            };
        }
    }
}
