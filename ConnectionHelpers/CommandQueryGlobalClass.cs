namespace AdoNetHelpersLibrary.ConnectionHelpers;
public static class CommandQueryGlobalClass<T>
{
    public static ICommandQuery<T>? MasterContext { get; set; }
}