using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for Doctor entity.</summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctor");
        builder.HasKey(e => e.DoctorId);
        builder.Property(e => e.DoctorId).HasColumnName("DoctorID").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(e => e.Address).HasMaxLength(40);
        builder.Property(e => e.BirthDate).IsRequired();
        builder.Property(e => e.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(e => e.ChargesPerVisit).HasColumnName("Charges_Per_Visit").IsRequired();
        builder.Property(e => e.MonthlySalary).HasColumnName("MonthlySalary");
        builder.Property(e => e.ReputeIndex).HasColumnName("ReputeIndex");
        builder.Property(e => e.PatientsTreated).HasColumnName("Patients_Treated").HasDefaultValue(0).IsRequired();
        builder.Property(e => e.Qualification).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Specialization).HasMaxLength(100);
        builder.Property(e => e.WorkExperience).HasColumnName("Work_Experience");
        builder.Property(e => e.Status).HasColumnName("status").IsRequired();

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Doctors)
            .HasForeignKey(e => e.DeptNo);

        builder.HasOne(e => e.Login)
            .WithOne(l => l.Doctor)
            .HasForeignKey<Doctor>(e => e.DoctorId);
    }
}
