namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetOneItem
{
    public static R GetSingleObject<E, R>(this ICaptureCommandParameter capture, string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandExecuteScalar<E, R>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, sortList, database, howMany: 1, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        }
        return capture.ExecuteScalar<E, R>(thisData, thisTran, connectionTimeOut)!;
    }
    public static async Task<R> GetSingleObjectAsync<E, R>(this ICaptureCommandParameter capture, string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandExecuteScalar<E, R>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, sortList, database, howMany: 1, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        }
        var output = await capture.ExecuteScalarAsync<E, R>(thisData, thisTran!, connectionTimeOut)!;
        if (output != null)
        {
            return output;
        }
        return default!;
    }
    public static BasicList<R> GetObjectList<E, R>(this ICaptureCommandParameter capture, string property, BasicList<ICondition>? conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E, R>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, sortList, database, howMany: howMany, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        }
        return capture.Query<E, R>(thisData.SQLStatement, thisData.Parameters, thisTran, connectionTimeOut, CommandType.Text);
    }
    public static async Task<BasicList<R>> GetObjectListAsync<E, R>(this ICaptureCommandParameter capture, string property, BasicList<ICondition>? Conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ICommandQuery<E, R>, ITableMapper<E>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, Conditions, sortList, database, howMany: howMany, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (Conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        }
        return await capture.QueryAsync<E, R>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
}