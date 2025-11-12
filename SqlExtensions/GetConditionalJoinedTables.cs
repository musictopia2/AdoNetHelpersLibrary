namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetConditionalJoinedTables
{
    #region 2 Tables One To Many 
    public static BasicList<E> GetOneToMany<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        return capture.PrivateOneToManySelectConditional(conditionList, sortList, action, thisTran, ConnectionTimeOut);
    }
    public async static Task<BasicList<E>> GetOneToManyAsync<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(conditionList, sortList, false, category, 0);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        Dictionary<int, E> thisDict = [];
        var thisList = await capture.QueryAsync<E, D1, E>(sqls, (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), data.Parameters, thisTran, commandTimeout: connectionTimeOut);
        return thisList.Distinct().ToBasicList();
    }
    private static BasicList<E> PrivateOneToManySelectConditional<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> ConditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1>, ITableMapper<E>, ICommandQuery<E, D1, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(ConditionList, sortList, false, category, 0);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        Dictionary<int, E> thisDict = [];
        return capture.Query<E, D1, E>(sqls, (Main, Detail) => GetOneToManyAction(Main, Detail, action, thisDict), data.Parameters, thisTran, commandTimeout: connectionTimeOut).Distinct().ToBasicList();
    }
    #endregion
    #region joined 2 Tables
    public static BasicList<E> Get<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ICommandQuery<E, D1, E>, IJoinedEntity<D1>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        return capture.PrivateOneToOneSelectConditional(conditionList, sortList, HowMany, action, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ICommandQuery<E, D1, E>, IJoinedEntity<D1>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(conditionList, sortList, true, category, HowMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        return await capture.QueryAsync<E, D1, E>(sqls, (Main, Detail) => OneToOneAction(Main, Detail, action), data.Parameters, thisTran, commandTimeout: connectionTimeOut);
    }
    private static BasicList<E> PrivateOneToOneSelectConditional<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1?>? action = null, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null)
        where E : class, ICommandQuery<E, D1, E>, IJoinedEntity<D1>, ITableMapper<E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(conditionList, sortList, true, category, HowMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        return capture.Query<E, D1, E>(sqls, (Main, Detail) => OneToOneAction(Main, Detail, action), data.Parameters, thisTran, commandTimeout: ConnectionTimeOut);
    }
    #endregion
    #region Join 3 Tables
    public static BasicList<E> Get<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int howMany = 0, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2 : class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        return capture.PrivateOneToOneSelectConditional(conditionList, sortList, howMany, action, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int howMany = 0, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2 : class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1, D2>(conditionList, sortList, category, howMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        return await capture.QueryAsync<E, D1, D2, E>(sqls, (Main, Detail1, Detail2) => OneToOneAction(Main, Detail1, Detail2, action), data.Parameters, thisTran, commandTimeout: connectionTimeOut);
    }
    private static BasicList<E> PrivateOneToOneSelectConditional<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int howMany = 0, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2 : class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1, D2>(conditionList, sortList, category, howMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        return capture.Query<E, D1, D2, E>(sqls, (Main, Detail1, Detail2) => OneToOneAction(Main, Detail1, Detail2, action), data.Parameters, thisTran, commandTimeout: ConnectionTimeOut);
    }
    #endregion
    #region 3 Tables One To Many
    public static BasicList<E> GetOneToMany<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2: class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        return capture.PrivateOneToManySelectConditional(conditionList, sortList, action, thisTran, ConnectionTimeOut);
    }
    public async static Task<BasicList<E>> GetOneToManyAsync<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2 : class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1, D2>(conditionList, sortList, category, 0, false);
        //var (sqls, ParameterMappings) = GetConditionalStatement<E, D1, D2>(conditionList, sortList, false, category, 0);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        Dictionary<int, E> thisDict = [];
        var thisList = await capture.QueryAsync<E, D1, D2, E>(sqls, (Main, Detail1, Detail2) => GetOneToManyAction(Main, Detail1, Detail2, action, thisDict), data.Parameters, thisTran, commandTimeout: connectionTimeOut);
        return thisList.Distinct().ToBasicList();
    }
    private static BasicList<E> PrivateOneToManySelectConditional<E, D1, D2>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, Action<E, D1?, D2?>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IJoinedEntity<D1, D2>, ITableMapper<E>, ICommandQuery<E, D1, D2, E>
        where D1 : class, ISimpleDatabaseEntity, ITableMapper<D1>
        where D2 : class, ISimpleDatabaseEntity, ITableMapper<D2>
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1, D2>(conditionList, sortList, category, 0, false);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional, category);
        Dictionary<int, E> thisDict = [];
        return capture.Query<E, D1, D2, E>(sqls, (Main, Detail1, Detail2) => GetOneToManyAction(Main, Detail1, Detail2, action, thisDict), data.Parameters, thisTran, commandTimeout: connectionTimeOut).Distinct().ToBasicList();
    }
    #endregion
}
