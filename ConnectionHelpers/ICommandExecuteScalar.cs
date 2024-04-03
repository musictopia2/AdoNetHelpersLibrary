namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICommandExecuteScalar<E, R>
    where E : class
{
    R? ExecuteScalar(IDbCommand command);
}