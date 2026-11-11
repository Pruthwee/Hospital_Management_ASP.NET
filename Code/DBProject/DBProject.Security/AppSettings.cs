using System;
using System.Collections.Generic;
using System.Configuration;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace DBProject.Security
{
    /// <summary>
    /// Provides runtime configuration by reading from AWS Systems Manager Parameter Store.
    /// This replaces Web.config transformation files (Web.Debug.config, Web.Release.config)
    /// with environment-specific settings injected at runtime.
    /// </summary>
    public static class AppSettings
    {
        private static readonly Lazy<AmazonSimpleSystemsManagementClient> _ssmClient =
            new Lazy<AmazonSimpleSystemsManagementClient>(() =>
            {
                var region = ConfigurationManager.AppSettings["AWSRegion"] ?? "us-east-1";
                return new AmazonSimpleSystemsManagementClient(
                    RegionEndpoint.GetBySystemName(region));
            });

        private static readonly string _parameterPath =
            ConfigurationManager.AppSettings["App:ParameterPath"] ??
            Environment.GetEnvironmentVariable("App__ParameterPath") ??
            "/clinicmanagementsystem/";

        private static Dictionary<string, string> _cachedParameters;
        private static DateTime _lastLoad = DateTime.MinValue;
        private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets a setting value from Parameter Store, falling back to
        /// appSettings in Web.config for local development.
        /// </summary>
        public static string GetSetting(string key)
        {
            // Check cached Parameter Store values first
            EnsureParametersLoaded();

            if (_cachedParameters.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fall back to Web.config appSettings (local development)
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Gets a connection string from Parameter Store, falling back to
        /// connectionStrings in Web.config.
        /// </summary>
        public static string GetConnectionString(string name)
        {
            EnsureParametersLoaded();

            var key = $"ConnectionStrings:{name}";
            if (_cachedParameters.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fall back to Web.config connectionStrings
            var cs = ConfigurationManager.ConnectionStrings[name];
            return cs?.ConnectionString;
        }

        private static void EnsureParametersLoaded()
        {
            if (_cachedParameters != null &&
                DateTime.UtcNow - _lastLoad < _cacheTtl)
            {
                return;
            }

            _cachedParameters = LoadParametersFromStore();
            _lastLoad = DateTime.UtcNow;
        }

        private static Dictionary<string, string> LoadParametersFromStore()
        {
            var result = new Dictionary<string, string>();

            try
            {
                var request = new GetParametersByPathRequest
                {
                    Path = _parameterPath,
                    Recursive = true,
                    WithDecryption = true
                };

                string nextToken = null;
                do
                {
                    request.NextToken = nextToken;
                    var response = _ssmClient.Value.GetParametersByPathAsync(request)
                        .GetAwaiter().GetResult();

                    foreach (var param in response.Parameters)
                    {
                        // Convert /path/to/KeyName to KeyName
                        var key = param.Name.Substring(_parameterPath.Length)
                            .TrimStart('/')
                            .Replace("/", ":");
                        result[key] = param.Value;
                    }

                    nextToken = response.NextToken;
                } while (!string.IsNullOrEmpty(nextToken));
            }
            catch
            {
                // Parameter Store not available (local development)
                // Fall back to Web.config values
            }

            return result;
        }
    }
}
