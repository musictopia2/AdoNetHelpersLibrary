namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class MiscExtensions
{
    private static void SetDatabaseParameters(this IConnector custom, IDbCommand command, BasicList<DynamicParameter>? parameters)
    {
        if (parameters is null)
        {
            return;
        }
        foreach (var item in parameters)
        {
            DbParameter parameter = custom.GetConnector.GetParameter();
            parameter.ParameterName = item.ParameterName;
            parameter.DbType = item.DbType;
            parameter.Precision = item.Precision;
            parameter.SourceColumnNullMapping = item.SourceColumnNullMapping;
            if (item.Value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = item.Value;
            }
            command.Parameters.Add(parameter);
        }
    }
    public static IDbCommand GetCommand(this IConnector custom, IDbConnection cnn, CommandDefinition command)
    {
        IDbCommand fins = custom.GetConnector.GetCommand();
        fins.Connection = cnn;
        fins.CommandText = command.CommandText;
        if (command.CommandTimeout is not null)
        {
            fins.CommandTimeout = command.CommandTimeout.Value;
        }
        fins.CommandType = command.CommandType;
        fins.Transaction = command.Transaction;
        SetDatabaseParameters(custom, fins, command.Parameters);
        return fins;
    }
}