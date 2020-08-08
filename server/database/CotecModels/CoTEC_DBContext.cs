using Microsoft.EntityFrameworkCore;

namespace HospiTec_Server.CotecModels
{
    /// <summary>
    /// This class is for manage the database of project CoTEC-2020
    /// This class use Entity Framework Core.
    /// See https://github.com/CE-DB/CoTEC2020-DBServer for more information
    /// about the project.
    /// </summary>
    public class CoTEC_DBContext : DbContext
    {
        public CoTEC_DBContext()
        {
        }

        public CoTEC_DBContext(DbContextOptions<CoTEC_DBContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patient { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "admin");

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Identification)
                    .HasName("PK_Patient_CoTEC");

                entity.ToTable("Patient", "healthcare");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(50);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
