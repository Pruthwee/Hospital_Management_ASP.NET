using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DBProject.DAL
{
    /// <summary>
    /// Cloud-ready connection manager with connection pooling support
    /// Implements proper connection lifecycle management for Azure SQL
    /// </summary>
    public static class ConnectionManager
    {
        private static readonly string _connectionString;

        static ConnectionManager()
        {
            // Try to get connection string from environment variable first (cloud-native)
            var envConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                _connectionString = envConnectionString;
            }
            else
            {
                // Fallback to configuration file
                _connectionString = ConfigurationManager.ConnectionStrings["sqlCon1"]?.ConnectionString;
                
                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException(
                        "Database connection string not found. Set SQL_CONNECTION_STRING environment variable or configure sqlCon1 in Web.config");
                }
            }
        }

        /// <summary>
        /// Gets a new SQL connection with proper timeout and pooling settings for Azure
        /// </summary>
        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            
            // Set connection timeout for cloud resilience
            if (connection.ConnectionTimeout < 30)
            {
                var builder = new SqlConnectionStringBuilder(_connectionString)
                {
                    ConnectTimeout = 30,
                    // Enable connection pooling (default is true, but explicit for clarity)
                    Pooling = true,
                    MinPoolSize = 0,
                    MaxPoolSize = 100,
                    // Enable retry logic for transient failures in Azure SQL
                    ConnectRetryCount = 3,
                    ConnectRetryInterval = 10
                };
                
                connection = new SqlConnection(builder.ConnectionString);
            }
            
            return connection;
        }

        /// <summary>
        /// Executes an action with a managed connection (automatically opens and closes)
        /// </summary>
        public static T ExecuteWithConnection<T>(Func<SqlConnection, T> action)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return action(connection);
            }
        }

        /// <summary>
        /// Executes an action with a managed connection (automatically opens and closes)
        /// </summary>
        public static void ExecuteWithConnection(Action<SqlConnection> action)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                action(connection);
            }
        }
    }
}
