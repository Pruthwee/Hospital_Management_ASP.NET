using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace DBProject.DAL
{
    /// <summary>
    /// Helper class that provides typed get/set operations over IDistributedCache (Azure Cache for Redis).
    /// Replaces the InProc HttpSessionState / HttpContext.Session InProc provider with a Redis-backed
    /// distributed cache, enabling stateless horizontal scaling across multiple Azure Container App instances.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replace in-process session state with Azure Cache for Redis via IDistributedCache.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   All cache operations now have async counterparts (SetInt32Async, GetInt32Async, etc.)
    ///   to support async/await patterns in Razor Page OnGetAsync() handlers, preventing
    ///   thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
    ///
    /// Usage:
    ///   Inject IDistributedCache in your PageModel constructor, then call:
    ///     await RedisSessionHelper.SetInt32Async(cache, "idoriginal", userId);
    ///     int id = await RedisSessionHelper.GetInt32Async(cache, "idoriginal") ?? 0;
    ///     await RedisSessionHelper.SetStringAsync(cache, "dID", doctorId.ToString());
    ///     string? dID = await RedisSessionHelper.GetStringAsync(cache, "dID");
    ///
    /// The session key is scoped per-request using a session token stored in a cookie (managed by
    /// ASP.NET Core's ISession middleware, which is configured to use Redis as its backing store).
    /// </summary>
    public static class RedisSessionHelper
    {
        private static readonly DistributedCacheEntryOptions DefaultOptions = new DistributedCacheEntryOptions
        {
            // Session expires after 20 minutes of inactivity — matches ASP.NET default session timeout
            SlidingExpiration = TimeSpan.FromMinutes(20)
        };

        // ─── Int32 (Synchronous) ──────────────────────────────────────────────────

        /// <summary>Stores a nullable int value in Redis under the given key.</summary>
        public static void SetInt32(IDistributedCache cache, string key, int? value)
        {
            if (value.HasValue)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value.Value.ToString());
                cache.Set(key, bytes, DefaultOptions);
            }
            else
            {
                cache.Remove(key);
            }
        }

        /// <summary>Retrieves a nullable int value from Redis for the given key.</summary>
        public static int? GetInt32(IDistributedCache cache, string key)
        {
            byte[]? bytes = cache.Get(key);
            if (bytes == null || bytes.Length == 0)
                return null;

            string raw = Encoding.UTF8.GetString(bytes);
            if (int.TryParse(raw, out int result))
                return result;

            return null;
        }

        // ─── Int32 (Async) — cr-dotnet-1034 fix ──────────────────────────────────

        /// <summary>
        /// Asynchronously stores a nullable int value in Redis under the given key.
        /// cr-dotnet-1034: Async variant used in OnGetAsync() handlers to prevent thread pool exhaustion.
        /// </summary>
        public static async Task SetInt32Async(IDistributedCache cache, string key, int? value, CancellationToken cancellationToken = default)
        {
            if (value.HasValue)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value.Value.ToString());
                await cache.SetAsync(key, bytes, DefaultOptions, cancellationToken);
            }
            else
            {
                await cache.RemoveAsync(key, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a nullable int value from Redis for the given key.
        /// cr-dotnet-1034: Async variant used in OnGetAsync() handlers to prevent thread pool exhaustion.
        /// </summary>
        public static async Task<int?> GetInt32Async(IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            byte[]? bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes == null || bytes.Length == 0)
                return null;

            string raw = Encoding.UTF8.GetString(bytes);
            if (int.TryParse(raw, out int result))
                return result;

            return null;
        }

        // ─── String (Synchronous) ─────────────────────────────────────────────────

        /// <summary>Stores a string value in Redis under the given key.</summary>
        public static void SetString(IDistributedCache cache, string key, string? value)
        {
            if (value != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                cache.Set(key, bytes, DefaultOptions);
            }
            else
            {
                cache.Remove(key);
            }
        }

        /// <summary>Retrieves a string value from Redis for the given key.</summary>
        public static string? GetString(IDistributedCache cache, string key)
        {
            byte[]? bytes = cache.Get(key);
            if (bytes == null || bytes.Length == 0)
                return null;

            return Encoding.UTF8.GetString(bytes);
        }

        // ─── String (Async) — cr-dotnet-1034 fix ─────────────────────────────────

        /// <summary>
        /// Asynchronously stores a string value in Redis under the given key.
        /// cr-dotnet-1034: Async variant used in OnGetAsync() handlers to prevent thread pool exhaustion.
        /// </summary>
        public static async Task SetStringAsync(IDistributedCache cache, string key, string? value, CancellationToken cancellationToken = default)
        {
            if (value != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                await cache.SetAsync(key, bytes, DefaultOptions, cancellationToken);
            }
            else
            {
                await cache.RemoveAsync(key, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a string value from Redis for the given key.
        /// cr-dotnet-1034: Async variant used in OnGetAsync() handlers to prevent thread pool exhaustion.
        /// </summary>
        public static async Task<string?> GetStringAsync(IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            byte[]? bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes == null || bytes.Length == 0)
                return null;

            return Encoding.UTF8.GetString(bytes);
        }

        // ─── Object (JSON serialization — Synchronous) ────────────────────────────

        /// <summary>Stores a serializable object as JSON in Redis under the given key.</summary>
        public static void SetObject<T>(IDistributedCache cache, string key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            cache.Set(key, bytes, DefaultOptions);
        }

        /// <summary>Retrieves and deserializes a JSON object from Redis for the given key.</summary>
        public static T? GetObject<T>(IDistributedCache cache, string key)
        {
            byte[]? bytes = cache.Get(key);
            if (bytes == null || bytes.Length == 0)
                return default;

            string json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        // ─── Object (JSON serialization — Async) — cr-dotnet-1034 fix ────────────

        /// <summary>
        /// Asynchronously stores a serializable object as JSON in Redis under the given key.
        /// cr-dotnet-1034: Async variant for use in async handler methods.
        /// </summary>
        public static async Task SetObjectAsync<T>(IDistributedCache cache, string key, T value, CancellationToken cancellationToken = default)
        {
            string json = JsonSerializer.Serialize(value);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await cache.SetAsync(key, bytes, DefaultOptions, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves and deserializes a JSON object from Redis for the given key.
        /// cr-dotnet-1034: Async variant for use in async handler methods.
        /// </summary>
        public static async Task<T?> GetObjectAsync<T>(IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            byte[]? bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes == null || bytes.Length == 0)
                return default;

            string json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        // ─── Remove ───────────────────────────────────────────────────────────────

        /// <summary>Removes a key from Redis.</summary>
        public static void Remove(IDistributedCache cache, string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// Asynchronously removes a key from Redis.
        /// cr-dotnet-1034: Async variant for use in async handler methods.
        /// </summary>
        public static async Task RemoveAsync(IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
    }
}
