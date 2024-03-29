﻿namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class ExecuteScalarExtensions
{
    //if i somehow specialized stuff, rethink.
    public static T ExecuteScalar<T>(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return connector.ExecuteScalar<T>(commandDefinition);
    }
    public static T ExecuteScalar<T>(this IConnector connector, CommandDefinition command)
    {
        using IDbConnection cons = connector.GetConnection();
        using IDbCommand fins = connector.GetCommand(cons, command);
        object? results = fins.ExecuteScalar();
        return (T)results!;
    }
    public static Task<T> ExecuteScalarAsync<T>(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return connector.ExecuteScalarAsync<T>(commandDefinition);
    }
    public static async Task<T> ExecuteScalarAsync<T>(this IConnector connector, CommandDefinition command)
    {
        T? item = default;
        await Task.Run(() =>
        {
            item = connector.ExecuteScalar<T>(command);  
        });
        if (item is null)
        {
            throw new CustomBasicException("Nothing for item");
        }
        return item;
    }
}