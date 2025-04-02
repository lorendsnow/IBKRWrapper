namespace IBApi.Interfaces
{
    public interface IDecoder
    {
        double ReadDouble();
        double ReadDoubleMax();
        long ReadLong();
        int ReadInt();
        int ReadIntMax();
        bool ReadBoolFromInt();
        string ReadString();
    }
}
