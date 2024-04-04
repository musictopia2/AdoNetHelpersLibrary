namespace AdoNetHelpersLibrary.MapHelpers;
public interface ITableMapper<E>
    where E : class, ISimpleDatabaseEntity
{
    //if i need extra information, then provide it.
    string TableName { get; }
    string GetForeignKey(string name); //if it returns blank, then means no foreign key.
    SourceGeneratedMap GetTableMap(E payLoad, bool isAutoIncremented = true, bool beingJoined = false);
    SourceGeneratedMap GetTableMap(bool beingJoined = false);
    bool IsAutoIncremented { get; } //the source generator will figure this out.
}
//public interface ITableMapper<E, D>
//    where E: class, ISimpleDatabaseEntity
//    where D: class, ISimpleDatabaseEntity
//{
//    string GetJoiner { get; }
//}