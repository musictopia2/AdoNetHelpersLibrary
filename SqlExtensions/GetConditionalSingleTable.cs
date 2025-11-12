namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetConditionalSingleTable
{
    public static BasicList<E> Get<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        return capture.PrivateSimpleSelectConditional<E>(conditions, sortList, howMany, thisTran, connectionTimeOut).ToBasicList();
    }
    public async static Task<BasicList<E>> GetAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        return await capture.PrivateSimpleSelectConditionalAsync<E>(conditions, sortList, howMany, thisTran, connectionTimeOut);
    }
    private static BasicList<E> PrivateSimpleSelectConditional<E>(this ICaptureCommandParameter capture, BasicList<ICondition> Conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, Conditions, sortList, database, howMany: howMany);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        return capture.Query<E>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
    private async static Task<BasicList<E>> PrivateSimpleSelectConditionalAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, sortList, database, howMany: howMany);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        return await capture.QueryAsync<E>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
}