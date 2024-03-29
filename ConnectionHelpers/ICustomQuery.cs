namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICustomQuery<T>
{
    BasicList<T> Query(IDbCommand command);
    Task<BasicList<T>> QueryAsync(IDbCommand command);
}