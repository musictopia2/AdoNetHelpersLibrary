namespace AdoNetHelpersLibrary.ConnectionHelpers;
public class BasicConnector : IConnector
{
    #region Main Functions
    private EnumDatabaseCategory _category;
    private string _connectionString = "";
    bool IConnector.IsTesting => _isTesting;
    public IDbConnector GetConnector { get; private set; }
    IDbConnector IConnector.GetConnector
    {
        get => GetConnector;
        set => GetConnector = value;
    }
    EnumDatabaseCategory IConnector.Category
    {
        get => _category; set => _category = value;
    }
    string IConnector.ConnectionString
    {
        get => _connectionString;
        set => _connectionString = value;
    }
    public static BasicConnector GetSQLiteTestHelper()
    {
        return new BasicConnector();
    }
    private readonly bool _isTesting;
    private static string GetInMemorySQLiteString()
    {
        return "Data Source=:memory:";
    }
    private BasicConnector()
    {
        _isTesting = true;
        _connectionString = GetInMemorySQLiteString();
        _category = EnumDatabaseCategory.SQLite;
        if (GlobalClass.SQLiteConnector is null)
        {
            throw new CustomBasicException("You never registered the interface for sqlite data connector");
        }
        GetConnector = GlobalClass.SQLiteConnector;
    }
    public BasicConnector(string key, EnumDatabaseCategory category = EnumDatabaseCategory.SQLServer) //sql server is most common here.
    {
        Init(category, key, "");
        GetConnector = this.PrivateConnector();
    }
    public IDbConnection GetConnection() //sometimes you need this for the basic connector.
    {
        return ConnectionExtensions.GetConnection(this);
    }
    private void Init(EnumDatabaseCategory category, string key, string proposedPath)
    {
        if (Configuration is null)
        {
            throw new CustomBasicException("Never registered configuration for the basic connection");
        }
        var configuration = Configuration;
        _category = category; //period no matter what.
        string? possibility = null;
        string? path = null;
        if (category == EnumDatabaseCategory.SQLite)
        {
            //this means you can specify a different path if you want (do in configuration).  only useful for sqlite
            path = configuration.GetValue<string>($"{key}Path");
        }
        else
        {
            possibility = configuration.GetConnectionString(key); //sqlite cannot consider the possibility.
        }
        if (possibility is not null)
        {
            if (_category == EnumDatabaseCategory.SQLite)
            {
                throw new CustomBasicException("Sqlite can never have any possibility");
            }
            _connectionString = possibility;
        }
        else if (category == EnumDatabaseCategory.SQLServer)
        {
            _connectionString = SQLServerConnectionClass!.GetConnectionString(key); //this can never be mysql
        }
        else if (category == EnumDatabaseCategory.SQLite)
        {
            string realPath;
            if (path is null)
            {
                realPath = proposedPath;
            }
            else
            {
                realPath = path;
            }
            if (realPath == "")
            {
                throw new CustomBasicException("Path to sqlite database cannot be blank");
            }
            if (ff1.FileExists(realPath) == false)
            {
                throw new CustomBasicException($"Sqlite database at {realPath} does not exist.  Cannot create automatically because its not generic");
            }
            _connectionString = $@"Data Source = {GetCleanedPath(realPath)}";
        }
        else
        {
            throw new CustomBasicException("Based on database category, unable to get connection string to even connect to a database");
        }
    }
    public BasicConnector(string key, string proposedPath)
    {
        Init(EnumDatabaseCategory.SQLite, key, proposedPath);
        GetConnector = this.PrivateConnector();
    }
    #endregion
    #region Work Functions
    public void DoWork(Action<IDbConnection> action)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        action.Invoke(cons);
        cons.Close();
    }
    public async Task DoWorkAsync(Func<IDbConnection, Task> action)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        await action.Invoke(cons);
        cons.Close();
    }
    public async Task DoBulkWorkAsync(Func<IDbConnection, IDbTransaction, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        if (isolationLevel == IsolationLevel.Unspecified)
        {

            using IDbTransaction tran = cons.BeginTransaction();
            await action.Invoke(cons, tran);
        }
        else
        {
            IDbTransaction tran = cons.BeginTransaction(isolationLevel);
            await action.Invoke(cons, tran);
        }
        cons.Close();
    }
    public void DoBulkWork<E>(Action<IDbConnection, IDbTransaction, E> action,
        BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Action<IDbConnection>? beforeWork = null, Action<IDbConnection>? afterWork = null)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        beforeWork?.Invoke(cons);
        thisList.ForEach(items =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                action.Invoke(cons, tran, items);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                action.Invoke(cons, tran, items);
            }
        });
        afterWork?.Invoke(cons);
        cons.Close();
    }
    public void DoWork(Action<IDbConnection, IDbTransaction> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        if (isolationLevel == IsolationLevel.Unspecified)
        {
            using IDbTransaction tran = cons.BeginTransaction();
            action.Invoke(cons, tran);
        }
        else
        {
            IDbTransaction tran = cons.BeginTransaction(isolationLevel);
            action.Invoke(cons, tran);
        }
        cons.Close();
    }
    public async Task DoBulkWorkAsync<E>(Func<IDbConnection, IDbTransaction, E, Task> action, BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Action<IDbConnection>? beforeWork = null, Func<IDbConnection, Task>? afterWork = null)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        beforeWork?.Invoke(cons);
        await thisList.ForEachAsync(async items =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                await action.Invoke(cons, tran, items);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                await action.Invoke(cons, tran, items);
            }
        });
        afterWork?.Invoke(cons);
        cons.Close();
    }
    public async Task DoWorkAsync(Func<IDbConnection, IDbTransaction, Task> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        using IDbConnection cons = GetConnection();
        cons.Open();
        if (isolationLevel == IsolationLevel.Unspecified)
        {
            using IDbTransaction tran = cons.BeginTransaction();
            await action.Invoke(cons, tran);
        }
        else
        {
            IDbTransaction tran = cons.BeginTransaction(isolationLevel);
            await action.Invoke(cons, tran);
        }
        cons.Close();
    }
    #endregion    
}