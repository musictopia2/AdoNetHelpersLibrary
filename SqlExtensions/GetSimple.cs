namespace AdoNetHelpersLibrary.SqlExtensions;
public static class GetSimple
{
    #region Single Tables
    public static BasicList<E> Get<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        return capture.PrivateSimpleSelectAll<E>(sortList, howMany, thisTran, connectionTimeOut);
    }
    public async static Task<BasicList<E>> GetAsync<E>(this ICaptureCommandParameter capture, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory category = capture.Category;

        //EnumDatabaseCategory category = db.GetDatabaseCategory(conn);
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
    #endregion
}