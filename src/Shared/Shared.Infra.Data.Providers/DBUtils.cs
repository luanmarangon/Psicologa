using System;


namespace Shared.Infra.Data.Providers
{
    public static class DBUtils
    {
        public static object ConvertEmptyStringToNull(string value)
        {
            if (value == null || value.Trim() == "")
                return DBNull.Value;
            else return value.Trim();
        }

        public static object ConvertIntZeroToNull(long value)
        {
            if (value == 0)
                return DBNull.Value;
            else return value;
        }

        public static object ConvertDecimalZeroToNull(decimal value)
        {
            if (value == 0)
                return DBNull.Value;
            else return value;
        }

        public static object ConvertDateTimeMinValueToNull(DateTime value)
        {
            if (value == null || value == DateTime.MinValue)
                return DBNull.Value;
            else return value;
        }
    }
}
