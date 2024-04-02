namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class ExecuteExtensions
{
    public static void Execute(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        capture.Execute(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static void Execute(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        capture.Execute(commandDefinition);
    }
    public static void Execute(this ICaptureCommandParameter capture, CommandDefinition command)
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
        fins.ExecuteNonQuery();
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
    }
    public static Task ExecuteAsync(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.ExecuteAsync(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static async Task ExecuteAsync(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        await capture.ExecuteAsync(commandDefinition);
    }
    public static async Task ExecuteAsync(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        await Task.Run(() =>
        {
            capture.Execute(command);
        });
    }
}