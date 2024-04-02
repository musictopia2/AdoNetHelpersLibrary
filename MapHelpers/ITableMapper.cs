namespace AdoNetHelpersLibrary.MapHelpers;
public interface ITableMapper<E>
    where E : class, ISimpleDapperEntity
{
    //if i need extra information, then provide it.
    SourceGeneratedMap GetTableMap(E payLoad, bool isAutoIncremented = true, bool beingJoined = false);
    SourceGeneratedMap GetTableMap(bool beingJoined = false);
    bool IsAutoIncremented { get; } //the source generator will figure this out.
}