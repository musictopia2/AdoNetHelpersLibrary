using CommonBasicLibraries.CrudRepositoryHelpers; //not common enough
namespace AdoNetHelpersLibrary.RepositoryHelpers;
//has to be in the adonet helpers library because of basicconnector
public class SQLRepository<TEntity> : IRepositoryQueryAdvanced<TEntity>
    where TEntity : class, ISimpleDatabaseEntity, ICommandQuery<TEntity>, ITableMapper<TEntity>
{
    readonly BasicConnector _connector;
    public SQLRepository(string key, IDatabaseConnectionManager manager)
    {
        EnumDatabaseCategory category = manager.PrepareDatabase();
        _connector = new(key, category);
    }
    async Task IRepository<TEntity>.DeleteAllAsync()
    {
        //this is to delete everything from the table (but don't delete the tables themselves)
        await _connector.DoWorkAsync(async (capture, trans) =>
        {
            await DeleteAllAsync(capture, trans);
            trans.Commit(); //at this point commit transaction
        });
    }
    //so if you want to do something else, you can.
    protected virtual async Task DeleteAllAsync(ICaptureCommandParameter capture, IDbTransaction transaction)
    {
        await capture.DeleteEntireTableAsync<TEntity>(transaction); //try this.
    }
    async Task<bool> IRepository<TEntity>.DeleteAsync(TEntity entityToDelete)
    {
        //i am guessing if it can't delete, then return false because it does not exist.
        bool successful = false;
        await _connector.DoWorkAsync(async (capture, trans) =>
        {
            successful = capture.Exists<TEntity>(entityToDelete.ID, trans);
            if (successful)
            {
                await DeleteAsync(entityToDelete, capture, trans);
            }
        });
        return successful;
    }
    protected virtual async Task DeleteAsync(TEntity entityToDelete, ICaptureCommandParameter capture, IDbTransaction transaction)
    {
        await capture.DeleteAsync(entityToDelete, transaction); //has to check to make sure it exist first.
    }
    async Task<bool> IRepository<TEntity>.DeleteByIdAsync(object id)
    {
        bool successful = false;
        if (int.TryParse(id.ToString(), out int key) == false)
        {
            throw new CustomBasicException("Cannot get by id because currently, only integer primary key are supported");
        }
        await _connector.DoWorkAsync(async (capture, tran) =>
        {
            successful = capture.Exists<TEntity>(key, tran);
            if (successful)
            {
                await DeleteAsync(key, capture, tran);
            }
        });
        return successful;
    }
    protected virtual async Task DeleteAsync(int key, ICaptureCommandParameter capture, IDbTransaction transaction)
    {
        await capture.DeleteAsync<TEntity>(key, transaction); //has to check to make sure it exist first.
    }
    async Task<IEnumerable<TEntity>> IRepository<TEntity>.GetAllAsync()
    {
        return await GetAllAsync();
    }
    //so if you need to do a different way because of joins, you can.
    protected virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        BasicList<TEntity> list = await _connector.GetDataListAsync<TEntity>();
        return list; //this should be okay.
    }
    //this should not support joins since query filters won't do joins.
    async Task<IEnumerable<TEntity>> IRepository<TEntity>.GetAsync(QueryFilter<TEntity> filter)
    {
        int top = filter.Top; //this gives better flexibility than the original version.
        var sort = GetSortList(filter);
        var conditions = GetConditions(filter);
        BasicList<TEntity> list = [];
        await _connector.DoWorkAsync(async capture =>
        {
            list = await capture.GetAsync<TEntity>(conditions, sort, top);
        });
        return list;
    }
    //the queryfilter only supports sorting by one thing alone.
    private static BasicList<ICondition> GetConditions(QueryFilter<TEntity> filter)
    {
        BasicList<ICondition> conditions = [];
        foreach (var item in filter.FilterProperties)
        {
            AndCondition condition = new();
            condition.Property = item.Name;
            condition.Value = item.Value;
            condition.Operator = item.Operator switch
            {
                EnumFilterOperator.Equals => cs1.Equals,
                EnumFilterOperator.NotEquals => cs1.NotEqual,
                EnumFilterOperator.LessThan => cs1.LessThan,
                EnumFilterOperator.LessThanOrEqual => cs1.LessThanOrEqual,
                EnumFilterOperator.Contains => cs1.Like,
                EnumFilterOperator.GreaterThan => cs1.GreaterThan,
                EnumFilterOperator.GreaterThanOrEqual => cs1.GreaterOrEqual,
                //EnumFilterOperator.StartsWith => throw new CustomBasicException("Unable to support 'StartsWith' currently."),
                //EnumFilterOperator.EndsWith => throw new CustomBasicException("Unable to support 'EndsWith' currently."),
                _ => throw new InvalidOperationException($"Unsupported operator: {item.Operator}"),
            };
            conditions.Add(condition);
        }
        return conditions;
    }
    private static BasicList<SortInfo>? GetSortList(QueryFilter<TEntity> filter)
    {
        if (filter.OrderByPropertyName != "")
        {
            BasicList<SortInfo> output = [];
            SortInfo first = new();
            first.Property = filter.OrderByPropertyName;
            if (filter.OrderByDescending)
            {
                first.OrderBy = SortInfo.EnumOrderBy.Descending;
            }
            else
            {
                first.OrderBy = SortInfo.EnumOrderBy.Ascending;
            }
            output.Add(first);
        }
        return null;
    }
    async Task<TEntity> IRepository<TEntity>.GetByIdAsync(object id)
    {
        TEntity? output = null;
        if (int.TryParse(id.ToString(), out int key) == false)
        {
            throw new CustomBasicException("Cannot get by id because currently, only integer primary key are supported");
        }
        await _connector.DoWorkAsync(async (capture) =>
        {
            output = await GetByIdAsync(key, capture);
        });
        if (output is null)
        {
            throw new CustomBasicException("Did not find by id");
        }
        return output;
    }
    protected virtual async Task<TEntity> GetByIdAsync(int key, ICaptureCommandParameter capture)
    {
        var output = await capture.GetAsync<TEntity>(key);
        return output;
    }
    //there can be cases where something else needs to insert.  the overrided classes has to figure that out.
    async Task<TEntity> IRepository<TEntity>.InsertAsync(TEntity entity)
    {
        await _connector.DoWorkAsync(async (capture, tran) =>
        {
            await capture.InsertSingleAsync(entity, tran);
            tran.Commit();
        });
        //easiest here too.
        return entity;
    }
    protected virtual async Task<TEntity> InsertAsync(ICaptureCommandParameter capture, TEntity entity, IDbTransaction tran)
    {
        entity.ID = await capture.InsertSingleAsync(entity, tran);
        if (entity.ID == 0)
        {
            throw new CustomBasicException("Did not insert");
        }
        return entity;
    }

    async Task<TEntity> IRepository<TEntity>.UpdateAsync(TEntity entityToUpdate)
    {
        await _connector.DoWorkAsync(async (capture, tran) =>
        {
            await UpdateAsync(capture, entityToUpdate, tran);
            tran.Commit();
        });
        return entityToUpdate;
    }
    protected virtual async Task<TEntity> UpdateAsync(ICaptureCommandParameter capture, TEntity entity, IDbTransaction tran)
    {
        await capture.UpdateEntityAsync(entity, EnumUpdateCategory.All, tran);
        return entity;
    }
    async Task<IEnumerable<TEntity>> IRepositoryQueryAdvanced<TEntity>.GetAsync(BasicList<ICondition> conditions, BasicList<SortInfo>? sortList, int top)
    {
        IEnumerable<TEntity> list = [];
        await _connector.DoWorkAsync(async (capture, trans) =>
        {
            list = await GetAsync(conditions, sortList, top, capture, trans);
        });
        return list;
    }
    //i am guessing the more complex part can do joins (since i have done before).
    protected virtual async Task<IEnumerable<TEntity>> GetAsync(BasicList<ICondition> conditions, BasicList<SortInfo>? sortList, int top, ICaptureCommandParameter capture, IDbTransaction transaction)
    {
        return await capture.GetAsync<TEntity>(conditions, sortList, top, transaction);
    }
}