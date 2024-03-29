namespace AdoNetHelpersLibrary.ConnectionHelpers;
public class DynamicParameters
{
    public DbType DbType { get; set; }
    public string ParameterName { get; set; } = "";
    public object? Value { get; set; }
}