namespace AdoNetHelpersLibrary.MapHelpers;
public static class TableMapGlobalClass<E>
    where E : class, ISimpleDapperEntity
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