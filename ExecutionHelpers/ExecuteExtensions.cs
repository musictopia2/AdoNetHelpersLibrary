namespace AdoNetHelpersLibrary.ExecutionHelpers; 
internal static class ExecuteExtensions
{

    //if i ever need the ability to have public access for the execute and executeasync (or even query),
    //then will do.
    public static int Execute(this IConnector connector, CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        return connector.Execute(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, commandType);
    }
    public static int Execute(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return connector.Execute(commandDefinition);
    }
    public static int Execute(this IConnector connector, CommandDefinition command)
    {
        using IDbConnection cons = connector.GetConnection();
        cons.Open();
        using IDbCommand fins = connector.GetCommand(cons, command);
        if (command.Transaction is not null)
        {
            fins.Transaction = command.Transaction;
        }
        int output = fins.ExecuteNonQuery();
        fins.ExecuteNonQuery();
        cons.Close();
        return output;
    }
    public static async Task<int> ExecuteAsync(this IConnector connector, string sql, BasicList<DynamicParameter>? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
    {
        CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
        return await connector.ExecuteAsync(commandDefinition);
    }
    public static async Task<int> ExecuteAsync(this IConnector connector, CommandDefinition command)
    {
        int output = 0;
        await Task.Run(() =>
        {
            output = connector.Execute(command);
        });
        return output;
    }
}