namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class QueryExtensions
{
    public static BasicList<T> Query<T>(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<T>(connector, commandDefinition);
    }
    public static BasicList<T> Query<T>(this IConnector connector, CommandDefinition command)
    {
        var item = CommandQueryGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
        using IDbConnection cons = connector.GetConnection();
        cons.Open();
        using IDbCommand fins = connector.GetCommand(cons, command);
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        var output = item.Query(fins);
        cons.Close();
        return output;
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<T>(connector, commandDefinition);
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this IConnector connector, CommandDefinition command)
    {
        var item = CommandQueryGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
        using IDbConnection cons = connector.GetConnection();
        cons.Open();
        using IDbCommand fins = connector.GetCommand(cons, command);
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        var output = await item.QueryAsync(fins);
        cons.Close();
        return output;
    }
}