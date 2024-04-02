namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetSimple
{
    #region Single Tables
    public static E Get<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        BasicList<E> results = capture.PrivateGetSingleItem<E>(id, thisTran, connectionTimeOut);
        return results.Single();
    }
    public static async Task<E> GetAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        BasicList<E> results = await capture.PrivateGetSingleItemAsync<E>(id, thisTran, connectionTimeOut);
        return results.Single();
    }
    public static BasicList<E> Get<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        return capture.PrivateSimpleSelectAll<E>(sortList, howMany, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, MapList) = GetSimpleSelectStatement<E>(category, howMany);
        if (sortList != null)
        {
            if (MapList.Count == 0)
            {
                throw new CustomBasicException("Needs maps");
            }
            throw new CustomBasicException("Cannot handle sorting for now");
        }
        return await capture.QueryAsync<E>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static BasicList<E> PrivateSimpleSelectAll<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory category = capture.Category;
        var (sqls, MapList) = GetSimpleSelectStatement<E>(category, howMany);
        if (sortList != null)
        {
            if (MapList.Count == 0)
            {
                throw new CustomBasicException("Needs maps");
            }
            throw new CustomBasicException("Cannot handle sorting for now");
            //StringBuilder thisStr = new(sqls);
            //thisStr.Append(GetSortStatement(MapList, sortList, false));
            //sqls = thisStr.ToString();
        }
        return capture.Query<E>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static BasicList<E> PrivateGetSingleItem<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        var (sqls, _) = GetSimpleSelectStatement<E>(category);
        builder.Append(sqls);
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, false);
        return capture.Query<E>(builder.ToString(), parameters, thisTran, connectionTimeOut, CommandType.Text);
    }
    private static Task<BasicList<E>> PrivateGetSingleItemAsync<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        StringBuilder builder = new();
        EnumDatabaseCategory category = capture.Category;
        var (sqls, _) = GetSimpleSelectStatement<E>(category);
        builder.Append(sqls);
        BasicList<DynamicParameter> parameters = GetDynamicIDData(ref builder, id, false);
        return capture.QueryAsync<E>(builder.ToString(), parameters, thisTran, connectionTimeOut, CommandType.Text);
    }
    #endregion
}