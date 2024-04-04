namespace AdoNetHelpersLibrary.SqlHelpers;
internal static class StatementSelectFactoryJoin
{
    #region No Conditions
    private static SourceGeneratedMap GetMap<E>()
        where E : class, ISimpleDatabaseEntity
    {
        return TableMapGlobalClass<E>.GetMap();
    }
    private static SourceGeneratedMap GetJoinedMap<D>(bool joined)
        where D: class, ISimpleDatabaseEntity
    {
        return TableMapGlobalClass<D>.GetMap(joined);
    }
    private static string GetForeignKey<E>(string tableName)
        where E: class, ISimpleDatabaseEntity
    {
        return TableMapGlobalClass<E>.GetForeignKey(tableName);
    }
    private static void StartList<E>(out BasicList<ColumnModel> thisList, out BasicList<string> joinList, out string tableName) where E : class, ISimpleDatabaseEntity
    {
        SourceGeneratedMap map = GetMap<E>();
        thisList = map.Columns;
        tableName = map.TableName; //i think
        joinList = [];
        thisList.ForEach(x => x.Prefix = "a");
    }
    private static void AppendList<E, D>(BasicList<ColumnModel> thisList, BasicList<string> joinList, string oldTableName, string prefix, bool isOneToOne = true, string firsts = "a", string newTable = "") where D : class, ISimpleDatabaseEntity where E : class, ISimpleDatabaseEntity
    {
        SourceGeneratedMap other = GetJoinedMap<D>(isOneToOne);
        BasicList<ColumnModel> newList = other.Columns;
        newList.ForEach(items => items.Prefix = prefix);
        thisList.AddRange(newList);
        string foreign;
        string otherTable;
        string thisStr;
        if (newTable != "")
        {
            otherTable = newTable;
        }
        else
        {
            otherTable = other.TableName;
        }
        bool hiddenOneToOne = isOneToOne;
        string possibleKey;
        if (isOneToOne)
        {
            possibleKey = GetForeignKey<E>(otherTable);
        }
        else
        {
            possibleKey = GetForeignKey<D>(oldTableName);
        }
        if (possibleKey == "")
        {
            hiddenOneToOne = false; //try this way.
            foreign = GetForeignKey<D>(oldTableName); //always the old table name period.
        }
        else
        {
            foreign = possibleKey;
        }
        if (foreign == "")
        {
            throw new CustomBasicException("No foreign key found for joining.  Rethink");
        }
        if (hiddenOneToOne == true)
        {
            //foreign = TableMapGlobalClass<E, D>.GetJoiner();
            thisStr = $"{otherTable} {prefix} on {firsts}.{foreign}={prefix}.ID";
        }
        else
        {
            //foreign = TableMapGlobalClass<D, E>.GetJoiner();
            thisStr = $"{otherTable} {prefix} on {firsts}.ID={prefix}.{foreign}";
        }
        joinList.Add(thisStr);
    }
    public static string GetSimpleSelectStatement<E, D1>(bool isOneToOne, BasicList<SortInfo>? sortList, EnumDatabaseCategory category, int howMany = 0) where E : class, ISimpleDatabaseEntity, IJoinedEntity<D1> where D1 : class, ISimpleDatabaseEntity
    {
        StartList<E>(out BasicList<ColumnModel> thisList, out BasicList<string> joinList, out string tableName);
        AppendList<E, D1>(thisList, joinList, tableName, "b", isOneToOne);
        string sqls = GetSimpleSelectStatement(thisList, joinList, tableName, category, howMany);
        if (sortList == null)
        {
            return sqls;
        }
        StringBuilder thisStr = new(sqls);
        thisStr.Append(GetSortStatement<E>(thisList, sortList, true)); //i think
        thisStr.Append(GetLimitSQLite(category, howMany));
        return thisStr.ToString();
    }
    private static string GetSimpleSelectStatement(BasicList<ColumnModel> thisList, BasicList<string> joinList, string tableName, EnumDatabaseCategory database, int howMany = 0) //sqlite requires limit at the end
    {
        if (joinList.Count == 0)
        {
            throw new CustomBasicException("Needs at least one other table.  Otherwise, no join");
        }
        StringBuilder thisStr = new("select ");
        StrCat cats = new();
        if (howMany > 0 && database == EnumDatabaseCategory.SQLServer) //sqlite requires it at the end.
        {
            thisStr.Append($"top {howMany} ");
        }
        thisList.ForEach(items =>
        {
            if (items.HasMatch == false)
            {
                cats.AddToString($"{items.Prefix}.{items.ColumnName} as {items.ObjectName}", ", ");
            }
            else
            {
                cats.AddToString($"{items.Prefix}.{items.ColumnName}", ", ");
            }
        });
        thisStr.Append(cats.GetInfo());
        thisStr.Append(" from ");
        thisStr.Append(tableName);
        thisStr.Append(" a left join ");
        cats = new StrCat();
        joinList.ForEach(Items => cats.AddToString(Items, " left join "));
        thisStr.Append(cats.GetInfo());
        return thisStr.ToString();
    }
    #endregion
    #region #region With Conditions
    private static (string sqls, BasicList<ColumnModel> ParameterMappings) FinishConditionStatement<E>(BasicList<ColumnModel> mapList, BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList, StringBuilder thisStr, EnumDatabaseCategory category, int howMany = 0)
        where E: class, ISimpleDatabaseEntity
    {
        var paramList = new BasicList<ColumnModel>();
        thisStr.Append(" Where ");
        BasicList<AndCondition> andList = conditionList.Where(items => items.ConditionCategory == EnumConditionCategory.And).ToCastedList<AndCondition>();
        Dictionary<string, int> thisDict = [];
        bool needsAppend;
        if (andList.Count > 0)
        {
            needsAppend = true;
        }
        else
        {
            needsAppend = false;
        }
        thisStr.Append(PopulatAnds<E>(andList, mapList, " and ", paramList, thisDict));
        BasicList<OrCondition> orList = conditionList.Where(items => items.ConditionCategory == EnumConditionCategory.Or).ToCastedList<OrCondition>();
        StrCat cats = new();
        if (orList.Count > 0)
        {
            if (needsAppend == true)
            {
                thisStr.Append(" and ");
            }
            needsAppend = true;
            thisStr.Append('(');
            orList.ForEach(items => cats.AddToString(PopulatAnds<E>(items.ConditionList, mapList, " or ", paramList, thisDict), ") and ("));
            thisStr.Append(cats.GetInfo());
            thisStr.Append(')');
        }
        BasicList<SpecificListCondition> includeList = conditionList.Where(items => items.ConditionCategory == EnumConditionCategory.ListInclude).ToCastedList<SpecificListCondition>();
        if (includeList.Count > 1)
        {
            BasicList<int> newList = [];
            includeList.ForEach(items =>
            {
                newList.AddRange(items.ItemList);
            });
            includeList = [];
            SpecificListCondition thisI = new();
            thisI.ItemList = newList;
            includeList.Add(thisI);
        }
        if (includeList.Count == 1)
        {
            if (needsAppend == true)
            {
                thisStr.Append(" and ");
            }
            needsAppend = true;
            thisStr.Append("a.ID in (");
            thisStr.Append(PopulateListInfo(includeList.Single().ItemList));
        }
        BasicList<NotListCondition> notList = conditionList.Where(items => items.ConditionCategory == EnumConditionCategory.ListNot).ToCastedList<NotListCondition>();
        if (notList.Count == 1)
        {
            if (needsAppend == true)
            {
                thisStr.Append(" and ");
            }
            thisStr.Append("a.ID not in (");
            thisStr.Append(PopulateListInfo(notList.Single().ItemList));
        }
        if (sortList != null)
        {
            thisStr.Append(GetSortStatement<E>(mapList, sortList, true));
        }
        thisStr.Append(GetLimitSQLite(category, howMany));
        return (thisStr.ToString(), paramList);
    }
    public static (string sqls, BasicList<ColumnModel> ParameterMappings) GetConditionalStatement<E, D1>(BasicList<ICondition> conditionList, BasicList<SortInfo>? sortList, bool isOneToOne, EnumDatabaseCategory category, int howMany = 0) where E : class, IJoinedEntity<D1> where D1 : class, ISimpleDatabaseEntity
    {
        StringBuilder thisStr = new();
        StartList<E>(out BasicList<ColumnModel> mapList, out BasicList<string> joinList, out string tableName);
        AppendList<E, D1>(mapList, joinList, tableName, "b", isOneToOne);
        thisStr.Append(GetSimpleSelectStatement(mapList, joinList, tableName, category, howMany));
        return FinishConditionStatement<E>(mapList, conditionList, sortList, thisStr, category, howMany);
    }
    private static string PopulateListInfo(BasicList<int> thisList)
    {
        StrCat cats = new();
        thisList.ForEach(items => cats.AddToString(items.ToString(), ", "));
        StringBuilder thisStr = new();
        thisStr.Append(cats.GetInfo());
        thisStr.Append(')');
        return thisStr.ToString();
    }
    private static string PopulatAnds<E>(BasicList<AndCondition> andList, BasicList<ColumnModel> mapList, string seperator, BasicList<ColumnModel> paramList, Dictionary<string, int> thisDict) where E: class, ISimpleDatabaseEntity
    {
        StrCat cats = new();
        andList.ForEach(items =>
        {
            ColumnModel thisMap = TableMapGlobalClass<E>.FindMappingForProperty(items, mapList);
            if (items.Operator == cs1.IsNotNull || items.Operator == cs1.IsNull)
            {
                cats.AddToString($"{thisMap.Prefix}.{thisMap.ColumnName} {items.Operator}", seperator); //i am guessing that with parameters no need to worry about the null parts
            }
            else
            {
                if (items.Operator == cs1.Like)
                {
                    thisMap.Like = true;
                }
                paramList.Add(thisMap);
                object realValue;
                if (bool.TryParse(items.Value!.ToString(), out bool NewBool) == false)
                {
                    realValue = items.Value;
                }
                else if (NewBool == true)
                {
                    realValue = 1;
                }
                else
                {
                    realValue = 0;
                }
                thisMap.Value = realValue;
                thisMap.ObjectName = thisDict.GetNewValue(thisMap.ColumnName);
                if (items.Property != "ID")
                {
                    cats.AddToString($"{items.Code}{thisMap.ColumnName} {items.Operator} @{thisMap.ObjectName}", seperator);
                }
                else
                {
                    cats.AddToString($"{thisMap.Prefix}.{thisMap.ColumnName} {items.Operator} @{thisMap.ObjectName}", seperator);
                }
            }
        });
        return cats.GetInfo();
    }
    #endregion
}