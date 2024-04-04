namespace AdoNetHelpersLibrary.SqlExtensions;
public static class InsertSimple
{
    private static CompleteSqlData PrivateGetInsert<E>(E thisObj, EnumDatabaseCategory category, out bool isAutoIncremented)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        CompleteSqlData output = new();
        isAutoIncremented = E.IsAutoIncremented;
        var map = E.GetTableMap(thisObj, isAutoIncremented);
        output.SQLStatement = GetInsertStatement(category, map, isAutoIncremented);
        PopulateSimple(map.Columns, output, EnumCategory.UseDatabaseMapping);
        return output;
    }
    public static int InsertSingle<E>(this ICaptureCommandParameter connector, E thisObject, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        EnumDatabaseCategory category = connector.Category;
        CompleteSqlData data = PrivateGetInsert(thisObject, category, out bool IsAutoIncremented);
        int id = connector.ExecuteScalar<int>(data, thisTran, connectionTimeOut); //this will focus on text alone.  may later think about stored procedures
        if (IsAutoIncremented == true)
        {
            return id;
        }
        return thisObject.ID;
    }
    public static async Task<int> InsertSingleAsync<E>(this ICaptureCommandParameter connector, E thisObject, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        EnumDatabaseCategory category = connector.Category;
        CompleteSqlData data = PrivateGetInsert(thisObject, category, out bool IsAutoIncremented);
        //for now, will always focus on text (not stored procedures).

        int id = await connector.ExecuteScalarAsync<int>(data, thisTran, connectionTimeOut); //this will focus on text alone.  may later think about stored procedures
        if (IsAutoIncremented == true)
        {
            return id;
        }
        return thisObject.ID;
    }
}