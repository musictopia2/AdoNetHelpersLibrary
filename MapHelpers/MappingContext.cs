namespace AdoNetHelpersLibrary.MapHelpers;
internal class MappingContext : MappingCloningContext
{
    protected override void Configure(ICustomConfig config)
    {
        config.Make<ColumnModel>(c =>
        {
            c.Cloneable(false);
        });
    }
}