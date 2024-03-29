namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class QueryExtensions
{
    public static BasicList<T> Query<T>(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return Query<T>(connector, commandDefinition);
    }
    public static BasicList<T> Query<T>(this IConnector connector, CommandDefinition command)
    {
        var item = SourceGeneratedGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
        using IDbConnection cons = connector.GetConnection();
        using IDbCommand fins = connector.GetCommand(cons, command);
        return item.Query(fins);
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await QueryAsync<T>(connector, commandDefinition);
    }
    public static async Task<BasicList<T>> QueryAsync<T>(this IConnector connector, CommandDefinition command)
    {
        var item = SourceGeneratedGlobalClass<T>.MasterContext ?? throw new CustomBasicException($"Nothing was registered for type {typeof(T)}.  Try creating a source generator and register it");
        using IDbConnection cons = connector.GetConnection();
        using IDbCommand fins = connector.GetCommand(cons, command);
        return await item.QueryAsync(fins);
    }
}