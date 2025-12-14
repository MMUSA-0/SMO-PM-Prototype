using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.EmployeePerformance;

namespace SMO.Infrastructure.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Table configuration
            builder.ToTable("Employees", "Performance");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .HasMaxLength(100);

            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.FullNameAr)
                .HasMaxLength(200);

            builder.Property(e => e.Email)
                .HasMaxLength(150);

            builder.Property(e => e.Department)
                .HasMaxLength(100);

            builder.Property(e => e.Position)
                .HasMaxLength(100);

            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            // Indexes
            builder.HasIndex(e => e.EmployeeCode)
                .IsUnique()
                .HasDatabaseName("IX_Employee_EmployeeCode");

            builder.HasIndex(e => e.Email)
                .HasDatabaseName("IX_Employee_Email");

            builder.HasIndex(e => e.Department)
                .HasDatabaseName("IX_Employee_Department");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Employee_Status");

            builder.HasIndex(e => new { e.Department, e.Status })
                .HasDatabaseName("IX_Employee_Department_Status");

            // Relationships
            builder.HasOne(e => e.Manager)
                .WithMany(m => m.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.PerformanceGoals)
                .WithOne(g => g.Employee)
                .HasForeignKey(g => g.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PerformanceReviews)
                .WithOne(r => r.Employee)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PerformanceRatings)
                .WithOne(r => r.Employee)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Data seeding (sample employees)
            builder.HasData(
                new Employee
                {
                    Id = 1,
                    EmployeeCode = "EMP001",
                    FirstName = "Ahmed",
                    LastName = "Al-Rashid",
                    FullName = "Ahmed Al-Rashid",
                    FullNameAr = "أحمد الراشد",
                    Email = "ahmed.alrashid@smo.gov.sa",
                    Department = "Strategic Planning",
                    Position = "Director",
                    Status = "Active",
                    JoinDate = new DateTime(2020, 1, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Employee
                {
                    Id = 2,
                    EmployeeCode = "EMP002",
                    FirstName = "Sara",
                    LastName = "Al-Zahrani",
                    FullName = "Sara Al-Zahrani",
                    FullNameAr = "سارة الزهراني",
                    Email = "sara.alzahrani@smo.gov.sa",
                    Department = "Performance Management",
                    Position = "Manager",
                    Status = "Active",
                    ManagerId = 1,
                    JoinDate = new DateTime(2021, 3, 1),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
    }
}
