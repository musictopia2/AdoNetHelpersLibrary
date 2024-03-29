namespace AdoNetHelpersLibrary.ExecutionHelpers; 
internal static class ExecuteExtensions
{

    //if i ever need the ability to have public access for the execute and executeasync (or even query),
    //then will do.

    public static void Execute(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        connector.Execute(commandDefinition);
    }
    public static void Execute(this IConnector connector, CommandDefinition command)
    {
        using IDbConnection cons = connector.GetConnection();
        using IDbCommand fins = connector.GetCommand(cons, command);
        fins.ExecuteNonQuery();
    }
    public static async Task ExecuteAsync(this IConnector connector, string sql, BasicList<DynamicParameters>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        await connector.ExecuteAsync(commandDefinition);
    }
    public static async Task ExecuteAsync(this IConnector connector, CommandDefinition command)
    {
        await Task.Run(() =>
        {
            connector.Execute(command);
        });
    }
}