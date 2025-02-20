﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class is for manage entities in the database.
    /// </summary>
    public partial class hospitecContext : DbContext
    {
        public hospitecContext()
        {
        }

        public hospitecContext(DbContextOptions<hospitecContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bed> Bed { get; set; }
        public virtual DbSet<ClinicRecord> ClinicRecord { get; set; }
        public virtual DbSet<MedicalEquipment> MedicalEquipment { get; set; }
        public virtual DbSet<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
        public virtual DbSet<MedicalProcedureRecord> MedicalProcedureRecord { get; set; }
        public virtual DbSet<MedicalProcedureReservation> MedicalProcedureReservation { get; set; }
        public virtual DbSet<MedicalProcedures> MedicalProcedures { get; set; }
        public virtual DbSet<MedicalRoom> MedicalRoom { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bed>(entity =>
            {
                entity.HasKey(e => e.IdBed)
                    .HasName("bed_pkey");

                entity.ToTable("bed", "doctor");

                entity.Property(e => e.IdBed)
                    .HasColumnName("id_bed")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdRoom).HasColumnName("id_room");

                entity.Property(e => e.IsIcu).HasColumnName("is_icu");

                entity.HasOne(d => d.IdRoomNavigation)
                    .WithMany(p => p.Bed)
                    .HasForeignKey(d => d.IdRoom)
                    .HasConstraintName("bed_id_room_fkey");
            });

            modelBuilder.Entity<ClinicRecord>(entity =>
            {
                entity.HasKey(e => new { e.Identification, e.PathologyName, e.DiagnosticDate })
                    .HasName("clinic_record_pkey");

                entity.ToTable("clinic_record", "doctor");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.PathologyName)
                    .HasColumnName("pathology_name")
                    .HasMaxLength(30);

                entity.Property(e => e.DiagnosticDate)
                    .HasColumnName("diagnostic_date")
                    .HasColumnType("date");

                entity.Property(e => e.Treatment)
                    .HasColumnName("treatment")
                    .HasMaxLength(1000);

                entity.HasOne(d => d.IdentificationNavigation)
                    .WithMany(p => p.ClinicRecord)
                    .HasForeignKey(d => d.Identification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("clinic_record_identification_fkey");
            });

            modelBuilder.Entity<MedicalEquipment>(entity =>
            {
                entity.HasKey(e => e.SerialNumber)
                    .HasName("medical_equipment_pkey");

                entity.ToTable("medical_equipment", "doctor");

                entity.Property(e => e.SerialNumber)
                    .HasColumnName("serial_number")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasColumnName("provider")
                    .HasMaxLength(200);

                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            modelBuilder.Entity<MedicalEquipmentBed>(entity =>
            {
                entity.HasKey(e => new { e.IdBed, e.SerialNumber })
                    .HasName("medical_equipment_bed_pkey");

                entity.ToTable("medical_equipment_bed", "doctor");

                entity.Property(e => e.IdBed).HasColumnName("id_bed");

                entity.Property(e => e.SerialNumber)
                    .HasColumnName("serial_number")
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdBedNavigation)
                    .WithMany(p => p.MedicalEquipmentBed)
                    .HasForeignKey(d => d.IdBed)
                    .HasConstraintName("medical_equipment_bed_id_bed_fkey");

                entity.HasOne(d => d.SerialNumberNavigation)
                    .WithMany(p => p.MedicalEquipmentBed)
                    .HasForeignKey(d => d.SerialNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("medical_equipment_bed_serial_number_fkey");
            });

            modelBuilder.Entity<MedicalProcedureRecord>(entity =>
            {
                entity.HasKey(e => new { e.Identification, e.PathologyName, e.ProcedureName, e.DiagnosticDate, e.OperationExecutionDate })
                    .HasName("medical_procedure_record_pkey");

                entity.ToTable("medical_procedure_record", "doctor");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.PathologyName)
                    .HasColumnName("pathology_name")
                    .HasMaxLength(30);

                entity.Property(e => e.ProcedureName)
                    .HasColumnName("procedure_name")
                    .HasMaxLength(50);

                entity.Property(e => e.DiagnosticDate)
                    .HasColumnName("diagnostic_date")
                    .HasColumnType("date");

                entity.Property(e => e.OperationExecutionDate)
                    .HasColumnName("operation_execution_date")
                    .HasColumnType("date");

                entity.HasOne(d => d.ProcedureNameNavigation)
                    .WithMany(p => p.MedicalProcedureRecord)
                    .HasForeignKey(d => d.ProcedureName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("medical_procedure_record_procedure_name_fkey");

                entity.HasOne(d => d.ClinicRecord)
                    .WithMany(p => p.MedicalProcedureRecord)
                    .HasForeignKey(d => new { d.Identification, d.PathologyName, d.DiagnosticDate })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("medical_procedure_record_identification_pathology_name_dia_fkey");
            });

            modelBuilder.Entity<MedicalProcedureReservation>(entity =>
            {
                entity.HasKey(e => new { e.Identification, e.CheckInDate, e.Name })
                    .HasName("medical_procedure_reservation_pkey");

                entity.ToTable("medical_procedure_reservation", "doctor");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.CheckInDate)
                    .HasColumnName("check_in_date")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.HasOne(d => d.NameNavigation)
                    .WithMany(p => p.MedicalProcedureReservation)
                    .HasForeignKey(d => d.Name)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("medical_procedure_reservation_name_fkey");

                entity.HasOne(d => d.Reservation)
                    .WithMany(p => p.MedicalProcedureReservation)
                    .HasForeignKey(d => new { d.Identification, d.CheckInDate })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("medical_procedure_reservation_identification_check_in_date_fkey");
            });

            modelBuilder.Entity<MedicalProcedures>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("medical_procedures_pkey");

                entity.ToTable("medical_procedures", "doctor");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.RecoveringDays).HasColumnName("recovering_days");
            });

            modelBuilder.Entity<MedicalRoom>(entity =>
            {
                entity.HasKey(e => e.IdRoom)
                    .HasName("medical_room_pkey");

                entity.ToTable("medical_room", "doctor");

                entity.Property(e => e.IdRoom)
                    .HasColumnName("id_room")
                    .ValueGeneratedNever();

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CareType)
                    .IsRequired()
                    .HasColumnName("care_type")
                    .HasMaxLength(50);

                entity.Property(e => e.FloorNumber).HasColumnName("floor_number");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Identification)
                    .HasName("patient_pkey");

                entity.ToTable("patient", "admin");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.PatientPassword)
                    .IsRequired()
                    .HasColumnName("patient_password")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdentificationNavigation)
                    .WithOne(p => p.Patient)
                    .HasForeignKey<Patient>(d => d.Identification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("patient_identification_fkey");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Ignore(e => e.External);

                entity.HasKey(e => e.Identification)
                    .HasName("person_pkey");

                entity.ToTable("person", "admin");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.BirthDate)
                    .HasColumnName("birth_date")
                    .HasColumnType("date");

                entity.Property(e => e.Canton)
                    .IsRequired()
                    .HasColumnName("canton")
                    .HasMaxLength(40);

                entity.Property(e => e.ExactAddress)
                    .IsRequired()
                    .HasColumnName("exact_address")
                    .HasMaxLength(500);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(15);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasMaxLength(15);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnName("phone_number")
                    .HasMaxLength(8);

                entity.Property(e => e.Province)
                    .IsRequired()
                    .HasColumnName("province")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {

                entity.HasKey(e => new { e.Identification, e.CheckInDate })
                    .HasName("reservation_pkey");

                entity.ToTable("reservation", "admin");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.CheckInDate)
                    .HasColumnName("check_in_date")
                    .HasColumnType("date");

                entity.Property(e => e.CheckOutDate)
                    .HasColumnName("check_out_date")
                    .HasColumnType("date");

                entity.Property(e => e.IdBed).HasColumnName("id_bed");

                entity.HasOne(d => d.IdBedNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.IdBed)
                    .HasConstraintName("reservation_id_bed_fkey");

                entity.HasOne(d => d.IdentificationNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.Identification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reservation_identification_fkey");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("role_pkey");

                entity.ToTable("role", "admin");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(15);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => new { e.Name, e.Identification })
                    .HasName("staff_pkey");

                entity.ToTable("staff", "admin");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(15);

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(12);

                entity.Property(e => e.AdmissionDate)
                    .HasColumnName("admission_date")
                    .HasColumnType("date");

                entity.Property(e => e.StaffPassword)
                    .IsRequired()
                    .HasColumnName("staff_password")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdentificationNavigation)
                    .WithMany(p => p.Staff)
                    .HasForeignKey(d => d.Identification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_identification_fkey");

                entity.HasOne(d => d.NameNavigation)
                    .WithMany(p => p.Staff)
                    .HasForeignKey(d => d.Name)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_name_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
