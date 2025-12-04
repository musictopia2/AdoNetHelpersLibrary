namespace AdoNetHelpersLibrary.ExecutionHelpers;
public static class QueryExtensions
{
    extension (ICaptureCommandParameter capture)
    {
        public BasicList<R> Query<E, R>(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        where E : class, ICommandQuery<E, R>
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return Query<E, R>(capture, commandDefinition);
        }
        public BasicList<R> Query<E, R>(CommandDefinition command)
            where E : class, ICommandQuery<E, R>
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
            var output = E.Query(fins, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public Task<BasicList<R>> QueryAsync<E, R>(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where E : class, ICommandQuery<E, R>
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return capture.QueryAsync<E, R>(commandDefinition);
        }
        public async Task<BasicList<R>> QueryAsync<E, R>(CommandDefinition command)
            where E : class, ICommandQuery<E, R>
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
            var output = await E.QueryAsync(fins, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public BasicList<E> Query<E>(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where E : class, ICommandQuery<E>
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return Query<E>(capture, commandDefinition);
        }
        public BasicList<E> Query<E>(CommandDefinition command)
            where E : class, ICommandQuery<E>
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
            var output = E.Query(fins, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public async Task<BasicList<E>> QueryAsync<E>(string sql, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where E : class, ICommandQuery<E>
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return await QueryAsync<E>(capture, commandDefinition);
        }
        public async Task<BasicList<E>> QueryAsync<E>(CommandDefinition command)
            where E : class, ICommandQuery<E>
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
            var output = await E.QueryAsync(fins, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public BasicList<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where TFirst : class, ICommandQuery<TFirst, TSecond, TReturn>
            where TSecond : class
            where TReturn : class
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return Query<TFirst, TSecond, TReturn>(capture, commandDefinition, action);
        }
        public BasicList<TReturn> Query<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond?, TFirst> action)
            where TFirst : class, ICommandQuery<TFirst, TSecond, TReturn>
            where TSecond : class
            where TReturn : class
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
            var output = TFirst.Query(fins, action, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
           where TFirst : class, ICommandQuery<TFirst, TSecond, TReturn>
            where TSecond : class
            where TReturn : class
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return await QueryAsync<TFirst, TSecond, TReturn>(capture, commandDefinition, action);
        }
        public async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond?, TFirst> action)
          where TFirst : class, ICommandQuery<TFirst, TSecond, TReturn>
            where TSecond : class
            where TReturn : class
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
            var output = await TFirst.QueryAsync(fins, action, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }

        public BasicList<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond?, TThird?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where TFirst : class, ICommandQuery<TFirst, TSecond, TThird, TReturn>
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return Query<TFirst, TSecond, TThird, TReturn>(capture, commandDefinition, action);
        }
        public BasicList<TReturn> Query<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond?, TThird?, TFirst> action)
            where TFirst : class, ICommandQuery<TFirst, TSecond, TThird, TReturn>
            where TSecond : class
            where TThird : class
            where TReturn : class
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
            var output = TFirst.Query(fins, action, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
        public async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond?, TThird?, TFirst> action, BasicList<DynamicParameter>? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
           where TFirst : class, ICommandQuery<TFirst, TSecond, TThird, TReturn>
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            CommandDefinition commandDefinition = new(sql, param, transaction, commandTimeout, commandType);
            return await QueryAsync<TFirst, TSecond, TThird, TReturn>(capture, commandDefinition, action);
        }
        public async Task<BasicList<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond?, TThird?, TFirst> action)
          where TFirst : class, ICommandQuery<TFirst, TSecond, TThird, TReturn>
            where TSecond : class
            where TThird : class
            where TReturn : class
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
            var output = await TFirst.QueryAsync(fins, action, capture.Category);
            if (isClosed)
            {
                capture.CurrentConnection.Close();
            }
            return output;
        }
    }
}