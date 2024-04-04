namespace AdoNetHelpersLibrary.MapHelpers;
internal static class MapperExtensions
{
    public static ColumnModel FindMappingForProperty(this IProperty property, BasicList<ColumnModel> originalMappings)
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