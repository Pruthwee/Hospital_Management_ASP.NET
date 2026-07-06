using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for Patient entity.</summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patient");
        builder.HasKey(e => e.PatientId);
        builder.Property(e => e.PatientId).HasColumnName("PatientID").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(e => e.Address).HasMaxLength(40);
        builder.Property(e => e.BirthDate).IsRequired();
        builder.Property(e => e.Gender).HasMaxLength(1).IsFixedLength().IsRequired();

        builder.HasOne(e => e.Login)
            .WithOne(l => l.Patient)
            .HasForeignKey<Patient>(e => e.PatientId);
    }
}
