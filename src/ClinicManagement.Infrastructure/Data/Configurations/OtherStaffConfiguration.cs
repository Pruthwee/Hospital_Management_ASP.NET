using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for OtherStaff entity.</summary>
public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("OtherStaff");
        builder.HasKey(e => e.StaffId);
        builder.Property(e => e.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(e => e.Address).HasMaxLength(30);
        builder.Property(e => e.Designation).HasMaxLength(15).IsRequired();
        builder.Property(e => e.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(e => e.HighestQualification).HasColumnName("Highest_Qualification").HasMaxLength(50);
        builder.Property(e => e.Salary);
    }
}
