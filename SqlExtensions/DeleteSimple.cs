namespace AdoNetHelpersLibrary.SqlExtensions;
public static class DeleteSimple
{
    internal static CompleteSqlData PrivateDeleteSingleItem<E>(int id) where E : class, ISimpleDapperEntity
    {
        StringBuilder builder = new();
        var map = TableMapGlobalClass<E>.GetMap();
        string tablename = map.TableName;
        builder.Append(GetDeleteStatement(tablename));
        BasicList<DynamicParameter> complete = GetDynamicIDData(ref builder, id);
        return new CompleteSqlData() { Parameters = complete, SQLStatement = builder.ToString() };
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, E thisObj, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        capture.Delete<E>(thisObj.ID, thisTran, connectionTimeOut);
    }
    public static void DeleteEverythingFromTable<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        string tablename = TableMapGlobalClass<E>.TableName();
        string sqls = GetDeleteStatement(tablename);
        capture.Execute(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        CompleteSqlData data = PrivateDeleteSingleItem<E>(id);
        capture.Execute(data, thisTran, connectionTimeOut);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        CompleteSqlData data = PrivateDeleteSingleItem<E>(id);
        await capture.ExecuteAsync(data, thisTran, connectionTimeOut);
    }
    public static async Task DeleteEntireTableAsync<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? ConnectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        string tablename = TableMapGlobalClass<E>.TableName();
        string sqls = GetDeleteStatement(tablename);
        await capture.ExecuteAsync(sqls, null, thisTran, ConnectionTimeOut, CommandType.Text);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, E thisObj, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        await capture.DeleteAsync<E>(thisObj.ID, thisTran, connectionTimeOut);
    }
    public static async Task DeleteAsync<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var map = TableMapGlobalClass<E>.GetMap();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, null, capture.Category, EnumSQLCategory.Delete);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        await capture.ExecuteAsync(thisData, thisTran, connectionTimeOut);
    }
    public static void Delete<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var map = TableMapGlobalClass<E>.GetMap();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, null, capture.Category, EnumSQLCategory.Delete);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
}