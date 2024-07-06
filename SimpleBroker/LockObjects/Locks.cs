namespace SimpleBroker.LockObjects
{
    /// <summary>
    /// Lock objects for async operations where a lock is necessary.
    /// </summary>
    internal class Locks
    {
        internal readonly object accountValuesLock = new();
        internal readonly object openOrderLock = new();
        internal readonly object orderIdLock = new();
        internal readonly object positionLock = new();
        internal readonly object reqIdLock = new();
        internal readonly object scannerParamLock = new();
    }
}
