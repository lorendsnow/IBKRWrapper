namespace SimpleBroker
{
    /// <summary>
    /// Holds a newly received tick.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of tick contained in the args (e.g., last, bid/ask, mid)
    /// </typeparam>
    /// <param name="tick">The tick data object</param>
    public class NewTickEventArgs<T>(T tick) : EventArgs
    {
        /// <summary>
        /// The tick data object
        /// </summary>
        public T Tick { get; } = tick;
    }
}
