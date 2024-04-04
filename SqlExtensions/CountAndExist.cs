namespace AdoNetHelpersLibrary.SqlExtensions;
public static class CountAndExist
{
    public static int Count<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, null, database, EnumSQLCategory.Count);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return capture.ExecuteScalar<int>(thisData, thisTran, connectionTimeOut);
    }
    public static int Count<E>(this ICaptureCommandParameter capture, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        string sqls = GetSimpleSelectStatement(map.Columns, map.TableName, database, EnumSQLCategory.Count);
        return capture.ExecuteScalar<int>(sqls, null, thisTran, connectionTimeOut, CommandType.Text);
    }
    public static bool Exists<E>(this ICaptureCommandParameter capture, BasicList<ICondition> conditions, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = E.GetTableMap();
        var (sqls, ParameterMappings) = GetConditionalStatement(map.Columns, map.TableName, conditions, null, database, EnumSQLCategory.Bool);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        return capture.Exists(thisData, thisTran, connectionTimeOut);
    }
    public static bool Exists<E>(this ICaptureCommandParameter capture, int id, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ITableMapper<E>, ISimpleDatabaseEntity
    {
        BasicList<ICondition> list = StartConditionWithID(id);
        return capture.Exists<E>(list, thisTran, connectionTimeOut);
    }
}