namespace AdoNetHelpersLibrary.ConnectionHelpers;
public interface ICommandQuery<E>
    where E : class
{
    abstract static BasicList<E> Query(IDbCommand command, EnumDatabaseCategory category);
    abstract static Task<BasicList<E>> QueryAsync(IDbCommand command, EnumDatabaseCategory category);
}
//this means will expand.
//this means the new version of the source generator has to create classes for each of the individual types.
//would prefer to have the same source generator do both.
//otherwise, requires complex names.

public interface ICommandQuery<E, R>
    where E : class
{
    abstract static BasicList<R> Query(IDbCommand command, EnumDatabaseCategory category);
    abstract static Task<BasicList<R>> QueryAsync(IDbCommand command, EnumDatabaseCategory category);
}
public interface ICommandQuery<TFirst, TSecond, TReturn>
    where TFirst: class
    where TSecond: class
    where TReturn : class
{
    abstract static BasicList<TReturn> Query(IDbCommand command, Func<TFirst, TSecond?, TFirst> action, EnumDatabaseCategory category);
    abstract static Task<BasicList<TReturn>> QueryAsync(IDbCommand command, Func<TFirst, TSecond?, TFirst> action, EnumDatabaseCategory category);
}
public interface ICommandQuery<TFirst, TSecond, TThird, TReturn>
    where TFirst : class
    where TSecond : class
    where TThird : class
    where TReturn : class
{
    abstract static BasicList<TReturn> Query(IDbCommand command, Func<TFirst, TSecond?, TThird?, TFirst> action, EnumDatabaseCategory category);
    abstract static Task<BasicList<TReturn>> QueryAsync(IDbCommand command, Func<TFirst, TSecond?, TThird?, TFirst> action, EnumDatabaseCategory category);
}