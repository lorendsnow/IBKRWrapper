using System.Globalization;
using System.Text;

namespace IBApi
{
    public static class IBParamsList
    {
        public static void AddParameter(this BinaryWriter source, decimal value) =>
            AddParameter(source, Util.DecimalMaxString(value));

        public static void AddParameter(this BinaryWriter source, byte[] bytes)
        {
            source.Write(bytes);
        }

        public static void AddParameter(
            this BinaryWriter source,
            OutgoingMessages outgoingMessage,
            int serverVersion
        )
        {
            int msgId = (int)outgoingMessage;
            if (serverVersion >= MinServerVer.MIN_SERVER_VER_PROTOBUF)
            {
                msgId = ClientSocket.UseProtoBuf(serverVersion, outgoingMessage)
                    ? msgId + Constants.PROTOBUF_MSG_ID
                    : msgId;
                byte[] bytes = BitConverter.GetBytes(msgId);
                Array.Reverse(bytes);
                source.Write(bytes);
            }
            else
            {
                AddParameter(source, (int)msgId);
            }
        }

        public static void AddParameter(this BinaryWriter source, int value) =>
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));

        public static void AddParameter(this BinaryWriter source, double value) =>
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));

        public static void AddParameter(this BinaryWriter source, bool? value)
        {
            if (value.HasValue)
                AddParameter(source, value.Value ? "1" : "0");
            else
                source.Write(Constants.EOL);
        }

        public static void AddParameter(this BinaryWriter source, string value)
        {
            if (value != null && !IsAsciiPrintable(value))
                throw new ClientException(ClientErrors.INVALID_SYMBOL, value);

            if (value != null)
                source.Write(Encoding.UTF8.GetBytes(value));
            source.Write(Constants.EOL);
        }

        public static void AddParameter(this BinaryWriter source, Contract value)
        {
            source.AddParameter(value.ConId);
            source.AddParameter(value.Symbol);
            source.AddParameter(value.SecType);
            source.AddParameter(value.LastTradeDateOrContractMonth);
            source.AddParameter(value.Strike);
            source.AddParameter(value.Right);
            source.AddParameter(value.Multiplier);
            source.AddParameter(value.Exchange);
            source.AddParameter(value.PrimaryExch);
            source.AddParameter(value.Currency);
            source.AddParameter(value.LocalSymbol);
            source.AddParameter(value.TradingClass);
            source.AddParameter(value.IncludeExpired);
        }

        public static void AddParameter(this BinaryWriter source, List<TagValue> options) =>
            source.AddParameter(Util.TagValueListToString(options));

        public static void AddParameterMax(this BinaryWriter source, double value)
        {
            if (value == double.MaxValue)
                source.Write(Constants.EOL);
            else if (value == double.PositiveInfinity)
                source.AddParameter(Constants.INFINITY_STR);
            else
                source.AddParameter(value);
        }

        public static void AddParameterMax(this BinaryWriter source, int value)
        {
            if (value == int.MaxValue)
                source.Write(Constants.EOL);
            else
                source.AddParameter(value);
        }

        public static bool IsAsciiPrintable(string str)
        {
            if (str == null)
                return false;
            for (var i = 0; i < str.Length; i++)
            {
                if (IsAsciiPrintable(str[i]) == false)
                    return false;
            }
            return true;
        }

        private static bool IsAsciiPrintable(char ch) =>
            (ch >= 32 && ch < 127) || ch == 9 || ch == 10 || ch == 13;
    }
}
