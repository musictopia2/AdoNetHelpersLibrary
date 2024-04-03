namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class ExistExtensions
{
    public static bool Exists(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.Exists(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static bool Exists(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.Exists(commandDefinition);
    }
    public static bool Exists(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        bool isClosed;
        isClosed = capture.CurrentConnection.State == ConnectionState.Closed;
        if (isClosed)
        {
            capture.CurrentConnection.Open();
        }
        using IDbCommand fins = capture.GetCommand(command);
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        object? results = fins.ExecuteScalar();
        if (results is null)
        {
            return false;
        }
        string item = results.ToString()!;
        if (item == "1")
        {
            return true;
        }
        if (item == "0")
        {
            return false;
        }
        throw new CustomBasicException($"Not sure whether the value {results} exist");
    }
}