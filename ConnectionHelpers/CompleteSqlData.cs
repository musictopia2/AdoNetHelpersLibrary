namespace AdoNetHelpersLibrary.ConnectionHelpers;
//in the old version, was internal.  has to be public so source generators can use this.
public class CompleteSqlData
{
    public string SQLStatement { get; set; } = "";
    public BasicList<DynamicParameter> Parameters { get; set; } = [];
}