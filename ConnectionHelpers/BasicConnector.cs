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
    internal void RunCustomConnection(Action<CaptureCommandParameter> action)
    {
        using CaptureCommandParameter capture = new(this);
        action.Invoke(capture);
    }
    internal async Task RunCustomConnectionAsync(Func<CaptureCommandParameter, Task> action)
    {
        using CaptureCommandParameter capture = new(this);
        await action.Invoke(capture);
    }
    #endregion
    #region Work Functions
    public void DoWork(Action<ICaptureCommandParameter> action)
    {
        RunCustomConnection(action.Invoke);
    }
    public async Task DoWorkAsync(Func<ICaptureCommandParameter, Task> action)
    {
        await RunCustomConnectionAsync(action.Invoke);
    }
    public void DoBulkWork(Action<ICaptureCommandParameter, IDbTransaction> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        RunCustomConnection(capture =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                action.Invoke(capture, tran);
            }
            else
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                action.Invoke(capture, tran);
            }
        });
    }
    public async Task DoBulkWorkAsync(Func<ICaptureCommandParameter, IDbTransaction, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        await RunCustomConnectionAsync(async capture =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                await action.Invoke(capture, tran);
            }
            else
            {
                IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                await action.Invoke(capture, tran);
            }
        });
    }
    public void DoBulkWork<E>(Action<ICaptureCommandParameter, IDbTransaction, E> action,
        BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Action<ICaptureCommandParameter>? beforeWork = null, Action<ICaptureCommandParameter>? afterWork = null)
    {
        RunCustomConnection(capture =>
        {
            beforeWork?.Invoke(capture);
            thisList.ForEach(item =>
            {
                if (isolationLevel == IsolationLevel.Unspecified)
                {
                    using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                    action.Invoke(capture, tran, item);
                }
                else
                {
                    using IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                    action.Invoke(capture, tran, item);
                }
            });
            afterWork?.Invoke(capture);
        });
    }
    public void DoWork(Action<ICaptureCommandParameter, IDbTransaction> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        RunCustomConnection(capture =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                action.Invoke(capture, tran);
            }
            else
            {
                IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                action.Invoke(capture, tran);
            }
        });
    }
    public async Task DoBulkWorkAsync<E>(Func<ICaptureCommandParameter, IDbTransaction, E, Task> action, BasicList<E> thisList, IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        Func<ICaptureCommandParameter, Task>? beforeWork = null, Func<ICaptureCommandParameter, Task>? afterWork = null)
    {
        await RunCustomConnectionAsync(async capture =>
        {
            if (beforeWork is not null)
            {
                await beforeWork.Invoke(capture);
            }
            await thisList.ForEachAsync(async item =>
            {
                if (isolationLevel == IsolationLevel.Unspecified)
                {
                    using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                    await action.Invoke(capture, tran, item);
                }
                else
                {
                    using IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                    await action.Invoke(capture, tran, item);
                }
            });
            if (afterWork is not null)
            {
                await afterWork.Invoke(capture);
            }
        });
    }
    public async Task DoWorkAsync(Func<ICaptureCommandParameter, IDbTransaction, Task> action, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        await RunCustomConnectionAsync(async capture =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                await action.Invoke(capture, tran);
            }
            else
            {
                IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                await action.Invoke(capture, tran);
            }
        });
    }
    #endregion
    #region Unique Functions
    public void UpdateAll<E>(E thisEntity) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(capture =>
        {
            capture.UpdateEntity(thisEntity, EnumUpdateCategory.All);
        });
    }
    public void UpdateCommonOnly<E>(E thisEntity) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(capture =>
        {
            capture.UpdateEntity(thisEntity, EnumUpdateCategory.Common);
        });
    }
    public int Insert<E>(E entity) where E : class, ISimpleDapperEntity
    {
        int output = default;
        RunCustomConnection(capture =>
        {
            output = capture.InsertSingle(entity);
        });
        return output;
    }
    public async Task<int> InsertAsync<E>(E entity) where E : class, ISimpleDapperEntity
    {
        int output = default;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.InsertSingleAsync(entity);
        });
        return output;
    }
    #endregion
    #region Direct To Extensions Except Get
    public void DeleteOnly<E>(E thisEntity) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(capture =>
        {
            capture.Delete(thisEntity);
        });
    }
    public void DeleteOnly<E>(int id) where E : class, ISimpleDapperEntity
    {
        RunCustomConnection(capture =>
        {
            capture.Delete<E>(id);
        });
    }
    #endregion
    #region Direct To Extensions For Getting
    public async Task<E> GetAsync<E>(int id) where E: class, ISimpleDapperEntity
    {
        E output = default!;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E>(id);
        });
        return output;
    }
    public E Get<E>(int id) where E : class, ISimpleDapperEntity
    {
        E output = default!;
        RunCustomConnection(capture =>
        {
            output = capture.Get<E>(id);
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
        RunCustomConnection(capture =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = capture.Query<T>(command);
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
        await RunCustomConnectionAsync(async capture =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = await capture.QueryAsync<T>(command);
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
        RunCustomConnection(capture =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            capture.Execute(command);
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
}