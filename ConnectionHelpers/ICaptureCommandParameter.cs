namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICaptureCommandParameter
{
    //needs to be public so i can use extensions that use it.
    DbParameter GetParameter();
    IDbCommand GetCommand();
    EnumDatabaseCategory Category { get; internal set; }
    IDbConnection? CurrentConnection { get; set; }
}