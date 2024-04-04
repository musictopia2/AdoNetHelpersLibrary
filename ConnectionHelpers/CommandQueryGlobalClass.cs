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
public static class CommandQueryGlobalClass<TFirst, TSecond, TReturn>
    where TFirst : class
    where TSecond : class
    where TReturn : class
{
    public static ICommandQuery<TFirst, TSecond, TReturn>? MasterContext { get; set; }
}