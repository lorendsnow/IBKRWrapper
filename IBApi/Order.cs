using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBApi
{
    public class Order
    {
        public const int CUSTOMER = 0;
        public const int FIRM = 1;
        public const char OPT_UNKNOWN = '?';
        public const char OPT_BROKER_DEALER = 'b';
        public const char OPT_CUSTOMER = 'c';
        public const char OPT_FIRM = 'f';
        public const char OPT_ISEMM = 'm';
        public const char OPT_FARMM = 'n';
        public const char OPT_SPECIALIST = 'y';
        public const int AUCTION_MATCH = 1;
        public const int AUCTION_IMPROVEMENT = 2;
        public const int AUCTION_TRANSPARENT = 3;
        public const double COMPETE_AGAINST_BEST_OFFSET_UP_TO_MID = double.PositiveInfinity;

        public int OrderId { get; set; }

        public bool Solicited { get; set; }

        public int ClientId { get; set; }

        public long PermId { get; set; }

        public string Action { get; set; }

        public decimal TotalQuantity { get; set; }

        public string OrderType { get; set; }

        public double LmtPrice { get; set; }

        public double AuxPrice { get; set; }

        public string Tif { get; set; }

        public string OcaGroup { get; set; }

        public int OcaType { get; set; }

        public string OrderRef { get; set; }

        public bool Transmit { get; set; }

        public int ParentId { get; set; }

        public bool BlockOrder { get; set; }

        public bool SweepToFill { get; set; }

        public int DisplaySize { get; set; }

        public int TriggerMethod { get; set; }

        public bool OutsideRth { get; set; }

        public bool Hidden { get; set; }

        public string GoodAfterTime { get; set; }

        public string GoodTillDate { get; set; }

        public bool OverridePercentageConstraints { get; set; }

        public string Rule80A { get; set; }

        public bool AllOrNone { get; set; }

        public int MinQty { get; set; }

        public double PercentOffset { get; set; }

        public double TrailStopPrice { get; set; }

        public double TrailingPercent { get; set; }

        public string FaGroup { get; set; }

        public string FaMethod { get; set; }

        public string FaPercentage { get; set; }

        public string OpenClose { get; set; }

        public int Origin { get; set; }

        public int ShortSaleSlot { get; set; }

        public string DesignatedLocation { get; set; }

        public int ExemptCode { get; set; }

        public double DiscretionaryAmt { get; set; }

        public bool OptOutSmartRouting { get; set; }

        public int AuctionStrategy { get; set; }

        public double StartingPrice { get; set; }

        public double StockRefPrice { get; set; }

        public double Delta { get; set; }

        public double StockRangeLower { get; set; }

        public double StockRangeUpper { get; set; }

        public double Volatility { get; set; }

        public int VolatilityType { get; set; }

        public int ContinuousUpdate { get; set; }

        public int ReferencePriceType { get; set; }

        public string DeltaNeutralOrderType { get; set; }

        public double DeltaNeutralAuxPrice { get; set; }

        public int DeltaNeutralConId { get; set; }

        public string DeltaNeutralSettlingFirm { get; set; }

        public string DeltaNeutralClearingAccount { get; set; }

        public string DeltaNeutralClearingIntent { get; set; }

        public string DeltaNeutralOpenClose { get; set; }

        public bool DeltaNeutralShortSale { get; set; }

        public int DeltaNeutralShortSaleSlot { get; set; }

        public string DeltaNeutralDesignatedLocation { get; set; }

        public double BasisPoints { get; set; }

        public int BasisPointsType { get; set; }

        public int ScaleInitLevelSize { get; set; }

        public int ScaleSubsLevelSize { get; set; }

        public double ScalePriceIncrement { get; set; }

        public double ScalePriceAdjustValue { get; set; }

        public int ScalePriceAdjustInterval { get; set; }

        public double ScaleProfitOffset { get; set; }

        public bool ScaleAutoReset { get; set; }

        public int ScaleInitPosition { get; set; }

        public int ScaleInitFillQty { get; set; }

        public bool ScaleRandomPercent { get; set; }

        public string HedgeType { get; set; }

        public string HedgeParam { get; set; }

        public string Account { get; set; }

        public string SettlingFirm { get; set; }

        public string ClearingAccount { get; set; }

        public string ClearingIntent { get; set; }

        public string AlgoStrategy { get; set; }

        public List<TagValue> AlgoParams { get; set; }

        public bool WhatIf { get; set; }

        public string AlgoId { get; set; }

        public bool NotHeld { get; set; }

        public List<TagValue> SmartComboRoutingParams { get; set; }

        public List<OrderComboLeg> OrderComboLegs { get; set; } = new List<OrderComboLeg>();

        public List<TagValue> OrderMiscOptions { get; set; } = new List<TagValue>();

        public string ActiveStartTime { get; set; }

        public string ActiveStopTime { get; set; }

        public string ScaleTable { get; set; }

        public string ModelCode { get; set; }

        public string ExtOperator { get; set; }

        public double CashQty { get; set; }

        public string Mifid2DecisionMaker { get; set; }

        public string Mifid2DecisionAlgo { get; set; }

        public string Mifid2ExecutionTrader { get; set; }

        public string Mifid2ExecutionAlgo { get; set; }

        public bool DontUseAutoPriceForHedge { get; set; }

        public string AutoCancelDate { get; set; }

        public decimal FilledQuantity { get; set; }

        public int RefFuturesConId { get; set; }

        public bool AutoCancelParent { get; set; }

        public string Shareholder { get; set; }

        public bool ImbalanceOnly { get; set; }

        public bool RouteMarketableToBbo { get; set; }

        public long ParentPermId { get; set; }

        public string AdvancedErrorOverride { get; set; }

        public string ManualOrderTime { get; set; }

        public int MinTradeQty { get; set; }

        public int MinCompeteSize { get; set; }

        public double CompeteAgainstBestOffset { get; set; }

        public double MidOffsetAtWhole { get; set; }

        public double MidOffsetAtHalf { get; set; }

        public string CustomerAccount { get; set; }

        public bool ProfessionalCustomer { get; set; }

        public string BondAccruedInterest { get; set; }

        public bool IncludeOvernight { get; set; }

        public int ManualOrderIndicator { get; set; }

        public string Submitter { get; set; }

        public Order()
        {
            Action = string.Empty;
            OrderType = string.Empty;
            Tif = string.Empty;
            OcaGroup = string.Empty;
            OrderRef = string.Empty;
            GoodAfterTime = string.Empty;
            GoodTillDate = string.Empty;
            Rule80A = string.Empty;
            FaGroup = string.Empty;
            FaMethod = string.Empty;
            FaPercentage = string.Empty;
            HedgeType = string.Empty;
            HedgeParam = string.Empty;
            Account = string.Empty;
            SettlingFirm = string.Empty;
            ClearingAccount = string.Empty;
            ClearingIntent = string.Empty;
            AlgoStrategy = string.Empty;
            AlgoId = string.Empty;
            AlgoParams = [];
            SmartComboRoutingParams = [];
            ModelCode = string.Empty;
            ReferenceExchange = string.Empty;
            AdjustedOrderType = string.Empty;
            LmtPrice = double.MaxValue;
            AuxPrice = double.MaxValue;
            ActiveStartTime = string.Empty;
            ActiveStopTime = string.Empty;
            OutsideRth = false;
            OpenClose = string.Empty;
            Origin = CUSTOMER;
            Transmit = true;
            DesignatedLocation = string.Empty;
            ExemptCode = -1;
            MinQty = int.MaxValue;
            PercentOffset = double.MaxValue;
            OptOutSmartRouting = false;
            StartingPrice = double.MaxValue;
            StockRefPrice = double.MaxValue;
            Delta = double.MaxValue;
            StockRangeLower = double.MaxValue;
            StockRangeUpper = double.MaxValue;
            Volatility = double.MaxValue;
            VolatilityType = int.MaxValue;
            DeltaNeutralOrderType = string.Empty;
            DeltaNeutralAuxPrice = double.MaxValue;
            DeltaNeutralConId = 0;
            DeltaNeutralSettlingFirm = string.Empty;
            DeltaNeutralClearingAccount = string.Empty;
            DeltaNeutralClearingIntent = string.Empty;
            DeltaNeutralOpenClose = string.Empty;
            DeltaNeutralShortSale = false;
            DeltaNeutralShortSaleSlot = 0;
            DeltaNeutralDesignatedLocation = string.Empty;
            ReferencePriceType = int.MaxValue;
            TrailStopPrice = double.MaxValue;
            TrailingPercent = double.MaxValue;
            BasisPoints = double.MaxValue;
            BasisPointsType = int.MaxValue;
            ScaleInitLevelSize = int.MaxValue;
            ScaleSubsLevelSize = int.MaxValue;
            ScalePriceIncrement = double.MaxValue;
            ScalePriceAdjustValue = double.MaxValue;
            ScalePriceAdjustInterval = int.MaxValue;
            ScaleProfitOffset = double.MaxValue;
            ScaleAutoReset = false;
            ScaleInitPosition = int.MaxValue;
            ScaleInitFillQty = int.MaxValue;
            ScaleRandomPercent = false;
            ScaleTable = string.Empty;
            WhatIf = false;
            NotHeld = false;
            Conditions = new List<OrderCondition>();
            TriggerPrice = double.MaxValue;
            LmtPriceOffset = double.MaxValue;
            AdjustedStopPrice = double.MaxValue;
            AdjustedStopLimitPrice = double.MaxValue;
            AdjustedTrailingAmount = double.MaxValue;
            ExtOperator = string.Empty;
            Tier = new SoftDollarTier(string.Empty, string.Empty, string.Empty);
            CashQty = double.MaxValue;
            Mifid2DecisionMaker = string.Empty;
            Mifid2DecisionAlgo = string.Empty;
            Mifid2ExecutionTrader = string.Empty;
            Mifid2ExecutionAlgo = string.Empty;
            DontUseAutoPriceForHedge = false;
            AutoCancelDate = string.Empty;
            FilledQuantity = decimal.MaxValue;
            RefFuturesConId = int.MaxValue;
            AutoCancelParent = false;
            Shareholder = string.Empty;
            ImbalanceOnly = false;
            RouteMarketableToBbo = false;
            ParentPermId = long.MaxValue;
            UsePriceMgmtAlgo = null;
            Duration = int.MaxValue;
            PostToAts = int.MaxValue;
            AdvancedErrorOverride = string.Empty;
            ManualOrderTime = string.Empty;
            MinTradeQty = int.MaxValue;
            MinCompeteSize = int.MaxValue;
            CompeteAgainstBestOffset = double.MaxValue;
            MidOffsetAtWhole = double.MaxValue;
            MidOffsetAtHalf = double.MaxValue;
            CustomerAccount = string.Empty;
            ProfessionalCustomer = false;
            BondAccruedInterest = string.Empty;
            IncludeOvernight = false;
            ManualOrderIndicator = int.MaxValue;
            Submitter = string.Empty;
        }

        // Note: Two orders can be 'equivalent' even if all fields do not match. This function is
        // not intended to be used with Order objects returned from TWS.
        public override bool Equals(object? p_other)
        {
            if (this == p_other)
                return true;

            if (p_other is not Order l_theOther)
                return false;

            if (PermId == l_theOther.PermId)
            {
                return true;
            }

            if (
                OrderId != l_theOther.OrderId
                || ClientId != l_theOther.ClientId
                || TotalQuantity != l_theOther.TotalQuantity
                || LmtPrice != l_theOther.LmtPrice
                || AuxPrice != l_theOther.AuxPrice
                || OcaType != l_theOther.OcaType
                || Transmit != l_theOther.Transmit
                || ParentId != l_theOther.ParentId
                || BlockOrder != l_theOther.BlockOrder
                || SweepToFill != l_theOther.SweepToFill
                || DisplaySize != l_theOther.DisplaySize
                || TriggerMethod != l_theOther.TriggerMethod
                || OutsideRth != l_theOther.OutsideRth
                || Hidden != l_theOther.Hidden
                || OverridePercentageConstraints != l_theOther.OverridePercentageConstraints
                || AllOrNone != l_theOther.AllOrNone
                || MinQty != l_theOther.MinQty
                || PercentOffset != l_theOther.PercentOffset
                || TrailStopPrice != l_theOther.TrailStopPrice
                || TrailingPercent != l_theOther.TrailingPercent
                || Origin != l_theOther.Origin
                || ShortSaleSlot != l_theOther.ShortSaleSlot
                || DiscretionaryAmt != l_theOther.DiscretionaryAmt
                || OptOutSmartRouting != l_theOther.OptOutSmartRouting
                || AuctionStrategy != l_theOther.AuctionStrategy
                || StartingPrice != l_theOther.StartingPrice
                || StockRefPrice != l_theOther.StockRefPrice
                || Delta != l_theOther.Delta
                || StockRangeLower != l_theOther.StockRangeLower
                || StockRangeUpper != l_theOther.StockRangeUpper
                || Volatility != l_theOther.Volatility
                || VolatilityType != l_theOther.VolatilityType
                || ContinuousUpdate != l_theOther.ContinuousUpdate
                || ReferencePriceType != l_theOther.ReferencePriceType
                || DeltaNeutralAuxPrice != l_theOther.DeltaNeutralAuxPrice
                || DeltaNeutralConId != l_theOther.DeltaNeutralConId
                || DeltaNeutralShortSale != l_theOther.DeltaNeutralShortSale
                || DeltaNeutralShortSaleSlot != l_theOther.DeltaNeutralShortSaleSlot
                || BasisPoints != l_theOther.BasisPoints
                || BasisPointsType != l_theOther.BasisPointsType
                || ScaleInitLevelSize != l_theOther.ScaleInitLevelSize
                || ScaleSubsLevelSize != l_theOther.ScaleSubsLevelSize
                || ScalePriceIncrement != l_theOther.ScalePriceIncrement
                || ScalePriceAdjustValue != l_theOther.ScalePriceAdjustValue
                || ScalePriceAdjustInterval != l_theOther.ScalePriceAdjustInterval
                || ScaleProfitOffset != l_theOther.ScaleProfitOffset
                || ScaleAutoReset != l_theOther.ScaleAutoReset
                || ScaleInitPosition != l_theOther.ScaleInitPosition
                || ScaleInitFillQty != l_theOther.ScaleInitFillQty
                || ScaleRandomPercent != l_theOther.ScaleRandomPercent
                || WhatIf != l_theOther.WhatIf
                || NotHeld != l_theOther.NotHeld
                || ExemptCode != l_theOther.ExemptCode
                || RandomizePrice != l_theOther.RandomizePrice
                || RandomizeSize != l_theOther.RandomizeSize
                || Solicited != l_theOther.Solicited
                || ConditionsIgnoreRth != l_theOther.ConditionsIgnoreRth
                || ConditionsCancelOrder != l_theOther.ConditionsCancelOrder
                || Tier != l_theOther.Tier
                || CashQty != l_theOther.CashQty
                || DontUseAutoPriceForHedge != l_theOther.DontUseAutoPriceForHedge
                || IsOmsContainer != l_theOther.IsOmsContainer
                || UsePriceMgmtAlgo != l_theOther.UsePriceMgmtAlgo
                || FilledQuantity != l_theOther.FilledQuantity
                || RefFuturesConId != l_theOther.RefFuturesConId
                || AutoCancelParent != l_theOther.AutoCancelParent
                || ImbalanceOnly != l_theOther.ImbalanceOnly
                || RouteMarketableToBbo != l_theOther.RouteMarketableToBbo
                || ParentPermId != l_theOther.ParentPermId
                || Duration != l_theOther.Duration
                || PostToAts != l_theOther.PostToAts
                || MinTradeQty != l_theOther.MinTradeQty
                || MinCompeteSize != l_theOther.MinCompeteSize
                || CompeteAgainstBestOffset != l_theOther.CompeteAgainstBestOffset
                || MidOffsetAtWhole != l_theOther.MidOffsetAtWhole
                || MidOffsetAtHalf != l_theOther.MidOffsetAtHalf
                || ProfessionalCustomer != l_theOther.ProfessionalCustomer
                || IncludeOvernight != l_theOther.IncludeOvernight
                || ManualOrderIndicator != l_theOther.ManualOrderIndicator
            )
            {
                return false;
            }

            if (
                Util.StringCompare(Action, l_theOther.Action) != 0
                || Util.StringCompare(OrderType, l_theOther.OrderType) != 0
                || Util.StringCompare(Tif, l_theOther.Tif) != 0
                || Util.StringCompare(ActiveStartTime, l_theOther.ActiveStartTime) != 0
                || Util.StringCompare(ActiveStopTime, l_theOther.ActiveStopTime) != 0
                || Util.StringCompare(OcaGroup, l_theOther.OcaGroup) != 0
                || Util.StringCompare(OrderRef, l_theOther.OrderRef) != 0
                || Util.StringCompare(GoodAfterTime, l_theOther.GoodAfterTime) != 0
                || Util.StringCompare(GoodTillDate, l_theOther.GoodTillDate) != 0
                || Util.StringCompare(Rule80A, l_theOther.Rule80A) != 0
                || Util.StringCompare(FaGroup, l_theOther.FaGroup) != 0
                || Util.StringCompare(FaMethod, l_theOther.FaMethod) != 0
                || Util.StringCompare(FaPercentage, l_theOther.FaPercentage) != 0
                || Util.StringCompare(OpenClose, l_theOther.OpenClose) != 0
                || Util.StringCompare(DesignatedLocation, l_theOther.DesignatedLocation) != 0
                || Util.StringCompare(DeltaNeutralOrderType, l_theOther.DeltaNeutralOrderType) != 0
                || Util.StringCompare(DeltaNeutralSettlingFirm, l_theOther.DeltaNeutralSettlingFirm)
                    != 0
                || Util.StringCompare(
                    DeltaNeutralClearingAccount,
                    l_theOther.DeltaNeutralClearingAccount
                ) != 0
                || Util.StringCompare(
                    DeltaNeutralClearingIntent,
                    l_theOther.DeltaNeutralClearingIntent
                ) != 0
                || Util.StringCompare(DeltaNeutralOpenClose, l_theOther.DeltaNeutralOpenClose) != 0
                || Util.StringCompare(
                    DeltaNeutralDesignatedLocation,
                    l_theOther.DeltaNeutralDesignatedLocation
                ) != 0
                || Util.StringCompare(HedgeType, l_theOther.HedgeType) != 0
                || Util.StringCompare(HedgeParam, l_theOther.HedgeParam) != 0
                || Util.StringCompare(Account, l_theOther.Account) != 0
                || Util.StringCompare(SettlingFirm, l_theOther.SettlingFirm) != 0
                || Util.StringCompare(ClearingAccount, l_theOther.ClearingAccount) != 0
                || Util.StringCompare(ClearingIntent, l_theOther.ClearingIntent) != 0
                || Util.StringCompare(AlgoStrategy, l_theOther.AlgoStrategy) != 0
                || Util.StringCompare(AlgoId, l_theOther.AlgoId) != 0
                || Util.StringCompare(ScaleTable, l_theOther.ScaleTable) != 0
                || Util.StringCompare(ModelCode, l_theOther.ModelCode) != 0
                || Util.StringCompare(ExtOperator, l_theOther.ExtOperator) != 0
                || Util.StringCompare(AutoCancelDate, l_theOther.AutoCancelDate) != 0
                || Util.StringCompare(Shareholder, l_theOther.Shareholder) != 0
                || Util.StringCompare(AdvancedErrorOverride, l_theOther.AdvancedErrorOverride) != 0
                || Util.StringCompare(ManualOrderTime, l_theOther.ManualOrderTime) != 0
                || Util.StringCompare(CustomerAccount, l_theOther.CustomerAccount) != 0
                || Util.StringCompare(Submitter, l_theOther.Submitter) != 0
                || Util.StringCompare(BondAccruedInterest, l_theOther.BondAccruedInterest) != 0
            )
            {
                return false;
            }

            if (!Util.VectorEqualsUnordered(AlgoParams, l_theOther.AlgoParams))
            {
                return false;
            }

            if (
                !Util.VectorEqualsUnordered(
                    SmartComboRoutingParams,
                    l_theOther.SmartComboRoutingParams
                )
            )
            {
                return false;
            }

            // compare order combo legs
            if (!Util.VectorEqualsUnordered(OrderComboLegs, l_theOther.OrderComboLegs))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 1040337091;
            hashCode *= -1521134295 + OrderId.GetHashCode();
            hashCode *= -1521134295 + Solicited.GetHashCode();
            hashCode *= -1521134295 + ClientId.GetHashCode();
            hashCode *= -1521134295 + PermId.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Action);
            hashCode *= -1521134295 + TotalQuantity.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(OrderType);
            hashCode *= -1521134295 + LmtPrice.GetHashCode();
            hashCode *= -1521134295 + AuxPrice.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tif);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(OcaGroup);
            hashCode *= -1521134295 + OcaType.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(OrderRef);
            hashCode *= -1521134295 + Transmit.GetHashCode();
            hashCode *= -1521134295 + ParentId.GetHashCode();
            hashCode *= -1521134295 + BlockOrder.GetHashCode();
            hashCode *= -1521134295 + SweepToFill.GetHashCode();
            hashCode *= -1521134295 + DisplaySize.GetHashCode();
            hashCode *= -1521134295 + TriggerMethod.GetHashCode();
            hashCode *= -1521134295 + OutsideRth.GetHashCode();
            hashCode *= -1521134295 + Hidden.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(GoodAfterTime);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(GoodTillDate);
            hashCode *= -1521134295 + OverridePercentageConstraints.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Rule80A);
            hashCode *= -1521134295 + AllOrNone.GetHashCode();
            hashCode *= -1521134295 + MinQty.GetHashCode();
            hashCode *= -1521134295 + PercentOffset.GetHashCode();
            hashCode *= -1521134295 + TrailStopPrice.GetHashCode();
            hashCode *= -1521134295 + TrailingPercent.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaGroup);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaMethod);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaPercentage);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(OpenClose);
            hashCode *= -1521134295 + Origin.GetHashCode();
            hashCode *= -1521134295 + ShortSaleSlot.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(DesignatedLocation);
            hashCode *= -1521134295 + ExemptCode.GetHashCode();
            hashCode *= -1521134295 + DiscretionaryAmt.GetHashCode();
            hashCode *= -1521134295 + OptOutSmartRouting.GetHashCode();
            hashCode *= -1521134295 + AuctionStrategy.GetHashCode();
            hashCode *= -1521134295 + StartingPrice.GetHashCode();
            hashCode *= -1521134295 + StockRefPrice.GetHashCode();
            hashCode *= -1521134295 + Delta.GetHashCode();
            hashCode *= -1521134295 + StockRangeLower.GetHashCode();
            hashCode *= -1521134295 + StockRangeUpper.GetHashCode();
            hashCode *= -1521134295 + Volatility.GetHashCode();
            hashCode *= -1521134295 + VolatilityType.GetHashCode();
            hashCode *= -1521134295 + ContinuousUpdate.GetHashCode();
            hashCode *= -1521134295 + ReferencePriceType.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralOrderType);
            hashCode *= -1521134295 + DeltaNeutralAuxPrice.GetHashCode();
            hashCode *= -1521134295 + DeltaNeutralConId.GetHashCode();
            hashCode *=
                -1521134295
                + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralSettlingFirm);
            hashCode *=
                -1521134295
                + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralClearingAccount);
            hashCode *=
                -1521134295
                + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralClearingIntent);
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralOpenClose);
            hashCode *= -1521134295 + DeltaNeutralShortSale.GetHashCode();
            hashCode *= -1521134295 + DeltaNeutralShortSaleSlot.GetHashCode();
            hashCode *=
                -1521134295
                + EqualityComparer<string>.Default.GetHashCode(DeltaNeutralDesignatedLocation);
            hashCode *= -1521134295 + BasisPoints.GetHashCode();
            hashCode *= -1521134295 + BasisPointsType.GetHashCode();
            hashCode *= -1521134295 + ScaleInitLevelSize.GetHashCode();
            hashCode *= -1521134295 + ScaleSubsLevelSize.GetHashCode();
            hashCode *= -1521134295 + ScalePriceIncrement.GetHashCode();
            hashCode *= -1521134295 + ScalePriceAdjustValue.GetHashCode();
            hashCode *= -1521134295 + ScalePriceAdjustInterval.GetHashCode();
            hashCode *= -1521134295 + ScaleProfitOffset.GetHashCode();
            hashCode *= -1521134295 + ScaleAutoReset.GetHashCode();
            hashCode *= -1521134295 + ScaleInitPosition.GetHashCode();
            hashCode *= -1521134295 + ScaleInitFillQty.GetHashCode();
            hashCode *= -1521134295 + ScaleRandomPercent.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(HedgeType);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(HedgeParam);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Account);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(SettlingFirm);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClearingAccount);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClearingIntent);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(AlgoStrategy);
            hashCode *=
                -1521134295 + EqualityComparer<List<TagValue>>.Default.GetHashCode(AlgoParams);
            hashCode *= -1521134295 + WhatIf.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(AlgoId);
            hashCode *= -1521134295 + NotHeld.GetHashCode();
            hashCode *=
                -1521134295
                + EqualityComparer<List<TagValue>>.Default.GetHashCode(SmartComboRoutingParams);
            hashCode *=
                -1521134295
                + EqualityComparer<List<OrderComboLeg>>.Default.GetHashCode(OrderComboLegs);
            hashCode *=
                -1521134295
                + EqualityComparer<List<TagValue>>.Default.GetHashCode(OrderMiscOptions);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ActiveStartTime);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ActiveStopTime);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ScaleTable);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModelCode);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ExtOperator);
            hashCode *= -1521134295 + CashQty.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mifid2DecisionMaker);
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mifid2DecisionAlgo);
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mifid2ExecutionTrader);
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mifid2ExecutionAlgo);
            hashCode *= -1521134295 + DontUseAutoPriceForHedge.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(AutoCancelDate);
            hashCode *= -1521134295 + FilledQuantity.GetHashCode();
            hashCode *= -1521134295 + RefFuturesConId.GetHashCode();
            hashCode *= -1521134295 + AutoCancelParent.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Shareholder);
            hashCode *= -1521134295 + ImbalanceOnly.GetHashCode();
            hashCode *= -1521134295 + RouteMarketableToBbo.GetHashCode();
            hashCode *= -1521134295 + ParentPermId.GetHashCode();
            hashCode *= -1521134295 + RandomizeSize.GetHashCode();
            hashCode *= -1521134295 + RandomizePrice.GetHashCode();
            hashCode *= -1521134295 + ReferenceContractId.GetHashCode();
            hashCode *= -1521134295 + IsPeggedChangeAmountDecrease.GetHashCode();
            hashCode *= -1521134295 + PeggedChangeAmount.GetHashCode();
            hashCode *= -1521134295 + ReferenceChangeAmount.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(ReferenceExchange);
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(AdjustedOrderType);
            hashCode *= -1521134295 + TriggerPrice.GetHashCode();
            hashCode *= -1521134295 + LmtPriceOffset.GetHashCode();
            hashCode *= -1521134295 + AdjustedStopPrice.GetHashCode();
            hashCode *= -1521134295 + AdjustedStopLimitPrice.GetHashCode();
            hashCode *= -1521134295 + AdjustedTrailingAmount.GetHashCode();
            hashCode *= -1521134295 + AdjustableTrailingUnit.GetHashCode();
            hashCode *=
                -1521134295
                + EqualityComparer<List<OrderCondition>>.Default.GetHashCode(Conditions);
            hashCode *= -1521134295 + ConditionsIgnoreRth.GetHashCode();
            hashCode *= -1521134295 + ConditionsCancelOrder.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<SoftDollarTier>.Default.GetHashCode(Tier);
            hashCode *= -1521134295 + IsOmsContainer.GetHashCode();
            hashCode *= -1521134295 + DiscretionaryUpToLimitPrice.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(UsePriceMgmtAlgo);
            hashCode *= -1521134295 + Duration.GetHashCode();
            hashCode *= -1521134295 + PostToAts.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(AdvancedErrorOverride);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ManualOrderTime);
            hashCode *= -1521134295 + MinTradeQty.GetHashCode();
            hashCode *= -1521134295 + MinCompeteSize.GetHashCode();
            hashCode *= -1521134295 + CompeteAgainstBestOffset.GetHashCode();
            hashCode *= -1521134295 + MidOffsetAtWhole.GetHashCode();
            hashCode *= -1521134295 + MidOffsetAtHalf.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(CustomerAccount);
            hashCode *= -1521134295 + ProfessionalCustomer.GetHashCode();
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(BondAccruedInterest);
            hashCode *= -1521134295 + IncludeOvernight.GetHashCode();
            hashCode *= -1521134295 + ManualOrderIndicator.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Submitter);

            return hashCode;
        }

        public bool RandomizeSize { get; set; }

        public bool RandomizePrice { get; set; }

        public int ReferenceContractId { get; set; }

        public bool IsPeggedChangeAmountDecrease { get; set; }

        public double PeggedChangeAmount { get; set; }

        public double ReferenceChangeAmount { get; set; }

        public string ReferenceExchange { get; set; }

        public string AdjustedOrderType { get; set; }

        public double TriggerPrice { get; set; }

        public double LmtPriceOffset { get; set; }

        public double AdjustedStopPrice { get; set; }

        public double AdjustedStopLimitPrice { get; set; }

        public double AdjustedTrailingAmount { get; set; }

        public int AdjustableTrailingUnit { get; set; }

        public List<OrderCondition> Conditions { get; set; }

        public bool ConditionsIgnoreRth { get; set; }

        public bool ConditionsCancelOrder { get; set; }

        public SoftDollarTier Tier { get; set; }

        public bool IsOmsContainer { get; set; }

        public bool DiscretionaryUpToLimitPrice { get; set; }

        public bool? UsePriceMgmtAlgo { get; set; }

        public int Duration { get; set; }

        public int PostToAts { get; set; }
    }
}
