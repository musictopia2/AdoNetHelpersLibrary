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
    public static void Delete<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        CompleteSqlData data = PrivateDeleteSingleItem<E>(id);
        capture.Execute(data, thisTran, connectionTimeOut);
    }
}