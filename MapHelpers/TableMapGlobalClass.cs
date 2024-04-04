namespace AdoNetHelpersLibrary.MapHelpers;
public static class TableMapGlobalClass<E>
    where E : class, ISimpleDatabaseEntity
{
    public static ITableMapper<E>? MasterContext { get; set; }
    internal static SourceGeneratedMap GetMap(E payLoad, bool isAutoIncremented = true, bool beingJoined = false)
    {
        if (MasterContext is null)
        {
            throw new CustomBasicException($"No map was created for table {typeof(E)}.  Try creating a source generator");
        }
        return MasterContext.GetTableMap(payLoad, isAutoIncremented, beingJoined);
    }
    internal static bool IsAutoIncrementing()
    {
        if (MasterContext is null)
        {
            throw new CustomBasicException($"No map was created for table {typeof(E)}.  Try creating a source generator");
        }
        return MasterContext.IsAutoIncremented;
    }
    internal static string TableName()
    {
        if (MasterContext is null)
        {
            throw new CustomBasicException($"No map was created for table {typeof(E)}.  Try creating a source generator");
        }
        return MasterContext.TableName;
    }
    internal static string GetForeignKey(string name)
    {
        if (MasterContext is null)
        {
            throw new CustomBasicException($"No map was created for table {typeof(E)}.  Try creating a source generator");
        }
        return MasterContext.GetForeignKey(name);
    }
    internal static SourceGeneratedMap GetMap(bool beingJoined = false)
    {
        if (MasterContext is null)
        {
            throw new CustomBasicException($"No map was created for table {typeof(E)}.  Try creating a source generator");
        }
        return MasterContext.GetTableMap(beingJoined);
    }
    internal static ColumnModel FindMappingForProperty(IProperty property, BasicList<ColumnModel> originalMappings)
    {
        try
        {
            var item = originalMappings.Where(x => x.ColumnName == property.Property || x.ObjectName == property.Property).First();
            //not sure about interface name.
            //once i find out more about that situation, i can add it.
            var output = item.Clone();
            return output;
        }
        catch (Exception ex)
        {
            throw new CustomBasicException($"Had problems getting mappings for conditions.  Condition Property Name Was {property.Property}.  Message Was {ex.Message}");
        }
    }
}
//public static class TableMapGlobalClass<E, D1>
//    where E: class, ISimpleDatabaseEntity
//    where D1: class, ISimpleDatabaseEntity
//{
//    public static ITableMapper<E, D1>? MasterContext { get; set; }
//    public static string GetJoiner()
//    {
//        if (MasterContext is null)
//        {
//            throw new CustomBasicException($"No join mapper was created for main type of {typeof(E)} and other type of {typeof(D1)}");
//        }
//        return MasterContext.GetJoiner;
//    }
//}