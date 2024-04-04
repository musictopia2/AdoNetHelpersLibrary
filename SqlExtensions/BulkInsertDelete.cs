namespace DapperHelpersLibrary.Extensions;
public static class BulkInsertDelete
{
    #region Insert
    private static CompleteSqlData GetDapperInsert(EnumDatabaseCategory category, SourceGeneratedMap map, bool isAutoIncremented)
    {
        CompleteSqlData output = new();
        output.SQLStatement = GetInsertStatement(category, map, isAutoIncremented);
        PopulateSimple(map.Columns, output, EnumCategory.UseDatabaseMapping);
        return output;
    }
    private static CompleteSqlData GetDapperInsert<E>(EnumDatabaseCategory category, E thisObj) where E : class, ISimpleDatabaseEntity
    {
        bool isAutoIncremented = TableMapGlobalClass<E>.IsAutoIncrementing();
        var map = TableMapGlobalClass<E>.GetMap(thisObj, isAutoIncremented, false);
        return GetDapperInsert(category, map, isAutoIncremented);
    }
    public static void InsertRange<E>(this ICaptureCommandParameter capture, BasicList<E> thisList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory category = capture.Category;
        thisList.ForEach(items =>
        {
            var thisData = GetDapperInsert(category, items);
            capture.PrivateInsertBulk(thisData, thisTran, connectionTimeOut);
        });
    }
    private static void PrivateInsertBulk(this ICaptureCommandParameter capture, CompleteSqlData thisData, IDbTransaction thisTran, int? connectionTimeOut = null)
    {
        capture.Execute(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
    private async static Task<int> PrivateInsertBulkAsync(this ICaptureCommandParameter capture, CompleteSqlData thisData, IDbTransaction thisTran, int? connectionTimeOut = null)
    {
        return await capture.ExecuteScalarAsync<int>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
    public static async Task InsertRangeAsync<E>(this ICaptureCommandParameter capture, BasicList<E> thisList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory category = capture.Category;
        await thisList.ForEachAsync(async Items =>
        {
            var ThisData = GetDapperInsert(category, Items);
            Items.ID = await capture.PrivateInsertBulkAsync(ThisData, thisTran, connectionTimeOut);
        });
    }
    #endregion
    #region Delete
    public static void DeleteRange<E>(this ICaptureCommandParameter capture, BasicList<int> deleteList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        deleteList.ForEach(xx =>
        {
            CompleteSqlData complete = PrivateDeleteSingleItem<E>(xx);
            capture.Execute(complete, thisTran, connectionTimeOut);
        });
    }
    public static async Task DeleteRangeAsync<E>(this ICaptureCommandParameter capture, BasicList<int> deleteList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        await deleteList.ForEachAsync(async xx =>
        {
            CompleteSqlData complete = PrivateDeleteSingleItem<E>(xx);
            await capture.ExecuteAsync(complete, thisTran, connectionTimeOut);
        });
    }
    public static void DeleteRange<E>(this ICaptureCommandParameter capture, BasicList<E> objectList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        BasicList<int> deleteList = objectList.GetIDList();
        deleteList.ForEach(xx =>
        {
            CompleteSqlData complete = PrivateDeleteSingleItem<E>(xx);
            capture.Execute(complete, thisTran, connectionTimeOut);
        });
    }
    public static async Task DeleteRangeAsync<E>(this ICaptureCommandParameter capture, BasicList<E> objectList, IDbTransaction thisTran, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        BasicList<int> deleteList = objectList.GetIDList();
        await deleteList.ForEachAsync(async xx =>
        {
            CompleteSqlData complete = PrivateDeleteSingleItem<E>(xx);
            await capture.ExecuteAsync(complete, thisTran, connectionTimeOut);
        });
    }
    #endregion
}