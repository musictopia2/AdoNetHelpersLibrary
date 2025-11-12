namespace AdoNetHelpersLibrary.SqlHelpers;
internal static class PopulateDynamics
{
    public enum EnumCategory
    {
        UseDatabaseMapping,
        Conditional
    }
    public static void PopulateSimple(BasicList<ColumnModel> thisList, CompleteSqlData output, EnumCategory category, EnumDatabaseCategory database)
    {
        thisList.ForEach(item =>
        {
            DynamicParameter parameter;
            parameter = new();
            if (database == EnumDatabaseCategory.SQLite && item.Value is DateOnly)
            {
                parameter.DbType = DbType.String; //hopefully does not break something else.
            }
            else
            {
                parameter.DbType = item.ColumnType;
            }
            if (category == EnumCategory.UseDatabaseMapping)
            {
                parameter.ParameterName = $"@{item.ColumnName}";
                parameter.Value = NormalizeParameterValue(item.Value, database);
            }
            else
            {
                parameter.ParameterName = $"@{item.ObjectName}";
                if (item.Like == false)
                {
                    parameter.Value = item.Value;
                }
                else
                {
                    parameter.Value = $"%{item.Value}%";
                }
            }
            output.Parameters.Add(parameter);
        });
    }

    private static object? NormalizeParameterValue(object? value, EnumDatabaseCategory category)
    {
        if (category == EnumDatabaseCategory.SQLServer)
        {
            return value;
        }
        if (value == null)
        {
            return DBNull.Value;
        }

        return value switch
        {
            DateOnly d => d.ToString(),
            TimeOnly t => t.ToString(),
            _ => value
        };
    }


    public static BasicList<DynamicParameter> GetDynamicIDData(ref StringBuilder builder, int ID, bool isJoined = false)
    {
        BasicList<DynamicParameter> output = [];
        DynamicParameter parameter = new();
        parameter.Value = ID;
        parameter.ParameterName = "@ID"; //hopefully this simple.
        parameter.DbType = DbType.Int32;
        output.Add(parameter);
        if (isJoined == false)
        {
            builder.Append(" where ID = @ID");
        }
        else
        {
            builder.Append(" where a.ID = @ID");
        }
        return output;
    }
}