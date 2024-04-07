namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class CommandReaderExtensions
{
    public static BasicList<int> GetIntList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<int> output = [];
        while (reader.Read())
        {
            output.Add(ReadIntItem(reader));
        }
        return output;
    }
    private static int ReadIntItem(DbDataReader reader)
    {
        int output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<int?> GetNullableIntList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<int?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableIntItem(reader));
        }
        return output;
    }
    private static int? ReadNullableIntItem(DbDataReader reader)
    {
        int? output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return output;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<string?> GetStringList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<string?> output = [];
        while (reader.Read())
        {
            output.Add(ReadStringItem(reader));
        }
        return output;
    }
    private static string? ReadStringItem(DbDataReader reader)
    {
        string? output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetString(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<bool> GetBoolList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<bool> output = [];
        while (reader.Read())
        {
            output.Add(ReadBoolItem(reader));
        }
        return output;
    }
    private static bool ReadBoolItem(DbDataReader reader)
    {
        bool output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetBoolean(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<bool?> GetNullableBoolList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<bool?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableBoolItem(reader));
        }
        return output;
    }
    private static bool? ReadNullableBoolItem(DbDataReader reader)
    {
        bool output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetBoolean(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<decimal> GetDecimalList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<decimal> output = [];
        while (reader.Read())
        {
            output.Add(ReadDecimalItem(reader));
        }
        return output;
    }
    private static decimal ReadDecimalItem(DbDataReader reader)
    {
        decimal output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDecimal(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<decimal?> GetNullableDecimalList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<decimal?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableDecimalItem(reader));
        }
        return output;
    }
    private static decimal? ReadNullableDecimalItem(DbDataReader reader)
    {
        decimal output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDecimal(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<double> GetDoubleList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<double> output = [];
        while (reader.Read())
        {
            output.Add(ReadDoubleItem(reader));
        }
        return output;
    }
    private static double ReadDoubleItem(DbDataReader reader)
    {
        double output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDouble(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<double?> GetNullableDoubleList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<double?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableDoubleItem(reader));
        }
        return output;
    }
    private static double? ReadNullableDoubleItem(DbDataReader reader)
    {
        double output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDouble(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<float> GetFloatList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<float> output = [];
        while (reader.Read())
        {
            output.Add(ReadFloatItem(reader));
        }
        return output;
    }
    private static float ReadFloatItem(DbDataReader reader)
    {
        float output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetFloat(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<float?> GetNullableFloatList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<float?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableFloatItem(reader));
        }
        return output;
    }
    private static float? ReadNullableFloatItem(DbDataReader reader)
    {
        float output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetFloat(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<DateTime> GetDateTimeList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<DateTime> output = [];
        while (reader.Read())
        {
            output.Add(ReadDateTimeItem(reader));
        }
        return output;
    }
    private static DateTime ReadDateTimeItem(DbDataReader reader)
    {
        DateTime output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<DateTime?> GetNullableDateTimeList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<DateTime?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableDateTimeItem(reader));
        }
        return output;
    }
    private static DateTime? ReadNullableDateTimeItem(DbDataReader reader)
    {
        DateTime output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            output = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<DateOnly> GetDateOnlyList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<DateOnly> output = [];
        while (reader.Read())
        {
            output.Add(ReadDateOnlyItem(reader));
        }
        return output;
    }
    private static DateOnly ReadDateOnlyItem(DbDataReader reader)
    {
        DateOnly output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            DateTime date = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
            output = new(date.Year, date.Month, date.Day);
            //output = DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<DateOnly?> GetNullableDateOnlyList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<DateOnly?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableDateOnlyItem(reader));
        }
        return output;
    }
    private static DateOnly? ReadNullableDateOnlyItem(DbDataReader reader)
    {
        DateOnly output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            DateTime date = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
            output = new(date.Year, date.Month, date.Day);
            //output = DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);
        }
        return output;
    }
    public static BasicList<TimeOnly> GetTimeOnlyList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<TimeOnly> output = [];
        while (reader.Read())
        {
            output.Add(ReadTimeOnlyItem(reader));
        }
        return output;
    }
    private static TimeOnly ReadTimeOnlyItem(DbDataReader reader)
    {
        TimeOnly output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            DateTime date = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
            output = new(date.Hour, date.Minute, date.Second, date.Millisecond);
        }
        return output;
    }
    public static BasicList<TimeOnly?> GetNullableTimeOnlyList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<TimeOnly?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableTimeOnlyItem(reader));
        }
        return output;
    }
    private static TimeOnly? ReadNullableTimeOnlyItem(DbDataReader reader)
    {
        TimeOnly output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return null;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            DateTime date = DataReaderExtensions.GetDateTime(reader, list.Single().ColumnName);
            output = new(date.Hour, date.Minute, date.Second, date.Millisecond);
        }
        return output;
    }
    public static BasicList<char> GetCharList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<char> output = [];
        while (reader.Read())
        {
            output.Add(ReadCharItem(reader));
        }
        return output;
    }
    private static char ReadCharItem(DbDataReader reader)
    {
        char output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return default;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            string temp = DataReaderExtensions.GetString(reader, list.Single().ColumnName);
            output = temp.SingleOrDefault();
        }
        return output;
    }
    public static BasicList<char?> GetNullableCharList(this IDbCommand command)
    {
        using DbDataReader? reader = command.ExecuteReader() as DbDataReader ?? throw new CustomBasicException("No reader found");
        BasicList<char?> output = [];
        while (reader.Read())
        {
            output.Add(ReadNullableCharItem(reader));
        }
        return output;
    }
    private static char? ReadNullableCharItem(DbDataReader reader)
    {
        char output = default;
        var list = DbDataReaderExtensions.GetColumnSchema(reader);
        if (list.Count == 0)
        {
            return null;
        }
        if (list.Count > 1)
        {
            throw new CustomBasicException("Cannot have more than one item");
        }
        if (DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)
        {
            string temp = DataReaderExtensions.GetString(reader, list.Single().ColumnName);
            output = temp.SingleOrDefault();
        }
        return output;
    }
}