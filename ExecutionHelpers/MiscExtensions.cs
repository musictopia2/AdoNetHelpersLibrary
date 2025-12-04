namespace AdoNetHelpersLibrary.ExecutionHelpers;
internal static class MiscExtensions
{
    extension (ICaptureCommandParameter capture)
    {
        private void SetDatabaseParameters(IDbCommand command, BasicList<DynamicParameter>? parameters)
        {
            if (parameters is null)
            {
                return;
            }
            foreach (var item in parameters)
            {
                DbParameter parameter = capture.GetParameter();
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
        public IDbCommand GetCommand(CommandDefinition command)
        {
            IDbCommand fins = capture.GetCommand();
            fins.Connection = capture.CurrentConnection;
            fins.CommandText = command.CommandText;
            if (command.CommandTimeout is not null)
            {
                fins.CommandTimeout = command.CommandTimeout.Value;
            }
            fins.CommandType = command.CommandType;
            fins.Transaction = command.Transaction;
            capture.SetDatabaseParameters(fins, command.Parameters);
            return fins;
        }
    }
}