using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace DBProject.DAL
{
    /// <summary>
    /// Entity Framework 6 DbContext for Hospital Management System.
    /// Configured with Azure SQL connection resiliency (execution strategy with retry logic)
    /// to handle transient faults in cloud environments.
    /// </summary>
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext()
            : base("name=sqlCon1")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // No entity mappings needed — DAL uses raw SQL / stored procedures via Database.Connection
            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Azure SQL execution strategy that enables automatic retry on transient failures.
    /// Implements connection resiliency for cloud deployments.
    /// </summary>
    public class AzureSqlExecutionStrategy : SqlAzureExecutionStrategy
    {
        // Uses default retry count (5) and default max delay (30 seconds)
        public AzureSqlExecutionStrategy() : base() { }
    }

    /// <summary>
    /// DbConfiguration that registers the Azure SQL execution strategy
    /// for automatic transient fault handling and connection resiliency.
    /// </summary>
    public class HospitalDbConfiguration : DbConfiguration
    {
        public HospitalDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new AzureSqlExecutionStrategy());
        }
    }
}
