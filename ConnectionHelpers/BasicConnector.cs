using CommonBasicLibraries.DatabaseHelpers.EntityInterfaces;

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
    public async Task UpdateListOnlyAsync<E>(BasicList<E> updateList, EnumUpdateCategory category = EnumUpdateCategory.Common, IsolationLevel isolationLevel = IsolationLevel.Unspecified) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await DoBulkWorkAsync(async (capture, tran, thisEntity) =>
        {
            await capture.UpdateEntityAsync(thisEntity, category: category, thisTran: tran);
            tran.Commit();
        }, updateList, isolationLevel);
    }
    public async Task UpdateListAutoOnlyAsync<E>(BasicList<E> updateList, IsolationLevel isolationLevel = IsolationLevel.Unspecified) 
        where E : class, ISimpleDatabaseEntity, IUpdatableEntity, ITableMapper<E>
    {
        await UpdateListOnlyAsync(updateList, category: EnumUpdateCategory.Auto, isolationLevel);
    }
    public async Task UpdateListOnlyAsync<E>(BasicList<E> updateList, BasicList<UpdateFieldInfo> manuelList, IsolationLevel isolationLevel = IsolationLevel.Unspecified) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await DoBulkWorkAsync(async (capture, tran, thisEntity) =>
        {
            await capture.UpdateEntityAsync(thisEntity, manuelList, thisTran: tran);
            tran.Commit();
        }, updateList, isolationLevel);
    }
    public void UpdateCommonListOnly<E>(BasicList<E> updateList, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        DoBulkWork((capture, tran, thisEntity) =>
        {
            capture.UpdateEntity(thisEntity, EnumUpdateCategory.Common, thisTran: tran);
            tran.Commit();
        }, updateList, isolationLevel);
    }
    public async Task UpdateCommonOnlyAsync<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            await capture.UpdateEntityAsync(thisEntity, EnumUpdateCategory.Common);
        });
    }
    public async Task UpdateAllAsync<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            await capture.UpdateEntityAsync(thisEntity, EnumUpdateCategory.All);
        });
    }
    public void UpdateAll<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            capture.UpdateEntity(thisEntity, EnumUpdateCategory.All);
        });
    }
    public void UpdateCommonOnly<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            capture.UpdateEntity(thisEntity, EnumUpdateCategory.Common);
        });
    }
    public async Task InsertRangeAsync<E>(BasicList<E> insertList, IsolationLevel isolationLevel = IsolationLevel.Unspecified, bool isStarterData = false, Action? recordsExisted = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await DoWorkAsync(async (capture, tran) =>
        {
            if (isStarterData)
            {
                int count = capture.Count<E>(tran);
                if (count > 0)
                {
                    recordsExisted?.Invoke();
                    return;
                }
            }
            await capture.InsertRangeAsync(insertList, tran);
            tran.Commit();
        }, isolationLevel);
    }
    public async Task<int> InsertAsync<E>(E entity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        int output = default;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.InsertSingleAsync(entity);
        });
        return output;
    }
    public int Insert<E>(E entity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        int output = default;
        RunCustomConnection(capture =>
        {
            output = capture.InsertSingle(entity);
        });
        return output;
    }
    public void InsertRange<E>(BasicList<E> insertList, IsolationLevel isolationLevel = IsolationLevel.Unspecified, bool isStarterData = false)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        DoWork((capture, tran) =>
        {
            if (isStarterData)
            {
                int count = capture.Count<E>(tran);
                if (count > 0)
                {
                    return;
                }
            }
            capture.InsertRange(insertList, tran);
            tran.Commit();
        }, isolationLevel);
    }
    #endregion
    #region Direct To Extensions Except Get
    public async Task AddListOnlyAsync<E>(BasicList<E> addList, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            if (isolationLevel == IsolationLevel.Unspecified)
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction();
                await capture.InsertRangeAsync(addList, tran);
                tran.Commit();
            }
            else
            {
                using IDbTransaction tran = capture.CurrentConnection.BeginTransaction(isolationLevel);
                await capture.InsertRangeAsync(addList, tran);
                tran.Commit();
            }
        });
    }
    public async Task AddEntityAsync<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            thisEntity.ID = await capture.InsertSingleAsync(thisEntity);
        });
    }
    public void AddEntity<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            thisEntity.ID = capture.InsertSingle(thisEntity);
        });
    }
    public void DeleteOnly<E>(E thisEntity) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            capture.Delete(thisEntity);
        });
    }
    public async Task DeleteOnlyAsync<E>(E thisEntity)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            await capture.DeleteAsync(thisEntity);
        });
    }
    public void DeleteOnly<E>(BasicList<ICondition> conditions) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            capture.Delete<E>(conditions);
        });
    }
    public async Task DeleteOnlyAsync<E>(BasicList<ICondition> conditions) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            await capture.DeleteAsync<E>(conditions);
        });
    }
    public async Task DeleteOnlyAsync<E>(int id) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        await RunCustomConnectionAsync(async capture =>
        {
            await capture.DeleteAsync<E>(id);
        });
    }
    public void DeleteOnly<E>(int id) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        RunCustomConnection(capture =>
        {
            capture.Delete<E>(id);
        });
    }
    public void Execute(string sqls)
    {
        DoWork(capture =>
        {
            capture.Execute(sqls, null, null, null, null);
        });
    }
    public async Task ExecuteAsync(string sqls)
    {
        await DoWorkAsync(async capture =>
        {
            await capture.ExecuteAsync(sqls, null, null, null, null);
        });
    }
    public async Task ExecuteAsync(BasicList<string> sqlList, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        await DoWorkAsync(async (capture, trans) =>
        {
            await sqlList.ForEachAsync(async items =>
            {
                await capture.ExecuteAsync(items, null, trans, null, CommandType.Text);
            });
            trans.Commit();
        }, isolationLevel);
    }
    public void Execute(BasicList<string> sqlList, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        DoWork((capture, trans) =>
        {
            sqlList.ForEach(items =>
            {
                capture.Execute(items, null, trans, null, CommandType.Text);
            });
            trans.Commit();
        }, isolationLevel);
    }
    public bool Exists<E>(int id) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        bool rets = false;
        DoWork(capture =>
        {
            var list = StartConditionWithID(id);
            rets = capture.Exists<E>(list);
        });
        return rets;
    }
    public bool Exists<E>(BasicList<ICondition> conditions) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        bool rets = false;
        DoWork(capture =>
        {
            rets = capture.Exists<E>(conditions);
        });
        return rets;
    }
    #endregion
    #region Direct To Extensions For Getting
    public R GetSingleObject<E, R>(string property, int id)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandExecuteScalar<E, R>
        where R : IParsable<R>
    {
        R output = default!;
        RunCustomConnection(capture =>
        {
            var conditions = StartConditionWithID(id);

            output = capture.GetSingleObject<E, R>(property, [], conditions);
        });
        return output;
    }
    public R GetSingleObject<E, R>(string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandExecuteScalar<E, R>
        where R : IParsable<R>
    {
        R output = default!;
        RunCustomConnection(capture =>
        {
            output = capture.GetSingleObject<E, R>(property, sortList, conditions);
        });
        return output;
    }
    public async Task<R?> GetSingleObjectAsync<E, R>(string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandExecuteScalar<E, R>
        where R : IParsable<R>
    {
        R? output = default!;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetSingleObjectAsync<E, R>(property, sortList, conditions);
        });
        return output;
    }
    public BasicList<R> GetObjectList<E, R>(string property, BasicList<ICondition>? conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E, R>
    {
        BasicList<R> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.GetObjectList<E, R>(property, conditions, sortList, howMany);
        });
        return output;
    }
    public async Task<BasicList<R>> GetObjectListAsync<E, R>(string property, BasicList<ICondition>? conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E, R>
    {
        BasicList<R> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetObjectListAsync<E, R>(property, conditions, sortList, howMany);
        });
        return output;
    }
    public E Get<E>(int id)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        E output = default!;
        RunCustomConnection(capture =>
        {
            output = capture.Get<E>(id);
        });
        return output;
    }
    public BasicList<E> Get<E>(BasicList<SortInfo>? sortList = null, int howMany = 0) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.Get<E>(sortList, howMany);
        });
        return output;
    }
    public async Task<E> GetAsync<E>(int id) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        E output = default!;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E>(id);
        });
        return output;
    }
    public async Task<IEnumerable<E>> GetAsync<E>(BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E>(sortList, howMany);
        });
        return output;
    }
    public BasicList<E> Get<E>(BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.Get<E>(conditions, sortList, howMany);
        });
        return output;
    }
    public async Task<BasicList<E>> GetAsync<E>(BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E>(conditions, sortList, howMany);
        });
        return output;
    }
    public E Get<E, D1>(int id)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        E output = default!;
        RunCustomConnection(capture =>
        {
            output = capture.Get<E, D1>(id);
        });
        return output;
    }
    public BasicList<E> Get<E, D1>(BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.Get<E,  D1>(sortList, howMany);
        });
        return output;
    }
    public async Task<E> GetAsync<E, D1>(int id)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        E output = default!;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E, D1>(id);
        });
        return output;
    }
    public async Task<BasicList<E>> GetAsync<E, D1>(BasicList<SortInfo>? sortList, int howMany = 0)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E, D1>(sortList, howMany);
        });
        return output;
    }
    public BasicList<E> Get<E, D1>(BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.Get<E, D1>(conditionList, sortList, howMany);
        });
        return output;
    }
    public async Task<BasicList<E>> GetAsync<E, D1>(BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int howMany = 0)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetAsync<E, D1>(conditionList, sortList, howMany);
        });
        return output;
    }
    public E GetOneToMany<E, D1>(int id)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        E output = default!;
        RunCustomConnection(capture =>
        {
            output = capture.GetOneToMany<E, D1>(id);
        });
        return output;
    }
    public BasicList<E> GetOneToMany<E, D1>(BasicList<SortInfo>? sortList = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.GetOneToMany<E, D1>(sortList);
        });
        return output;
    }
    public BasicList<E> GetOneToMany<E, D1>(BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            output = capture.GetOneToMany<E, D1>(conditionList, sortList);
        });
        return output;
    }
    public async Task<BasicList<E>> GetOneToManyAsync<E, D1>(BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetOneToManyAsync<E, D1>(conditionList, sortList);
        });
        return output;
    }
    public async Task<E> GetOneToManyAsync<E, D1>(int id)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        E output = default!;
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetOneToManyAsync<E, D1>(id);
        });
        return output;
    }
    public async Task<IEnumerable<E>> GetOneToManyAsync<E, D1>(BasicList<SortInfo>? sortList = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            output = await capture.GetOneToManyAsync<E, D1>(sortList);
        });
        return output;
    }
    public BasicList<E> GetDataList<E>()
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
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
    public async Task<BasicList<E>> GetDataListAsync<E>() 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>, ICommandQuery<E>
    {
        BasicList<E> output = [];
        await DoWorkAsync(async capture =>
        {
            output = await capture.GetAsync<E>();
        });
        return output;
    }
    public BasicList<E> LoadData<E>(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
        where E : class, ICommandQuery<E>
    {
        BasicList<E> output = [];
        RunCustomConnection(capture =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = capture.Query<E>(command);
        });
        return output;
    }
    public BasicList<E> LoadData<E>(string sqlStatement, BasicList<DynamicParameter> parameters)
        where E : class, ICommandQuery<E>
    {
        return LoadData<E>(sqlStatement, parameters, false);
    }
    public BasicList<E> LoadData<E>(string sqlStatement)
        where E : class, ICommandQuery<E>
    {
        return LoadData<E>(sqlStatement, [], false);
    }
    public BasicList<E> LoadData<E>(string sqlStatement, bool isStoredProcedure)
        where E : class, ICommandQuery<E>
    {
        return LoadData<E>(sqlStatement, [], isStoredProcedure);
    }
    public async Task<BasicList<E>> LoadDataAsync<E>(string sqlStatement, BasicList<DynamicParameter> parameters, bool isStoredProcedure)
        where E : class, ICommandQuery<E>
    {
        BasicList<E> output = [];
        await RunCustomConnectionAsync(async capture =>
        {
            CommandType commandType = CommandType.Text;
            if (isStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }
            CommandDefinition command = new(sqlStatement, parameters, commandType: commandType);
            output = await capture.QueryAsync<E>(command);
        });
        return output;
    }
    public async Task<BasicList<E>> LoadDataAsync<E>(string sqlStatement, BasicList<DynamicParameter> parameters)
        where E : class, ICommandQuery<E>
    {
        return await LoadDataAsync<E>(sqlStatement, parameters, false);
    }
    public async Task<BasicList<E>> LoadDataAsync<E>(string sqlStatement)
        where E : class, ICommandQuery<E>
    {
        return await LoadDataAsync<E>(sqlStatement, [], false);
    }
    public async Task<BasicList<E>> LoadDataAsync<E>(string sqlStatement, bool isStoredProcedure)
        where E : class, ICommandQuery<E>
    {
        return await LoadDataAsync<E>(sqlStatement, [], isStoredProcedure);
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