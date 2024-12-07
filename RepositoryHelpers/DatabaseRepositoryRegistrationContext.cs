using CommonBasicLibraries.CrudRepositoryHelpers;
using Microsoft.Extensions.DependencyInjection; //not common enough.
namespace AdoNetHelpersLibrary.RepositoryHelpers;
public class DatabaseRepositoryRegistrationContext(IServiceCollection services, string connectionString, IDatabaseConnectionManager? manager)
{
    public DatabaseRepositoryRegistrationContext Add<TEntity>() where TEntity : class, ISimpleDatabaseEntity, ICommandQuery<TEntity>, ITableMapper<TEntity>
    {
        services.AddScoped(provider =>
        {
            if (Configuration is null)
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                Configuration = configuration;
            }
            manager ??= provider.GetRequiredService<IDatabaseConnectionManager>();
            return new SQLRepository<TEntity>(connectionString, manager);
        }).AddScoped<IRepository<TEntity>>(provider =>
                provider.GetRequiredService<SQLRepository<TEntity>>())
            .AddScoped<IRepositoryQueryAdvanced<TEntity>>(provider =>
            provider.GetRequiredService<SQLRepository<TEntity>>());
        return this;
    }
}