namespace AdoNetHelpersLibrary.SqlHelpers;
public static class StatementFactoryUpdates
{
    public enum EnumUpdateCategory
    {
        Auto,
        All,
        Common
    }
    internal static (string sqls, BasicList<ColumnModel> ParameterMappings) GetUpdateStatement<E>(BasicList<UpdateFieldInfo> manuelList, SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        if (manuelList.Count == 0)
        {
            throw new CustomBasicException("If you are manually updating, you have to send at least one field to update");
        }
        BasicList<ColumnModel> updateList = GetParameterMappings<E>(manuelList, map);
        return GetUpdateStatement(updateList);
    }
    internal static (string sqls, BasicList<ColumnModel> ParameterMappings) GetUpdateStatement<E>(BasicList<UpdateEntity> manuelList, SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        if (manuelList.Count == 0)
        {
            throw new CustomBasicException("If you are manually updating, you have to send at least one field to update");
        }
        BasicList<ColumnModel> updateList = GetParameterMappings<E>(manuelList, map);
        return GetUpdateStatement(updateList);
    }
    internal static (string sqls, BasicList<ColumnModel> ParameterMappings) GetUpdateStatement<E>(E thisEntity, SourceGeneratedMap map) where E : class, IUpdatableEntity
    {
        BasicList<ColumnModel> updateList = GetParameterMappings(thisEntity, map);
        return GetUpdateStatement(updateList);
    }
    internal static (string sqls, BasicList<ColumnModel> ParameterMappings) GetUpdateStatement<E>(E thisEntity, EnumUpdateCategory category, SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        BasicList<ColumnModel> updateList = GetParameterMappings(thisEntity, category, map);
        return GetUpdateStatement(updateList);
    }
    private static (string sqls, BasicList<ColumnModel> ParameterMappings) GetUpdateStatement(BasicList<ColumnModel> updateList)
    {
        if (updateList.Count == 0)
        {
            return ("", []);
        }
        StringBuilder thisStr = new();
        string tableName = updateList.First().TableName; //found a use for this.
        thisStr.Append("update ");
        thisStr.Append(tableName);
        thisStr.Append(" set ");
        StrCat cats = new();
        updateList.ForEach(xx =>
        {
            cats.AddToString($"{xx.ColumnName} = @{xx.ColumnName}", ", ");
        });
        thisStr.Append(cats.GetInfo());
        //for now, will do this way.  could change in future.
        thisStr.Append(" where ID = @ID");
        return (thisStr.ToString(), updateList);
    }
    private static BasicList<ColumnModel> GetParameterMappings<E>(BasicList<UpdateEntity> updateList ,SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        if (updateList.Count == 0)
        {
            return [];
        }
        BasicList<ColumnModel> mapList = map.Columns;
        BasicList<ColumnModel> newList = [];
        updateList.ForEach(items =>
        {
            ColumnModel thisMap = TableMapGlobalClass<E>.FindMappingForProperty(items, mapList);
            thisMap.Value = items.Value;
            if (items.Property == "ID")
            {
                throw new CustomBasicException("You are not allowed to update the ID");
            }
            newList.Add(thisMap);
        });
        return newList;
    }
    private static BasicList<ColumnModel> GetParameterMappings<E>(BasicList<UpdateFieldInfo> updateList, SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        if (updateList.Count == 0)
        {
            return [];
        }
        BasicList<ColumnModel> mapList = map.Columns;
        BasicList<ColumnModel> newList = [];
        updateList.ForEach(items =>
        {
            ColumnModel thisMap = TableMapGlobalClass<E>.FindMappingForProperty(items, mapList);
            //not sure if i need this (hopefully the value is done automatically) not sure though (?)
            //thisMap.Value = thisMap.PropertyDetails.GetValue(thisEntity, null);
            if (items.Property == "ID")
            {
                throw new CustomBasicException("You are not allowed to update the ID");
            }
            newList.Add(thisMap);
        });
        return newList;
    }
    private static BasicList<ColumnModel> GetParameterMappings<E>(E thisEntity, SourceGeneratedMap map) where E : class, IUpdatableEntity
    {
        BasicList<UpdateFieldInfo> updateList = [];
        BasicList<string> firstList = thisEntity.GetChanges();
        updateList.Append(firstList);
        return GetParameterMappings<E>(updateList, map);
    }
    private static BasicList<ColumnModel> GetParameterMappings<E>(E thisEntity, EnumUpdateCategory category, SourceGeneratedMap map) where E : class, ISimpleDatabaseEntity
    {
        BasicList<UpdateFieldInfo> updateList = [];
        if (category == EnumUpdateCategory.Auto)
        {
            return GetParameterMappings((IUpdatableEntity)thisEntity, map);
        }
        //if i cannot send the parameter for the entity here, then will need another method for something else.
        BasicList<ColumnModel> mapList = map.Columns;
        if (category == EnumUpdateCategory.Common)
        {
            mapList.RemoveAllOnly(xx => xx.CommonForUpdating == false);
        }
        else
        {
            mapList.RemoveAllOnly(xx => xx.ColumnName == "ID");
        }
        //not sure if i need this or not (?)

        //mapList.ForEach(items =>
        //{
        //    items.Value = items.PropertyDetails.GetValue(thisEntity, null);
        //});
        return mapList;
    }
}