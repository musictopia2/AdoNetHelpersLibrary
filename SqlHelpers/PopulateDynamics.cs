namespace AdoNetHelpersLibrary.SqlHelpers;
internal static class PopulateDynamics
{
    public enum EnumCategory
    {
        UseDatabaseMapping,
        Conditional
    }
    public static void PopulateSimple(BasicList<ColumnModel> thisList, CompleteSqlData output, EnumCategory category)
    {
        thisList.ForEach(item =>
        {
            DynamicParameter parameter;
            parameter = new();
            parameter.DbType = item.ColumnType;
            if (category == EnumCategory.UseDatabaseMapping)
            {
                parameter.ParameterName = $"@{item.ColumnName}";
                parameter.Value = item.Value;
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