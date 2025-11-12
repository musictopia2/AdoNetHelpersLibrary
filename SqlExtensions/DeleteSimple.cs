namespace AdoNetHelpersLibrary.SqlExtensions;
public static class DeleteSimple
{
    internal static CompleteSqlData PrivateDeleteSingleItem<E>(int id)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        StringBuilder builder = new();
        var map = E.GetTableMap();
        string tablename = map.TableName;
        builder.Append(GetDeleteStatement(tablename));
        BasicList<DynamicParameter> complete = GetDynamicIDData(ref builder, id);
        return new CompleteSqlData() { Parameters = complete, SQLStatement = builder.ToString() };
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, E thisObj, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        capture.Delete<E>(thisObj.ID, thisTran, connectionTimeOut);
    }
    public static void DeleteEverythingFromTable<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        string tablename = E.TableName;
        string sqls = GetDeleteStatement(tablename);
        capture.Execute(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        CompleteSqlData data = PrivateDeleteSingleItem<E>(id);
        capture.Execute(data, thisTran, connectionTimeOut);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        CompleteSqlData data = PrivateDeleteSingleItem<E>(id);
        await capture.ExecuteAsync(data, thisTran, connectionTimeOut);
    }
    public static async Task DeleteEntireTableAsync<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        string tablename = E.TableName;
        string sqls = GetDeleteStatement(tablename);
        await capture.ExecuteAsync(sqls, null, thisTran, ConnectionTimeOut, CommandType.Text);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, E thisObj, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        await capture.DeleteAsync<E>(thisObj.ID, thisTran, connectionTimeOut);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        var map = E.GetTableMap();
        EnumDatabaseCategory database = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, null, capture.Category, EnumSQLCategory.Delete);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        await capture.ExecuteAsync(thisData, thisTran, connectionTimeOut);
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        var map = E.GetTableMap();
        EnumDatabaseCategory database = capture.Category;
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, null, capture.Category, EnumSQLCategory.Delete);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional, database);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
}