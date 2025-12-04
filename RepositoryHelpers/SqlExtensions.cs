using CommonBasicLibraries.CrudRepositoryHelpers;
using Microsoft.Extensions.DependencyInjection;
namespace AdoNetHelpersLibrary.RepositoryHelpers;
public static class SqlExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the database connection and repositories for specified entities using a database connection manager.
        /// This overload is used when you want to provide a custom database connection manager (e.g., for SQL Server, SQLite).
        /// The connection manager is created and used for all repository registrations in this method.
        /// </summary>
        /// <param name="connectionKey">The connection key used to identify the database.</param>
        /// <param name="entityConfigurator">A delegate that configures repositories for specific entities by calling RegisterRepository.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddDatabaseRepositories<T>(
        string connectionKey,
        Action<DatabaseRepositoryRegistrationContext> entityConfigurator)
        where T : class, IDatabaseConnectionManager, new()
        {
            T manager = new();  // Create an instance of the database connection manager (e.g., SQLServerConnectionManager, SQLiteConnectionManager)

            // Create a new DatabaseRepositoryRegistrationContext to hold services and connection key
            DatabaseRepositoryRegistrationContext context = new(services, connectionKey, manager);

            // Configure repositories via the provided delegate
            entityConfigurator?.Invoke(context);
            return services;
        }

        /// <summary>
        /// Registers repositories for a database configuration type that implements ISqlDatabaseConfiguration.
        /// This overload is used when the connection string is derived from a configuration class (e.g., TestDatabaseClass, PersonDatabase).
        /// The database connection manager (e.g., SQLServerConnectionManager, SQLiteConnectionManager) is created and used for all repository registrations in this method.
        /// </summary>
        /// <param name="entityConfigurator">A delegate that configures repositories for specific entities by calling RegisterRepository.</param>
        /// <typeparam name="T">The type implementing ISqlDatabaseConfiguration that holds the connection string.</typeparam>
        /// <typeparam name="U">The type of the database connection manager that will be used to initialize the database and create repositories.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddDatabaseRepositories<T, U>(
            Action<DatabaseRepositoryRegistrationContext> entityConfigurator)
            where T : ISqlDatabaseConfiguration
            where U : class, IDatabaseConnectionManager, new()
        {
            // Get the connection key based on the configuration class T.DatabaseName
            U manager = new();  // Create an instance of the database connection manager (e.g., SQLServerConnectionManager, SQLiteConnectionManager)

            // Create a new DatabaseRepositoryRegistrationContext to hold services and connection key
            DatabaseRepositoryRegistrationContext context = new(services, T.DatabaseName, manager);

            // Configure repositories via the provided delegate
            entityConfigurator?.Invoke(context);
            return services;
        }

        /// <summary>
        /// Registers repositories for a database configuration type that implements ISqlDatabaseConfiguration.
        /// This overload is used when the connection string is derived from the configuration class (e.g., TestDatabaseClass, PersonDatabase),
        /// but no database connection manager is specified. The connection manager must already be registered in the DI container ahead of time.
        /// </summary>
        /// <param name="entityConfigurator">A delegate that configures repositories for specific entities by calling RegisterRepository.</param>
        /// <typeparam name="T">The type implementing ISqlDatabaseConfiguration that holds the connection string.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddDatabaseRepositories<T>(
            Action<DatabaseRepositoryRegistrationContext> entityConfigurator)
            where T : ISqlDatabaseConfiguration
        {
            // Create a new DatabaseRepositoryRegistrationContext with the connection string
            DatabaseRepositoryRegistrationContext context = new(services, T.DatabaseName, null);

            // Configure repositories via the provided delegate
            entityConfigurator.Invoke(context);
            return services;
        }

        /// <summary>
        /// Registers repositories for a specific database connection key.
        /// This overload is used when you have the connection string explicitly provided (e.g., "TestDb", "ProductionDb"),
        /// and no database connection manager is specified. The connection manager must already be registered in the DI container ahead of time.
        /// </summary>
        /// <param name="connectionKey">The connection key used to identify the database (e.g., "TestDb", "ProductionDb").</param>
        /// <param name="entityConfigurator">A delegate that configures repositories for specific entities by calling RegisterRepository.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddDatabaseRepositories(
            string connectionKey,
            Action<DatabaseRepositoryRegistrationContext> entityConfigurator)
        {
            // Create a new DatabaseRepositoryRegistrationContext with the connection string
            DatabaseRepositoryRegistrationContext context = new(services, connectionKey, null);

            // Configure repositories via the provided delegate
            entityConfigurator.Invoke(context);
            return services;
        }
    }       
}