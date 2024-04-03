namespace AdoNetHelpersLibrary.ConnectionHelpers;
public static class CommandQueryGlobalClass<E>
    where E: class
{
    public static ICommandQuery<E>? MasterContext { get; set; }
}
public static class CommandQueryGlobalClass<E, R>
    where E: class
{
    public static ICommandQuery<E, R>? MasterContext { get; set; }
}