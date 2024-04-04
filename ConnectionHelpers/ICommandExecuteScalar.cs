namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICommandExecuteScalar<E, R>
    where E : class
{
    abstract static R? ExecuteScalar(IDbCommand command);
}