using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using ClinicSystemTest.Entities;
using Microsoft.Extensions.Hosting;

namespace ClinicSystemTest.Data;

public class ClinicSystemTestContext : IdentityDbContext<IdentityUser>
{
    public ClinicSystemTestContext(DbContextOptions<ClinicSystemTestContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Department>();
        builder.Entity<Doctor>();
        builder.Entity<Patient>();
        builder.Entity<Appointment>();
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        var cascadeFKs = builder.Model.GetEntityTypes()
      .SelectMany(t => t.GetForeignKeys())
      .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {

        if (!options.IsConfigured)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FullTrainSystem;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

}
