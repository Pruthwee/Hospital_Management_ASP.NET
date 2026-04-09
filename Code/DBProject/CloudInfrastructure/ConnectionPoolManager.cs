using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DBProject.CloudInfrastructure
{
    /// <summary>
    /// Cloud-ready connection pool manager for Azure SQL Database
    /// Implements connection pooling, retry logic, and transient fault handling
    /// </summary>
    public class ConnectionPoolManager
    {
        private static readonly string _connectionString;
        private static readonly int _maxRetryAttempts = 3;
        private static readonly int _retryDelayMs = 1000;

        static ConnectionPoolManager()
        {
            // Get connection string from environment variable (cloud-native approach)
            string envConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                _connectionString = envConnectionString;
            }
            else
            {
                // Fallback to Web.config for local development
                _connectionString = ConfigurationManager.ConnectionStrings["sqlCon1"]?.ConnectionString;
            }

            // Ensure connection string has pooling enabled
            if (!string.IsNullOrEmpty(_connectionString))
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                
                // Enable connection pooling (default is true, but explicitly set)
                builder.Pooling = true;
                builder.MinPoolSize = 5;
                builder.MaxPoolSize = 100;
                
                // Set timeouts for cloud environments
                builder.ConnectTimeout = 30;
                builder.CommandTimeout = 30;
                
                // Enable Multiple Active Result Sets for better performance
                builder.MultipleActiveResultSets = true;
                
                // For Azure SQL, enable connection resiliency
                builder.ConnectRetryCount = 3;
                builder.ConnectRetryInterval = 10;
                
                _connectionString = builder.ConnectionString;
            }
        }

        /// <summary>
        /// Get a connection from the pool with retry logic
        /// </summary>
        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string not configured. " +
                    "Set SQL_CONNECTION_STRING environment variable or configure sqlCon1 in Web.config");
            }

            int retryCount = 0;
            Exception lastException = null;

            while (retryCount < _maxRetryAttempts)
            {
                try
                {
                    var connection = new SqlConnection(_connectionString);
                    connection.Open();
                    return connection;
                }
                catch (SqlException ex) when (IsTransientError(ex))
                {
                    lastException = ex;
                    retryCount++;
                    
                    if (retryCount < _maxRetryAttempts)
                    {
                        System.Diagnostics.Trace.TraceWarning(
                            $"Transient SQL error occurred. Retry attempt {retryCount} of {_maxRetryAttempts}. Error: {ex.Message}");
                        
                        System.Threading.Thread.Sleep(_retryDelayMs * retryCount);
                    }
                }
                catch (Exception ex)
                {
                    // Non-transient error, throw immediately
                    System.Diagnostics.Trace.TraceError($"SQL connection error: {ex.Message}");
                    throw;
                }
            }

            // All retries exhausted
            throw new InvalidOperationException(
                $"Failed to connect to database after {_maxRetryAttempts} attempts", 
                lastException);
        }

        /// <summary>
        /// Execute a command with automatic connection management and retry logic
        /// </summary>
        public static T ExecuteWithRetry<T>(Func<SqlConnection, T> operation)
        {
            using (var connection = GetConnection())
            {
                return operation(connection);
            }
        }

        /// <summary>
        /// Execute a non-query command with automatic connection management
        /// </summary>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(commandText, connection))
            {
                command.CommandType = commandType;
                command.CommandTimeout = 30;
                
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute a scalar command with automatic connection management
        /// </summary>
        public static object ExecuteScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(commandText, connection))
            {
                command.CommandType = commandType;
                command.CommandTimeout = 30;
                
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Execute a reader command with automatic connection management
        /// </summary>
        public static DataTable ExecuteReader(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(commandText, connection))
            {
                command.CommandType = commandType;
                command.CommandTimeout = 30;
                
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// Check if SQL exception is transient (can be retried)
        /// </summary>
        private static bool IsTransientError(SqlException ex)
        {
            // Azure SQL transient error codes
            int[] transientErrorNumbers = new int[]
            {
                -2,     // Timeout
                -1,     // Connection broken
                2,      // Network error
                53,     // Connection broken
                64,     // Error on server
                233,    // Connection initialization error
                10053,  // Transport-level error
                10054,  // Connection forcibly closed
                10060,  // Network or instance-specific error
                10061,  // Connection refused
                40197,  // Service error processing request
                40501,  // Service is busy
                40613,  // Database unavailable
                49918,  // Cannot process request
                49919,  // Cannot process create or update request
                49920   // Cannot process request
            };

            foreach (SqlError error in ex.Errors)
            {
                if (Array.IndexOf(transientErrorNumbers, error.Number) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Test database connectivity
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get connection string (for diagnostics only - do not log in production)
        /// </summary>
        public static string GetConnectionStringInfo()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return "Not configured";
            }

            var builder = new SqlConnectionStringBuilder(_connectionString);
            // Return sanitized version without password
            return $"Server: {builder.DataSource}, Database: {builder.InitialCatalog}, Pooling: {builder.Pooling}";
        }
    }
}
