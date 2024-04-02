namespace AdoNetHelpersLibrary.SqlExtensions;
public static class UpdateDatabase
{
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, EnumUpdateCategory category, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, category);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, IUpdatableEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void UpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, BasicList<UpdateFieldInfo> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, updateList);
        capture.PrivateUpdateEntity(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, EnumUpdateCategory category, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, category);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, IUpdatableEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static async Task UpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, BasicList<UpdateFieldInfo> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement(thisEntity, updateList);
        await capture.PrivateUpdateEntityAsync(thisEntity, sqls, ParameterMappings, thisTran, connectionTimeOut);
    }
    public static void Update<E>(this ICaptureCommandParameter capture, int id, BasicList<UpdateEntity> updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        var (sqls, ParameterMappings) = GetUpdateStatement<E>(updateList);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(ParameterMappings, thisData, EnumCategory.UseDatabaseMapping);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = id;
        thisData.Parameters.Add(parameter);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
    private static void PrivateUpdateEntity<E>(this ICaptureCommandParameter capture, E thisEntity, string sqls, BasicList<ColumnModel>? updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        if (sqls == "")
        {
            return;
        }
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(updateList!, thisData, EnumCategory.UseDatabaseMapping);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = thisEntity.ID;
        thisData.Parameters.Add(parameter);
        capture.Execute(thisData, thisTran, connectionTimeOut);
    }
    private static async Task PrivateUpdateEntityAsync<E>(this ICaptureCommandParameter capture, E thisEntity, string sqls, BasicList<ColumnModel>? updateList, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        if (sqls == "")
        {
            return;
        }
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        PopulateSimple(updateList!, thisData, EnumCategory.UseDatabaseMapping);
        DynamicParameter parameter = new();
        parameter.ParameterName = "@ID";
        parameter.DbType = DbType.Int32; //hopefully good enough (?)
        parameter.Value = thisEntity.ID;
        thisData.Parameters.Add(parameter);
        await capture.ExecuteAsync(thisData, thisTran, connectionTimeOut);
    }
}