namespace AdoNetHelpersLibrary.SqlExtensions;
public static class InsertSimple
{
    private static CompleteSqlData PrivateGetInsert<E>(E thisObj, EnumDatabaseCategory category, out bool isAutoIncremented)
        where E : class, ISimpleDapperEntity
    {
        CompleteSqlData output = new();
        isAutoIncremented = TableMapGlobalClass<E>.IsAutoIncrementing();
        var map = TableMapGlobalClass<E>.GetMap(thisObj, isAutoIncremented);
        output.SQLStatement = GetInsertStatement(category, map, isAutoIncremented);
        PopulateSimple(map.Columns, output, EnumCategory.UseDatabaseMapping);
        return output;
    }
    public static long InsertSingle<E>(this ICaptureCommandParameter connector, E thisObject, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory category = connector.Category;
        CompleteSqlData data = PrivateGetInsert(thisObject, category, out bool IsAutoIncremented);
        long id = connector.ExecuteScalar<long>(data, thisTran, connectionTimeOut); //this will focus on text alone.  may later think about stored procedures
        if (IsAutoIncremented == true)
        {
            return id;
        }
        return thisObject.ID;
    }
    public static async Task<long> InsertSingleAsync<E>(this ICaptureCommandParameter connector, E thisObject, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory category = connector.Category;
        CompleteSqlData data = PrivateGetInsert(thisObject, category, out bool IsAutoIncremented);
        //for now, will always focus on text (not stored procedures).

        long id = await connector.ExecuteScalarAsync<long>(data, thisTran, connectionTimeOut); //this will focus on text alone.  may later think about stored procedures
        if (IsAutoIncremented == true)
        {
            return id;
        }
        return thisObject.ID;
    }
}