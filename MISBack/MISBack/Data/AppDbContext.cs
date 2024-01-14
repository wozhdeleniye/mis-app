using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MISBack.Data.Entities;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MISBack.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Icd10> Icd10s { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().HasKey(x => x.id);

            modelBuilder.Entity<Speciality>().HasKey(x => x.id);

            modelBuilder.Entity<Inspection>().HasKey(x => x.id);

            modelBuilder.Entity<Patient>().HasKey(x => x.id);

            modelBuilder.Entity<Comment>().HasKey(x => x.id);

            modelBuilder.Entity<Consultation>().HasKey(x => x.id);

            modelBuilder.Entity<Diagnosis>().HasKey(x => x.id);

            modelBuilder.Entity<Icd10>().HasKey(x => x.id);

            modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);

            base.OnModelCreating(modelBuilder);
        }
    }
}
