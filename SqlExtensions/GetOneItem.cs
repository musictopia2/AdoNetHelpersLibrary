﻿namespace DapperHelpersLibrary.Extensions;
public static class GetOneItem
{
    private static SourceGeneratedMap GetMap<E>()
        where E : class, ISimpleDapperEntity
    {
        return TableMapGlobalClass<E>.GetMap();
    }
    public static R GetSingleObject<E, R>(this ICaptureCommandParameter capture, string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDapperEntity
        where R: IParsable<R>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, sortList, database, howMany: 1, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        }
        return capture.ExecuteScalar<R>(thisData, thisTran, connectionTimeOut)!;
    }
    public static async Task<R?> GetSingleObjectAsync<E, R>(this ICaptureCommandParameter capture, string property, BasicList<SortInfo> sortList, BasicList<ICondition>? conditions = null, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDapperEntity
        where R : IParsable<R>
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, sortList, database, howMany: 1, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        }
        return await capture.ExecuteScalarAsync<R>(thisData, thisTran!, connectionTimeOut);
    }
    public static BasicList<R> GetObjectList<E, R>(this ICaptureCommandParameter capture, string property, BasicList<ICondition>? conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null)
        where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, conditions, sortList, database, howMany: howMany, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        }

        //this may be iffy.
        return capture.Query<R>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
    public static async Task<BasicList<R>> GetObjectListAsync<E, R>(this ICaptureCommandParameter capture, string property, BasicList<ICondition>? Conditions = null, BasicList<SortInfo>? sortList = null, int howMany = 0, IDbTransaction? thisTran = null, int? connectionTimeOut = null) where E : class, ISimpleDapperEntity
    {
        EnumDatabaseCategory database = capture.Category;
        SourceGeneratedMap map = GetMap<E>();
        var (sqls, ParameterMappings) = GetConditionalStatement<E>(map.Columns, map.TableName, Conditions, sortList, database, howMany: howMany, property: property);
        CompleteSqlData thisData = new();
        thisData.SQLStatement = sqls;
        if (Conditions != null)
        {
            PopulateSimple(ParameterMappings, thisData, EnumCategory.Conditional);
        }
        return await capture.QueryAsync<R>(thisData.SQLStatement, thisData.Parameters, thisTran, commandTimeout: connectionTimeOut, CommandType.Text);
    }
}