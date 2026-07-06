using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for LoginTable entity.</summary>
public class LoginTableConfiguration : IEntityTypeConfiguration<LoginTable>
{
    public void Configure(EntityTypeBuilder<LoginTable> builder)
    {
        builder.ToTable("LoginTable");
        builder.HasKey(e => e.LoginId);
        builder.Property(e => e.LoginId).HasColumnName("LoginID").ValueGeneratedOnAdd();
        builder.Property(e => e.Password).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Type).HasColumnName("Type").IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();
    }
}
