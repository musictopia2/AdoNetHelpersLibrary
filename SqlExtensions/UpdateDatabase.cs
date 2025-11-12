namespace AdoNetHelpersLibrary.SqlExtensions;
public static class UpdateDatabase
{
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, EnumUpdateCategory category, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, category, map);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, IUpdatableEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, map);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, BasicList<UpdateFieldInfo> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(updateList, map);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, EnumUpdateCategory category, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, category, map);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, IUpdatableEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, map);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, BasicList<UpdateFieldInfo> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(thisEntity);
        var (sqls, ParameterMappings) = GetUpdateStatement(updateList, map);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void Update<E>(this ICaptureCommandParameter capture, int id, BasicList<UpdateEntity> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(); //try this way.
        var (sqls, ParameterMappings) = GetUpdateStatement(updateList, map);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        var category = capture.Category;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.UseDatabaseMapping, category);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = id;
        thisData.Parameters.Add(parameter);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
    public static async Task UpdateAsync<E>(this ICaptureCommandParameter capture, int id, BasicList<UpdateEntity> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDatabaseEntity, ITableMapper<E>
    {
        var map = E.GetTableMap(); //try this way.
        var (sqls, ParameterMappings) = GetUpdateStatement(updateList, map);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        var category = capture.Category;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.UseDatabaseMapping, category);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = id;
        thisData.Parameters.Add(parameter);
        await capture.ExecuteAsync(thisData, thisTran, connectionTimeOut);
    }
    private static void PrivateUpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, string sqls, BasicList<ColumnModel>? updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity
    {
        if (sqls == "")
        {
            return;
        }
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        var category = capture.Category;
        PopulateSimple(updateList!, thisData, EnumCategory.UseDatabaseMapping, category);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = thisEntity.ID;
        thisData.Parameters.Add(parameter);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
    private static async Task PrivateUpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, string sqls, BasicList<ColumnModel>? updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) 
        where E : class, ISimpleDatabaseEntity
    {
        if (sqls == "")
        {
            return;
        }
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        var category = capture.Category;
        PopulateSimple(updateList!, thisData, EnumCategory.UseDatabaseMapping, category);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = thisEntity.ID;
        thisData.Parameters.Add(parameter);
        await capture.ExecuteAsync(thisData, thisTran, connectionTimeOut);
    }
}