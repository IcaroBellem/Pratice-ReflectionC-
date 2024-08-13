using Microsoft.EntityFrameworkCore;
using AppointmentRules.Models;

namespace AppointmentRules.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTask>()
                .HasMany(pt => pt.Members)
                .WithMany(m => m.Tasks) 
                .UsingEntity<Dictionary<string, object>>(
                    "MemberProjectTask",
                    j => j.HasOne<Member>().WithMany().HasForeignKey("MemberId"),
                    j => j.HasOne<ProjectTask>().WithMany().HasForeignKey("ProjectTaskId")
                );

            modelBuilder.Entity<ProjectTask>()
                .HasMany(pt => pt.TimeEntries)
                .WithOne(te => te.Tasks)
                .HasForeignKey(te => te.TaskId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.TimeEntries)
                .WithOne(te => te.Member)
                .HasForeignKey(te => te.MemberId);

            modelBuilder.Entity<Member>()
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<ProjectTask>()
                .Property(pt => pt.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<TimeEntry>()
                .Property(te => te.StartTime)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Name)
                .IsUnique();

           
            modelBuilder.Entity<Member>()
                .Property(m => m.ContractType)
                .HasConversion<int>();

        }
    }
}
