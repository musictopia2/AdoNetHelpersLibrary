﻿namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class ExecuteScalarExtensions
{
    public static T ExecuteScalar<T>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.ExecuteScalar<T>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static T ExecuteScalar<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalar<T>(commandDefinition);
    }
    public static T ExecuteScalar<T>(this ICaptureCommandParameter capture, CommandDefinition command)
    {
        if (capture.CurrentConnection is null)
        {
            throw new CustomBasicException("No connection when ExecutingScalar");
        }
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
        return (T)results!;
    }
    public static Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
    {
        return capture.ExecuteScalarAsync<T>(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
    }
    public static Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return capture.ExecuteScalarAsync<T>(commandDefinition);
    }
    public static async Task<T> ExecuteScalarAsync<T>(this ICaptureCommandParameter capture, CommandDefinition command)
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