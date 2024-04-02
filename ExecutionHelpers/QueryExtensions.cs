namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class QueryExtensions
{
    public static BasicList<T> Query<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<T>(capture, commandDefinition);
    }
    public static BasicList<T> Query<T>(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        if (capture.CurrentConnection is null)
        {
            throw new CustomBasicException("No connection when doing query");
        }
        var item = CommandQueryGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
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
        var output = item.Query(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<T>(capture, commandDefinition);
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        if (capture.CurrentConnection is null)
        {
            throw new CustomBasicException("No connection when doing query async ");
        }
        var item = CommandQueryGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
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
        var output = await item.QueryAsync(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
}