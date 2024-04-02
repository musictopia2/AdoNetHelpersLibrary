namespace AdoNetHelpersLibrary.MapHelpers;
public class ColumnModel
{
    public string ColumnName { get; set; } = "";
    public bool IsIdentity { get; set; }
    public string ObjectName { get; set; } = "";
    public object? Value { get; set; }
    //public string ParameterName { get; set; } = "";
    public DbType ColumnType { get; set; } //this is needed.

    //most likely the source generator has to figure out if there is a match or not.
    public bool HasMatch { get; set; }
    public bool Like { get; set; } //this can be set later (not by the source generator)
}