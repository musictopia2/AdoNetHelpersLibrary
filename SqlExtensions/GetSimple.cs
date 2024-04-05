namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetSimple
{
    #region Single Tables
    public static E Get<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        BasicList<E> results = capture.PrivateGetSingleItem<E>(id, thisTran, connectionTimeOut);
        return results.Single();
    }
    public static async Task<E> GetAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        BasicList<E> results = await capture.PrivateGetSingleItemAsync<E>(id, thisTran, connectionTimeOut);
        return results.Single();
    }
    public static BasicList<E> Get<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        return capture.PrivateSimpleSelectAll<E>(sortList, howMany, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, MapList) = GetSimpleSelectStatement<E>(category, howMany);
        if (sortList != null)
        {
            if (MapList.Count == 0)
            {
                throw new CustomBasicException("Needs maps");
            }
            StringBuilder thisStr = new(sqls);
            thisStr.Append(GetSortStatement(MapList, sortList, false));
            sqls = thisStr.ToString();
        }
        return await capture.QueryAsync<E>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static BasicList<E> PrivateSimpleSelectAll<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, MapList) = GetSimpleSelectStatement<E>(category, howMany);
        if (sortList != null)
        {
            if (MapList.Count == 0)
            {
                throw new CustomBasicException("Needs maps");
            }
            StringBuilder thisStr = new(sqls);
            thisStr.Append(GetSortStatement(MapList, sortList, false));
            sqls = thisStr.ToString();
        }
        return capture.Query<E>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static BasicList<E> PrivateGetSingleItem<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        var (sqls, _) = GetSimpleSelectStatement<E>(category);
        builder.Append(sqls);
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, false);
        return capture.Query<E>(builder.ToString(), parameters, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static Task<BasicList<E>> PrivateGetSingleItemAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        var (sqls, _) = GetSimpleSelectStatement<E>(category);
        builder.Append(sqls);
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, false);
        return capture.QueryAsync<E>(builder.ToString(), parameters, thisTran, connectionTimeOut, CommandType.Text);
    }
    #endregion
    #region Two Table One On One

    public static E Get<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        IEnumerable<E> Results = capture.PrivateGetOneToOneItem(id, action, thisTran, connectionTimeOut);
        return Results.Single();
    }
    public static BasicList<E> Get<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, int howMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        return capture.PrivateOneToOneSelectAll(sortList, howMany, action, thisTran, connectionTimeOut);
    }
    public async static Task<E> GetAsync<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        IEnumerable<E> Results = await capture.PrivateGetOneToOneItemAsync(id, action, thisTran, connectionTimeOut);
        return Results.Single();
    }
    public async static Task<BasicList<E>> GetAsync<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, int howMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;

        string sqls = GetSimpleSelectStatement<E, D1>(true, sortList, category, howMany);
        return await capture.QueryAsync<E, D1, E>(sqls, (Main, Detail) => OneToOneAction(Main, Detail, action), null, thisTran, commandTimeout: connectionTimeOut);
    }
    private static BasicList<E> PrivateGetOneToOneItem<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        builder.Append(GetSimpleSelectStatement<E, D1>(true, null, category, 0)); //its implied because of id.
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, true);
        return capture.Query<E, D1, E>(builder.ToString(), (Main, Detail) => OneToOneAction(Main, Detail, action), parameters, thisTran, commandTimeout: connectionTimeOut);
    }
    private static BasicList<E> PrivateOneToOneSelectAll<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, int howMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        string sqls = GetSimpleSelectStatement<E, D1>(true, sortList, category, howMany);
        return capture.Query<E, D1, E>(sqls, (Main, Detail) => OneToOneAction(Main, Detail, action), null, thisTran, commandTimeout: connectionTimeOut);
    }
    private async static Task<BasicList<E>> PrivateGetOneToOneItemAsync<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        builder.Append(GetSimpleSelectStatement<E, D1>(true, null, category, 0));
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, true);
        return await capture.QueryAsync<E, D1, E>(builder.ToString(), (Main, Detail) => OneToOneAction(Main, Detail, action), parameters, thisTran, commandTimeout: connectionTimeOut);
    }
    public static E OneToOneAction<E, D1>(E main, D1? detail)
        where E : class, IJoinedEntity<D1>
        where D1 : class, ISimpleDatabaseEntity
    {
        if (detail is null)
        {
            return main;
        }
        main.AddRelationships(detail);
        return main;
    }
    public static E OneToOneAction<E, D1>(E main, D1? detail, Action<E, D1?>? action)
        where E : class, IJoinedEntity<D1>
        where D1 : class, ISimpleDatabaseEntity
    {
        if (detail is null)
        {
            action?.Invoke(main, detail);
            return main;
        }
        main.AddRelationships(detail);
        action?.Invoke(main, detail);
        return main;
    }
    #endregion
    #region Two Table One To Many
    public static E GetOneToManyAction<E, D1>(E main, D1? detail, Action<E, D1?>? action, Dictionary<int, E> thisDict)
        where E : class, IJoinedEntity<D1>
        where D1: class, ISimpleDatabaseEntity
    {
        if (detail == null)
        {
            action?.Invoke(main, detail);
            return main;
        }
        bool had = false;
        if (thisDict.TryGetValue(main.ID, out E? thisTemp) == false)
        {
            thisTemp = main;
            thisDict.Add(main.ID, thisTemp);
            had = true;
        }
        thisTemp.AddRelationships(detail);
        if (action != null && had == true)
        {
            action.Invoke(main, detail);
        }
        return thisTemp;
    }
    public static E GetOneToMany<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ICommandQuery<E, D1, E>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        IEnumerable<E> results = capture.PrivateGetOneToManyItem(id, action, thisTran, connectionTimeOut);
        return results.Single();
    }
    public static BasicList<E> GetOneToMany<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        return capture.PrivateOneToManySelectAll(sortList, action, thisTran, connectionTimeOut);
    }
    public async static Task<E> GetOneToManyAsync<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        BasicList<E> results = await capture.PrivateGetOneToManyItemAsync(id, action, thisTran, connectionTimeOut);
        return results.Single();
    }
    public async static Task<BasicList<E>> GetOneToManyAsync<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        string sqls = GetSimpleSelectStatement<E, D1>(false, sortList, category, 0);
        Dictionary<int, E> thisDict = [];
        var thisList = await capture.QueryAsync<E, D1, E>(sqls, (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), null, thisTran, commandTimeout: connectionTimeOut);
        return thisList.Distinct().ToBasicList();
    }
    private static BasicList<E> PrivateGetOneToManyItem<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        StringBuilder builder = new();
        builder.Append(GetSimpleSelectStatement<E, D1>(false, null, category, 0));
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, true);
        Dictionary<int, E> thisDict = [];
        return capture.Query<E, D1, E>(builder.ToString(), (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), parameters, thisTran, commandTimeout: connectionTimeOut).Distinct().ToBasicList();
    }
    private static BasicList<E> PrivateOneToManySelectAll<E, D1>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        string sqls = GetSimpleSelectStatement<E, D1>(false, sortList, category, 0);
        Dictionary<int, E> thisDict = [];
        return capture.Query<E, D1, E>(sqls, (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), null, thisTran, commandTimeout: connectionTimeOut).Distinct().ToBasicList();
    }
    private async static Task<BasicList<E>> PrivateGetOneToManyItemAsync<E, D1>(this ICaptureCommandParameter capture, int id, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        StringBuilder builder = new();
        builder.Append(GetSimpleSelectStatement<E, D1>(false, null, category, 0));
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, true);
        Dictionary<int, E> thisDict = [];
        var list = await capture.QueryAsync<E, D1, E>(builder.ToString(), (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), parameters, thisTran, commandTimeout: connectionTimeOut);
        return list.Distinct().ToBasicList();
    }
    #endregion
}