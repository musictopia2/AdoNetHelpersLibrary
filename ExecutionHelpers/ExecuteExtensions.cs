namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class ExecuteExtensions
{
    public static int Execute(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.Execute(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static int Execute(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.Execute(commandDefinition);
    }
    public static int Execute(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        if (capture.CurrentConnection is null)
        {
            throw new CustomBasicException("No connection when executing");
        }
        bool isClosed;
        isClosed = capture.CurrentConnection.State == ConnectionState.Closed;
        if (isClosed)
        {
            capture.CurrentConnection.Open();
        }
        using IDbCommand fins = capture.GetCommand();
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        int output = fins.ExecuteNonQuery();
        fins.ExecuteNonQuery();
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static Task<int> ExecuteAsync(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.ExecuteAsync(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static async Task<int> ExecuteAsync(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await capture.ExecuteAsync(commandDefinition);
    }
    public static async Task<int> ExecuteAsync(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        int output = 0;
        await Task.Run(() =>
        {
            output = capture.Execute(command);
        });
        return output;
    }
}