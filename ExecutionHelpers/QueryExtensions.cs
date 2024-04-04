namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class QueryExtensions
{
    public static BasicList<R> Query<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandQuery<E, R>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<E, R>(capture, commandDefinition);
    }
    public static BasicList<R> Query<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandQuery<E, R>
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
        var output = E.Query(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static Task<BasicList<R>> QueryAsync<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandQuery<E, R>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.QueryAsync<E, R>(commandDefinition);
    }
    public static async Task<BasicList<R>> QueryAsync<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandQuery<E, R>
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
        var output = await E.QueryAsync(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static BasicList<E> Query<E>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandQuery<E>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<E>(capture, commandDefinition);
    }
    public static BasicList<E> Query<E>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandQuery<E>
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
        var output = E.Query(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static async Task<BasicList<E>> QueryAsync<E>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandQuery<E>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<E>(capture, commandDefinition);
    }
    public static async Task<BasicList<E>> QueryAsync<E>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandQuery<E>
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
        var output = await E.QueryAsync(fins);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static BasicList<TReturn> Query<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
       where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
       where TSecond : class, ISimpleDatabaseEntity
       where TReturn : class, ISimpleDatabaseEntity
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<TFirst, TSecond, TReturn>(capture, commandDefinition, PrivateOneToOne);
    }
    public static BasicList<TReturn> Query<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, string sql, Func<TFirst, TSecond?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
        where TSecond : class, ISimpleDatabaseEntity
        where TReturn : class, ISimpleDatabaseEntity
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<TFirst, TSecond, TReturn>(capture, commandDefinition, action);
    }
    public static BasicList<TReturn> Query<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, CommandDefinition command, Func<TFirst, TSecond?, TFirst> action)
        where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
        where TSecond : class, ISimpleDatabaseEntity
        where TReturn : class, ISimpleDatabaseEntity
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
        var output = TFirst.Query(fins, action);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
    public static Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
       where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
       where TSecond : class, ISimpleDatabaseEntity
       where TReturn : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return QueryAsync<TFirst, TSecond, TReturn>(capture, command: commandDefinition, PrivateOneToOne);
    }
    public static async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, string sql, Func<TFirst, TSecond?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
       where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
       where TSecond : class, ISimpleDatabaseEntity
       where TReturn : class
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<TFirst, TSecond, TReturn>(capture, commandDefinition, action);
    }
    public static async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this ICaptureCommandParameter capture, CommandDefinition command, Func<TFirst, TSecond?, TFirst> action)
        where TFirst : class, IJoinedEntity<TSecond>, ICommandQuery<TFirst, TSecond, TReturn>
       where TSecond : class, ISimpleDatabaseEntity
       where TReturn : class
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
        var output = await TFirst.QueryAsync(fins, action);
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        return output;
    }
}