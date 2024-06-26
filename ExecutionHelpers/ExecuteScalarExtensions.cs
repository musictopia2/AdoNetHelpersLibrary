﻿namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class ExecuteScalarExtensions
{
    //i like the idea of 2 possibilities.  one requires iparsable.  the other allows more open.
    internal static R? ExecuteScalar<E, R>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction = null, int? commandTimeout = null)
        where E: class, ICommandExecuteScalar<E, R>
    {
        return capture.ExecuteScalar<E, R>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static R? ExecuteScalar<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandExecuteScalar<E, R>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalar<E, R>(commandDefinition);
    }
    public static R? ExecuteScalar<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandExecuteScalar<E, R>
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
        R? output = E.ExecuteScalar(fins);
        return output;
    }
    internal static Task<R?> ExecuteScalarAsync<E, R>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction = null, int? commandTimeout = null)
        where E: class, ICommandExecuteScalar<E, R>
    {
        return capture.ExecuteScalarAsync<E, R>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static Task<R?> ExecuteScalarAsync<E, R>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E: class, ICommandExecuteScalar<E, R>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalarAsync<E, R>(commandDefinition);
    }
    public static async Task<R?> ExecuteScalarAsync<E, R>(this ICaptureCommandParameter capture, CommandDefinition command)
        where E : class, ICommandExecuteScalar<E, R>
    {
        R? item = default;
        await Task.Run(() =>
        {
            item = capture.ExecuteScalar<E, R>(command);
        });
        if (item is null)
        {
            throw new CustomBasicException("Nothing for item");
        }
        return item;
    }
    internal static T ExecuteScalar<T>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction = null, int? commandTimeout = null)
        where T : IParsable<T>
    {
        return capture.ExecuteScalar<T>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    internal static T ExecuteScalar<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where T : IParsable<T>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalar<T>(commandDefinition);
    }
    internal static T ExecuteScalar<T>(this ICaptureCommandParameter capture, CommandDefinition command)
        where T : IParsable<T>
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
        if (isClosed)
        {
            capture.CurrentConnection.Close();
        }
        //can be questionable with bool.
        //may be forced to do something else for scalar for some situations (?)

        return T.Parse(results!.ToString()!, null);
    }
    internal static Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction = null, int? commandTimeout = null)
        where T : IParsable<T>
    {
        return capture.ExecuteScalarAsync<T>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    internal static Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where T : IParsable<T>
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalarAsync<T>(commandDefinition);
    }
    internal static async Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, CommandDefinition command)
        where T : IParsable<T>
    {
        T? item = default;
        await Task.Run(() =>
        {
            item = capture.ExecuteScalar<T>(command);
        });
        if (item is null)
        {
            throw new CustomBasicException("Nothing for item");
        }
        return item;
    }
}