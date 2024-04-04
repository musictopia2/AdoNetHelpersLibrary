namespace AdoNetHelpersLibrary.MapHelpers;
//its possible that source generators don't even need this (?)
internal class CompleteSqlData
{
    public string SQLStatement { get; set; } = "";
    public BasicList<DynamicParameter> Parameters { get; set; } = [];
}