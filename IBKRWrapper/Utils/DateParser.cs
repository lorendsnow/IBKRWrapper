namespace IBKRWrapper.Utils
{
    public static class IbDateParser
    {
        public static DateTimeOffset ParseIBDateTime(string timeString)
        {
            DateTimeOffset dtResult;
            char[] chars = timeString.ToCharArray();
            switch (
                (
                    timeString.Length,
                    int.TryParse(timeString, out int i),
                    chars.Where(x => x.ToString() == " ").Count() >= 2,
                    chars.All(x => x.ToString() != "  ")
                )
            )
            {
                case (8, _, _, _):
                    dtResult = new DateTimeOffset(
                        ParseCompressedString(timeString),
                        TimeZoneInfo.Local.BaseUtcOffset
                    );
                    break;
                case (_, true, _, _):
                    dtResult = DateTimeOffset
                        .FromUnixTimeSeconds(int.Parse(timeString))
                        .ToLocalTime();
                    break;
                case (_, _, true, true):
                    string[] split = timeString.Split(" ");
                    DateTime date = ParseCompressedString(split[0]);
                    TimeOnly time = ParseTimeString(split[1]);
                    dtResult = new DateTimeOffset(
                        DateOnly.FromDateTime(date),
                        time,
                        TimeZoneInfo.Local.BaseUtcOffset
                    );
                    break;
                default:
                    DateTimeOffset j;
                    if (!DateTimeOffset.TryParse(timeString, out j))
                    {
                        DateTime dateParsed = ParseCompressedString(timeString[0..8]);
                        TimeOnly timeParsed = ParseTimeString(timeString[10..]);
                        dtResult = new DateTimeOffset(
                            DateOnly.FromDateTime(dateParsed),
                            timeParsed,
                            TimeZoneInfo.Local.BaseUtcOffset
                        );
                    }
                    else
                    {
                        if (DateTime.TryParse(timeString, out DateTime raw))
                        {
                            dtResult = new DateTimeOffset(
                                raw.ToLocalTime(),
                                TimeZoneInfo.Local.BaseUtcOffset
                            );
                        }
                        else
                        {
                            dtResult = new DateTimeOffset();
                        }
                    }
                    break;
            }
            return dtResult;
        }

        private static DateTime ParseCompressedString(string str)
        {
            int y = int.Parse(str[0..4]);
            int m = int.Parse(str[4..6]);
            int d = int.Parse(str[6..]);
            return new DateTime(y, m, d);
        }

        private static TimeOnly ParseTimeString(string str)
        {
            if (TimeOnly.TryParse(str, out TimeOnly result))
                return result;
            else
            {
                return new TimeOnly();
            }
        }
    }
}
