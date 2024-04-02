namespace AdoNetHelpersLibrary.ConnectionHelpers;
public class BasicConnector : IConnector, ICaptureCommandParameter
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
    public IDbConnection? CurrentConnection { get; set; }
    EnumDatabaseCategory ICaptureCommandParameter.Category
    {
        get => _category; set => _category = value;
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
    internal void RunCustomConnection(Action action)
    {
        using IDbConnection cons = GetConnection();
        CurrentConnection = cons;
        CurrentConnection.Open();
        action.Invoke();
        CurrentConnection.Close();
        CurrentConnection = null;
    }
    internal async Task RunCustomConnectionAsync(Func<Task> action)
    {
        using IDbConnection cons = GetConnection();
        CurrentConnection = cons;
        CurrentConnection.Open();
        await action.Invoke();
        CurrentConnection.Close();
        CurrentConnection = null;
    }

    internal void RunCustomConnection(Action<IDbConnection> action)
    {
        using IDbConnection cons = GetConnection();
        CurrentConnection = cons;
        CurrentConnection.Open();
        action.Invoke(cons);
        CurrentConnection.Close();
        CurrentConnection = null;
    }
    internal async Task RunCustomConnectionAsync(Func<IDbConnection, Task> action)
    {
        using IDbConnection cons = GetConnection();
        CurrentConnection = cons;
        CurrentConnection.Open();
        await action.Invoke(cons);
        CurrentConnection.Close();
        CurrentConnection = null;
    }
    #endregion
    #region Work Functions
    public void DoWork(Action<ICaptureCommandParameter> action)
    {
        RunCustomConnection(() =>
        {
            action.Invoke(this);
        });
    }
    public async Task DoWorkAsync(Func<ICaptureCommandParameter, Task> action)
    {
        await RunCustomConnectionAsync(async () =>
        {
            await action.Invoke(this);
        });
    }
    public void DoBulkWork(Action<ICaptureCommandParameter, IDbTransaction> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        RunCustomConnection(cons =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                action.Invoke(this, tran);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                action.Invoke(this, tran);
            }
        });
    }
    public async Task DoBulkWorkAsync(Func<ICaptureCommandParameter, IDbTransaction, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        await RunCustomConnectionAsync(async cons =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                await action.Invoke(this, tran);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                await action.Invoke(this, tran);
            }
        });
    }
    public void DoBulkWork<E>(Action<ICaptureCommandParameter, IDbTransaction, E> action,
        BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Action<ICaptureCommandParameter>? beforeWork = null, Action<ICaptureCommandParameter>? afterWork = null)
    {
        RunCustomConnection(cons =>
        {
            beforeWork?.Invoke(this);
            thisList.ForEach(item =>
            {
                if (isolationLevel == IsolationLevel.Unspecified)
                {
                    using IDbTransaction tran = cons.BeginTransaction();
                    action.Invoke(this, tran, item);
                }
                else
                {
                    using IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                    action.Invoke(this, tran, item);
                }
            });
            afterWork?.Invoke(this);
        });
    }
    public void DoWork(Action<ICaptureCommandParameter, IDbTransaction> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        RunCustomConnection(cons =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                action.Invoke(this, tran);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                action.Invoke(this, tran);
            }
        });
    }
    public async Task DoBulkWorkAsync<E>(Func<ICaptureCommandParameter, IDbTransaction, E, Task> action, BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Func<ICaptureCommandParameter, Task>? beforeWork = null, Func<ICaptureCommandParameter, Task>? afterWork = null)
    {
        await RunCustomConnectionAsync(async cons =>
        {
            if (beforeWork is not null)
            {
                await beforeWork.Invoke(this);
            }
            await thisList.ForEachAsync(async item =>
            {
                if (isolationLevel == IsolationLevel.Unspecified)
                {
                    using IDbTransaction tran = cons.BeginTransaction();
                    await action.Invoke(this, tran, item);
                }
                else
                {
                    using IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                    await action.Invoke(this, tran, item);
                }
            });
            if (afterWork is not null)
            {
                await afterWork.Invoke(this);
            }
        });
    }
    public async Task DoWorkAsync(Func<ICaptureCommandParameter, IDbTransaction, Task> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        await RunCustomConnectionAsync(async (cons) =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = cons.BeginTransaction();
                await action.Invoke(this, tran);
            }
            else
            {
                IDbTransaction tran = cons.BeginTransaction(isolationLevel);
                await action.Invoke(this, tran);
            }
        });
    }
    #endregion
    #region Unique Functions
    public void UpdateAll<E>(E thisEntity) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(() =>
        {
            this.UpdateEntity(thisEntity, EnumUpdateCategory.All);
        });
    }
    public int Insert<E>(E entity) where E : class, ISimpleDapperEntity
    {
        int output = default;
        RunCustomConnection(() =>
        {
            output = this.InsertSingle(entity);
        });
        return output;
    }
    public async Task<int> InsertAsync<E>(E entity) where E : class, ISimpleDapperEntity
    {
        int output = default;
        await RunCustomConnectionAsync(async () =>
        {
            output = await this.InsertSingleAsync(entity);
        });
        return output;
    }
    #endregion
    #region Direct To Extensions Except Get
    public void DeleteOnly<E>(E thisEntity) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(() =>
        {
            this.Delete(thisEntity);
        });
    }
    public void DeleteOnly<E>(int id) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(() =>
        {
            this.Delete<E>(id);
        });
    }
    #endregion
    #region Direct To Extensions For Getting
    public async Task<E> GetAsync<E>(int id) where E: class, ISimpleDapperEntity
    {
        E output = default!;
        await RunCustomConnectionAsync(async () =>
        {
            output = await this.GetAsync<E>(id, null, null);
        });
        return output;
    }
    public E Get<E>(int id) where E : class, ISimpleDapperEntity
    {
        E output = default!;
        RunCustomConnection(() =>
        {
            output = this.Get<E>(id, null, null); //has to send the others to use extension so no never ending loops
        });
        return output;
    }
    public BasicList<E> GetDataList<E>()
        where E: class, ISimpleDapperEntity
    {
        BasicList<E> output = [];
        DoWork(capture =>
        {
            output = capture.Get<E>();
        });
        return output;
    }

    /// <summary>
    /// this is when you just want to get all the data from the database.  most simple but no parameters this time.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <returns></returns>
    public async Task<BasicList<E>> GetDataListAsync<E>() where E : class, ISimpleDapperEntity //this should still work.
    {
        BasicList<E> output = [];
        await DoWorkAsync(async capture =>
        {
            output = await capture.GetAsync<E>();
        });
        return output;
    }
    public BasicList<T> LoadData<T>(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
    {
        BasicList<T> output = [];
        RunCustomConnection(() =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = this.Query<T>(command);
        });
        return output;
    }
    public BasicList<T> LoadData<T>(string sqlStatement, BasicList<DynamicParameter> parameters)
    {
        return LoadData<T>(sqlStatement, parameters, false);
    }
    public BasicList<T> LoadData<T>(string sqlStatement)
    {
        return LoadData<T>(sqlStatement, [], false);
    }
    public BasicList<T> LoadData<T>(string sqlStatement, bool isStoredProcedure)
    {
        return LoadData<T>(sqlStatement, [], isStoredProcedure);
    }
    public async Task<BasicList<T>> LoadDataAsync<T>(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
    {
        BasicList<T> output = [];
        await RunCustomConnectionAsync(async () =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = await this.QueryAsync<T>(command);
        });
        return output;
    }
    public async Task<BasicList<T>> LoadDataAsync<T>(string sqlStatement, BasicList<DynamicParameter> parameters)
    {
        return await LoadDataAsync<T>(sqlStatement, parameters, false);
    }
    public async Task<BasicList<T>> LoadDataAsync<T>(string sqlStatement)
    {
        return await LoadDataAsync<T>(sqlStatement, [], false);
    }
    public async Task<BasicList<T>> LoadDataAsync<T>(string sqlStatement, bool isStoredProcedure)
    {
        return await LoadDataAsync<T>(sqlStatement, [], isStoredProcedure);
    }
    public void SaveData(string sqlStatement)
    {
        SaveData(sqlStatement, []);
    }
    public void SaveData(string sqlStatement, bool isStoredProcedure)
    {
        SaveData(sqlStatement, [], isStoredProcedure);
    }
    public void SaveData(string sqlStatement, BasicList<DynamicParameter> parameters)
    {
        SaveData(sqlStatement, parameters, false);
    }
    public async Task SaveDataAsync(string sqlStatement, BasicList<DynamicParameter> parameters)
    {
        await SaveDataAsync(sqlStatement, parameters, false);
    }
    public void SaveData(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
    {
        RunCustomConnection(cons =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            this.Execute(command);
        });
    }
    public async Task SaveDataAsync(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
    {
        await Task.Run(() =>
        {
            SaveData(sqlStatement, parameters, isStoredProcedure); //this is the best i can do for now.
        });
    }
    public async Task SaveDataAsync(string sqlStatement)
    {
        await SaveDataAsync(sqlStatement, []);
    }
    public async Task SaveDataAsync(string sqlStatement, bool isStoredProcedure)
    {
        await SaveDataAsync(sqlStatement, [], isStoredProcedure);
    }
    #endregion
    #region Advanced Interface Functions
    DbParameter ICaptureCommandParameter.GetParameter()
    {
        return GetConnector.GetParameter();
    }
    IDbCommand ICaptureCommandParameter.GetCommand()
    {
        return GetConnector.GetCommand();
    }
    #endregion
}