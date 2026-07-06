using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for Department entity.</summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(e => e.DeptNo);
        builder.Property(e => e.DeptNo).ValueGeneratedNever();
        builder.Property(e => e.DeptName).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.DeptName).IsUnique();
        builder.Property(e => e.Description).HasMaxLength(1000);
    }
}
