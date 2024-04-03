namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class QueryExtensions
{
    public static BasicList<R> Query<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
        where E : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<E, R>(capture, commandDefinition);
    }
    public static BasicList<R> Query<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class
    {
        var item = CommandQueryGlobalClass<E, R>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(E)} and simple object of {typeof(R)}.  Try creating a source generator and register it");
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
    public static Task<BasicList<R>> QueryAsync<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
        where E : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.QueryAsync<E, R>(commandDefinition);
    }
    public static async Task<BasicList<R>> QueryAsync<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class
    {
        var item = CommandQueryGlobalClass<E, R>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(E)} and simple object of {typeof(R)}.  Try creating a source generator and register it");
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
    public static BasicList<E> Query<E>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
        where E : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<E>(capture, commandDefinition);
    }
    public static BasicList<E> Query<E>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class
    {
        var item = CommandQueryGlobalClass<E>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(E)}.  Try creating a source generator and register it");
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
    public static async Task<BasicList<E>> QueryAsync<E>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
        where E : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<E>(capture, commandDefinition);
    }
    public static async Task<BasicList<E>> QueryAsync<E>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class
    {
        var item = CommandQueryGlobalClass<E>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(E)}.  Try creating a source generator and register it");
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