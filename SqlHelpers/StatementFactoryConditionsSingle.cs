namespace AdoNetHelpersLibrary.SqlHelpers;
internal static class StatementFactoryConditionsSingle
{
    public static (string sqls, BasicList<ColumnModel> ParameterMappings) GetConditionalStatement<E>(BasicList<ColumnModel> mapList, string tableName,
        BasicList<ICondition>? conditionList, BasicList<SortInfo>? sortList, EnumDatabaseCategory database, EnumSQLCategory category = EnumSQLCategory.Normal,
        int howMany = 0, string property = "")
        where E : class, ISimpleDapperEntity
    {
        StringBuilder thisStr = new();
        if (category != EnumSQLCategory.Delete)
        {
            thisStr.Append(GetSimpleSelectStatement(mapList, tableName, database, category, howMany, property));
        }
        else
        {
            thisStr.Append(GetDeleteStatement(tableName));
        }
        var paramList = new BasicList<ColumnModel>();
        if (conditionList == null)
        {
            thisStr.Append(GetSortStatement<E>(mapList, sortList, false));
            return (thisStr.ToString()!, null!);
        }
        thisStr.Append(" Where ");
        Dictionary<string, int> thisDict = [];
        BasicList<AndCondition> andList = conditionList.Where(xx => xx.ConditionCategory == EnumConditionCategory.And).ToCastedList<AndCondition>();
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
        BasicList<OrCondition> orList = conditionList.Where(xx => xx.ConditionCategory == EnumConditionCategory.Or).ToCastedList<OrCondition>();
        StrCat cats = new();
        if (orList.Count > 0)
        {
            if (needsAppend == true)
            {
                thisStr.Append(" and ");
            }
            needsAppend = true;
            thisStr.Append('(');
            orList.ForEach(xx => cats.AddToString(PopulatAnds<E>(xx.ConditionList, mapList, " or ", paramList, thisDict), ") and ("));
            thisStr.Append(cats.GetInfo());
            thisStr.Append(')');
        }
        BasicList<SpecificListCondition> includeList = conditionList.Where(xx => xx.ConditionCategory == EnumConditionCategory.ListInclude).ToCastedList<SpecificListCondition>();
        if (includeList.Count > 1)
        {
            BasicList<int> newList = [];
            includeList.ForEach(xx =>
            {
                newList.AddRange(xx.ItemList);
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
            thisStr.Append("ID in (");
            thisStr.Append(PopulateListInfo(includeList.Single().ItemList));
        }
        BasicList<NotListCondition> notList = conditionList.Where(xx => xx.ConditionCategory == EnumConditionCategory.ListNot).ToCastedList<NotListCondition>();
        if (notList.Count > 1)
        {
            throw new CustomBasicException("You can only have one not list");
        }
        if (notList.Count == 1)
        {
            if (needsAppend == true)
            {
                thisStr.Append(" and ");
            }
            thisStr.Append("ID not in (");
            thisStr.Append(PopulateListInfo(notList.Single().ItemList));
        }
        if (sortList != null)
        {
            thisStr.Append(GetSortStatement<E>(mapList, sortList, false));
        }
        if (database == EnumDatabaseCategory.SQLite && howMany != 0)
        {
            thisStr.Append($" limit {howMany}");
        }
        return (thisStr.ToString(), paramList);
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
    private static string PopulatAnds<E>(BasicList<AndCondition> andList, BasicList<ColumnModel> mapList, string seperator, BasicList<ColumnModel> paramList, Dictionary<string, int> thisDict)
        where E : class, ISimpleDapperEntity
    {
        StrCat cats = new();
        andList.ForEach(items =>
        {
            ColumnModel thisMap = TableMapGlobalClass<E>.FindMappingForProperty(items, mapList);
            if (items.Operator == cs1.IsNotNull || items.Operator == cs1.IsNull)
            {
                cats.AddToString($"{thisMap.ColumnName} {items.Operator}", seperator);
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
                cats.AddToString($"{thisMap.ColumnName} {items.Operator} @{thisMap.ObjectName}", seperator);
            }
        });
        return cats.GetInfo();
    }
}