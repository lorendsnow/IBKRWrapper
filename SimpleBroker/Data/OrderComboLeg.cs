namespace SimpleBroker
{
    /// <summary>
    /// Allows a user to specify a price on an order's leg
    /// </summary>
    public class OrderComboLeg(double price)
    {
        /// <summary>
        /// The price of the order leg
        /// </summary>
        public double Price { get; set; } = price;
    }
}
