namespace AdoNetHelpersLibrary.SqlExtensions;
public static class CountAndExist
{
    private static SourceGeneratedMap GetMap<E>()
        where E : class, ISimpleDatabaseEntity
    {
        return TableMapGlobalClass<E>.GetMap();
    }
    public static int Count<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, null, database, EnumSQLCategory.Count);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return capture.ExecuteScalar<int>(thisData, thisTran, connectionTimeOut);
    }
    public static int Count<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        string sqls = GetSimpleSelectStatement(map.Columns, map.TableName, database, EnumSQLCategory.Count);
        return capture.ExecuteScalar<int>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    public static bool Exists<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, null, database, EnumSQLCategory.Bool);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return capture.Exists(thisData, thisTran, connectionTimeOut);
    }
    public static bool Exists<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDatabaseEntity
    {
        BasicList<ICondition> list = StartConditionWithID(id);
        return capture.Exists<E>(list, thisTran, connectionTimeOut);
    }
}