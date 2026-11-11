using System;
using System.Configuration;
using System.Data.SqlClient;
using Amazon;
using Amazon.RDS.Util;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Threading.Tasks;

namespace DBProject.DAL
{
    /// <summary>
    /// Factory that creates SqlConnection instances configured to use
    /// Amazon RDS Proxy with IAM authentication.
    /// The connection string is built from configuration keys:
    ///   Database:Host       - RDS Proxy endpoint
    ///   Database:Port       - Database port (default 1433)
    ///   Database:Name       - Database name
    ///   Database:User       - Database user (IAM auth user)
    ///   Database:SecretName - Secrets Manager secret name/ARN (holds username and password)
    /// </summary>
    internal static class ConnectionFactory
    {
        private static readonly string _proxyEndpoint;
        private static readonly string _databaseName;
        private static readonly string _dbUser;
        private static readonly string _dbPort;
        private static readonly string _secretName;
        private static readonly string _awsRegion;

        static ConnectionFactory()
        {
            _proxyEndpoint = ConfigurationManager.AppSettings["Database:Host"] ?? "";
            _databaseName = ConfigurationManager.AppSettings["Database:Name"] ?? "DBProject";
            _dbUser = ConfigurationManager.AppSettings["Database:User"] ?? "";
            _dbPort = ConfigurationManager.AppSettings["Database:Port"] ?? "1433";
            _secretName = ConfigurationManager.AppSettings["Database:SecretName"] ?? "";
            _awsRegion = ConfigurationManager.AppSettings["AWS:Region"] ?? "us-east-1";
        }

        /// <summary>
        /// Creates a new SqlConnection configured for RDS Proxy with IAM authentication.
        /// The IAM auth token is generated fresh for each connection (valid 15 minutes).
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            var token = RDSAuthTokenGenerator.GenerateAuthToken(
                _awsRegion, _proxyEndpoint, int.Parse(_dbPort), _dbUser);

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _proxyEndpoint,
                InitialCatalog = _databaseName,
                UserID = _dbUser,
                Password = token,
                Encrypt = true,
                TrustServerCertificate = false,
                ConnectTimeout = 30,
                MaxPoolSize = 20,
                MinPoolSize = 5,
                Pooling = true
            };

            return new SqlConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Retrieves the database password from AWS Secrets Manager.
        /// Used as a fallback when IAM auth is not configured.
        /// </summary>
        public static string GetSecretPassword()
        {
            if (string.IsNullOrEmpty(_secretName))
                return null;

            var client = new AmazonSecretsManagerClient(
                RegionEndpoint.GetBySystemName(_awsRegion));

            var response = client.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = _secretName
            }).GetAwaiter().GetResult();

            return response.SecretString;
        }
    }
}
