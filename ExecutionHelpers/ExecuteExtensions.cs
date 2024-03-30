namespace AdoNetHelpersLibrary.ExecutionHelpers; 
internal static class ExecuteExtensions
{

    //if i ever need the ability to have public access for the execute and executeasync (or even query),
    //then will do.

    public static void Execute(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        connector.Execute(commandDefinition);
    }
    public static void Execute(this IConnector connector, CommandDefinition command)
    {
        using IDbConnection cons = connector.GetConnection();
        cons.Open();
        using IDbCommand fins = connector.GetCommand(cons, command);
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        fins.ExecuteNonQuery();
        cons.Close();
    }
    public static async Task ExecuteAsync(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
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