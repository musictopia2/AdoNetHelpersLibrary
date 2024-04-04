namespace AdoNetHelpersLibrary.SqlHelpers;
internal class SimpleStatementHelpers
{
    public enum EnumSQLCategory
    {
        Normal,
        Count,
        Bool,
        Delete
    }
    public static string GetInsertStatement(EnumDatabaseCategory category, SourceGeneratedMap map, bool isAutoIncremented)
    {
        if (category == EnumDatabaseCategory.None)
        {
            throw new CustomBasicException("Must choose what database to use");
        }
        StringBuilder thisStr = new("insert into ");
        thisStr.Append(map.TableName);
        thisStr.Append(" (");
        StrCat cat1 = new();
        StrCat cat2 = new();

        map.Columns.ForEach(column =>
        {
            cat1.AddToString(column.ColumnName, ", ");
            cat2.AddToString($"@{column.ColumnName}", ", ");
        });

        thisStr.Append(cat1.GetInfo());
        thisStr.Append(") values (");
        thisStr.Append(cat2.GetInfo());
        thisStr.Append(')');
        if (isAutoIncremented == true)
        {
            if (category == EnumDatabaseCategory.SQLite)
            {
                thisStr.Append("; SELECT last_insert_rowid()");
            }
            else if (category == EnumDatabaseCategory.SQLServer)
            {
                thisStr.Append("; SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
            }
            else if (category == EnumDatabaseCategory.MySQL)
            {
                thisStr.Append("; SELECT LAST_INSERT_ID();");
            }
            else
            {
                throw new CustomBasicException("Not Supported");
            }
        }
        return thisStr.ToString();
    }
    public static (string sqls, BasicList<ColumnModel> MapList) GetSimpleSelectStatement<E>(EnumDatabaseCategory database, int howMany = 0) where E : class, ISimpleDatabaseEntity
    {
        string tableName;
        var map = TableMapGlobalClass<E>.GetMap();
        tableName = map.TableName;
        string thisStr = GetSimpleSelectStatement(map.Columns, tableName, database, howMany: howMany);
        return (thisStr, map.Columns);
    }
    public static string GetSimpleSelectStatement(BasicList<ColumnModel> thisList, string tableName, EnumDatabaseCategory database, EnumSQLCategory category = EnumSQLCategory.Normal, int howMany = 0, string property = "")
    {
        StringBuilder thisStr = new("select ");
        if (howMany > 0 && database == EnumDatabaseCategory.SQLServer)
        {
            thisStr.Append($"top {howMany} ");
        }
        if (category == EnumSQLCategory.Normal && property == "")
        {
            if (thisList.TrueForAll(xx => xx.HasMatch == true))
            {
                thisStr.Append(" * from ");
                thisStr.Append(tableName);
                return thisStr.ToString();
            }
            StrCat cats = new();
            thisList.ForEach(xx =>
            {
                if (xx.HasMatch == false)
                {
                    cats.AddToString($"{xx.ColumnName} as {xx.ObjectName}", ", ");
                }
                else
                {
                    cats.AddToString(xx.ColumnName, ", ");
                }
            });
            thisStr.Append(cats.GetInfo());
        }
        else if (category == EnumSQLCategory.Normal)
        {
            ColumnModel column = thisList.Where(xx => xx.ObjectName == property).Single();
            if (column.HasMatch == false)
            {
                thisStr.Append($"{column.ColumnName} as {column.ObjectName} ");
            }
            else
            {
                thisStr.Append($"{column.ColumnName} ");
            }
        }
        else if (category == EnumSQLCategory.Count)
        {
            thisStr.Append("count (*)");
        }
        else if (category == EnumSQLCategory.Bool)
        {
            thisStr.Append('1');
        }
        else if (category == EnumSQLCategory.Delete)
        {
            throw new CustomBasicException("Deleting is not supposed to get a select statement.  Try delete statement instead");
        }
        else
        {
            throw new CustomBasicException("Not supported");
        }
        thisStr.Append(" from ");
        thisStr.Append(tableName);
        return thisStr.ToString();
    }
    public static string GetDeleteStatement(string tableName)
    {
        StringBuilder thisStr = new("delete from ");
        thisStr.Append(tableName);
        return thisStr.ToString();
    }
    public static string GetLimitSQLite(EnumDatabaseCategory database, int howMany)
    {
        if (database == EnumDatabaseCategory.SQLServer)
        {
            return "";
        }
        if (howMany <= 0)
        {
            return "";
        }
        return $"Limit {howMany}";
    }
    public static string GetSortStatement<E>(BasicList<ColumnModel> mapList, BasicList<SortInfo>? sortList, bool isJoined)
        where E: class, ISimpleDatabaseEntity
    {
        if (sortList == null || sortList.Count == 0)
        {
            return "";
        }
        //if (sortList.Count == 0)
        //{
        //    throw new CustomBasicException("If you are not sending nothing. you must have at least one condition");
        //}
        StringBuilder thisStr = new();
        thisStr.Append(" order by ");
        string extras;
        StrCat cats = new();
        sortList.ForEach(items =>
        {
            ColumnModel thisMap = TableMapGlobalClass<E>.FindMappingForProperty(items, mapList);
            if (items.OrderBy == SortInfo.EnumOrderBy.Descending)
            {
                extras = " desc";
            }
            else
            {
                extras = "";
            }
            if (isJoined == false)
            {
                cats.AddToString($"{thisMap.ColumnName}{extras}", ", ");
            }
            else
            {
                cats.AddToString($"{thisMap.Prefix}.{thisMap.ColumnName}{extras}", ", ");
            }
        });
        thisStr.Append(cats.GetInfo());
        return thisStr.ToString();
    }
}