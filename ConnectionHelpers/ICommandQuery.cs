namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICommandQuery<T>
{
    BasicList<T> Query(IDbCommand command);
    Task<BasicList<T>> QueryAsync(IDbCommand command);
}