using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for Appointment entity.</summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.HasKey(e => e.AppointId);
        builder.Property(e => e.AppointId).HasColumnName("AppointID").ValueGeneratedOnAdd();
        builder.Property(e => e.DoctorId).HasColumnName("DoctorID");
        builder.Property(e => e.PatientId).HasColumnName("PatientID");
        builder.Property(e => e.Date);
        builder.Property(e => e.AppointmentStatus).HasColumnName("Appointment_Status");
        builder.Property(e => e.BillAmount).HasColumnName("Bill_Amount");
        builder.Property(e => e.BillStatus).HasColumnName("Bill_Status").HasMaxLength(10);
        builder.Property(e => e.DoctorNotification);
        builder.Property(e => e.PatientNotification);
        builder.Property(e => e.FeedbackStatus);
        builder.Property(e => e.Disease).HasMaxLength(100);
        builder.Property(e => e.Progress).HasMaxLength(100);
        builder.Property(e => e.Prescription).HasMaxLength(100);

        builder.HasOne(e => e.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
