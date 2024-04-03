namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICommandQuery<E>
    where E : class
{
    BasicList<E> Query(IDbCommand command);
    Task<BasicList<E>> QueryAsync(IDbCommand command);
}
//this means will expand.
//this means the new version of the source generator has to create classes for each of the individual types.
//would prefer to have the same source generator do both.
//otherwise, requires complex names.

public interface ICommandQuery<E, R>
    where E : class
{
    BasicList<R> Query(IDbCommand command);
    Task<BasicList<R>> QueryAsync(IDbCommand command);
}