namespace AdoNetHelpersLibrary.ConnectionHelpers;
public static class CommandExecuteScalarGlobalClass<E, R>
    where E : class
{
    public static ICommandExecuteScalar<E, R>? MasterContext { get; set; }
}