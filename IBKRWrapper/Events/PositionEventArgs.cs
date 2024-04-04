using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class PositionEventArgs(Position position) : EventArgs
    {
        public Position Position { get; private set; } = position;
    }
}
