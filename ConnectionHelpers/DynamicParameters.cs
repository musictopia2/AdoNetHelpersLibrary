namespace AdoNetHelpersLibrary.ConnectionHelpers;
public class DynamicParameter
{
    public DbType DbType { get; set; }
    public string ParameterName { get; set; } = "";
    public object? Value { get; set; }
    public bool SourceColumnNullMapping { get; set; }
    //not sure if i need this but might as well put in there just in case.
    public byte Precision { get; set; }
}