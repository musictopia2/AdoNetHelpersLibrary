namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetConditionalSingleTable
{
    private static SourceGeneratedMap GetMap<E>()
        where E : class, ISimpleDatabaseEntity
    {
        return TableMapGlobalClass<E>.GetMap();
    }
    public static BasicList<E> Get<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        return capture.PrivateSimpleSelectConditional<E>(conditions, sortList, howMany, thisTran, connectionTimeOut).ToBasicList();
    }
    public async static Task<BasicList<E>> GetAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        return await capture.PrivateSimpleSelectConditionalAsync<E>(conditions, sortList, howMany, thisTran, connectionTimeOut);
    }
    private static BasicList<E> PrivateSimpleSelectConditional<E>(this ICaptureCommandParameter capture, BasicList<ICondition> Conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, Conditions, sortList, database, howMany: howMany);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return capture.Query<E>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
    private async static Task<BasicList<E>> PrivateSimpleSelectConditionalAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, sortList, database, howMany: howMany);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return await capture.QueryAsync<E>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
}