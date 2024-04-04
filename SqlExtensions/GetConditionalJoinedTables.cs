namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetConditionalJoinedTables
{
    #region joined 2 Tables
    public static BasicList<E> Get<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, IJoinedEntity<D1> where D1 : class, ISimpleDatabaseEntity
    {
        return capture.PrivateOneToOneSelectConditional(conditionList, sortList, HowMany, action, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1>? action = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, IJoinedEntity<D1> where D1 : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(conditionList, sortList, true, category, HowMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional);
        return await capture.QueryAsync<E, D1, E>(sqls, (Main, Detail) => PrivateOneToOne(Main, Detail, action), data.Parameters, thisTran, commandTimeout: connectionTimeOut);
    }
    private static BasicList<E> PrivateOneToOneSelectConditional<E, D1>(this ICaptureCommandParameter capture, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList = null, int HowMany = 0, Action<E, D1>? action = null, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null) where E : class, IJoinedEntity<D1> where D1 : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement<E, D1>(conditionList, sortList, true, category, HowMany);
        CompleteSqlData data = new();
        data.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, data, EnumCategory.Conditional);
        return capture.Query<E, D1, E>(sqls, (Main, Detail) => PrivateOneToOne(Main, Detail, action), data.Parameters, thisTran, commandTimeout: ConnectionTimeOut);
    }
    #endregion
}
