namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class ExecuteExtensions
{
    extension (ICaptureCommandParameter capture)
    {
        internal void Execute(CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
        {
            capture.Execute(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
        }
        public void Execute(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            capture.Execute(commandDefinition);
        }
        public void Execute(CommandDefinition command)
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
            fins.ExecuteNonQuery();
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
        }
        internal Task ExecuteAsync(CompleteSqlData complete, IDbTransaction? transaction, int? commandTimeout)
        {
            return capture.ExecuteAsync(complete.SQLStatement, complete.Parameters, transaction, commandTimeout, null);
        }
        public async Task ExecuteAsync(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            await capture.ExecuteAsync(commandDefinition);
        }
        public async Task ExecuteAsync(CommandDefinition command)
        {
            await Task.Run(() =>
            {
                capture.Execute(command);
            });
        }
    }
}