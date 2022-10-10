using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FBus.Data.Models;

namespace FBus.Data.Context
{
    public partial class FBusContext : DbContext
    {
        public FBusContext()
        {
        }

        public FBusContext(DbContextOptions<FBusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<BusVehicle> BusVehicles { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<DriverNotification> DriverNotifications { get; set; }
        public virtual DbSet<DriverShift> DriverShifts { get; set; }
        public virtual DbSet<FavoriteTrip> FavoriteTrips { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<RouteStation> RouteStations { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<StartLocation> StartLocations { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentNotification> StudentNotifications { get; set; }
        public virtual DbSet<StudentTrip> StudentTrips { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId).ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsFixedLength();

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BusVehicle>(entity =>
            {
                entity.ToTable("BusVehicle");

                entity.Property(e => e.BusVehicleId).ValueGeneratedNever();

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LicensePlates)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.ToTable("Driver");

                entity.Property(e => e.DriverId).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NotifyToken).IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .IsUnicode(false)
                    .HasColumnName("PhotoURL");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsFixedLength();
            });

            modelBuilder.Entity<DriverNotification>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("PK_DriverNotify");

                entity.ToTable("DriverNotification");

                entity.Property(e => e.NotifyId).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.DriverNotifications)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DriverNotify_Driver");
            });

            modelBuilder.Entity<DriverShift>(entity =>
            {
                entity.ToTable("DriverShift");

                entity.Property(e => e.DriverShiftId).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.DriverShifts)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DriverShift_Driver");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.DriverShifts)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DriverShift_Shift");
            });

            modelBuilder.Entity<FavoriteTrip>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.StudentId });

                entity.ToTable("FavoriteTrip");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Time).HasColumnType("time(0)");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.FavoriteTrips)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteTrip_Route");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.FavoriteTrips)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteTrip_Student");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");

                entity.Property(e => e.RouteId).ValueGeneratedNever();

                entity.Property(e => e.Distance).HasColumnType("decimal(8, 7)");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<RouteStation>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.StationId });

                entity.ToTable("RouteStation");

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.Property(e => e.StationId).HasColumnName("StationID");

                entity.Property(e => e.Distance).HasColumnType("decimal(8, 7)");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.RouteStations)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RouteStation_Route");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.RouteStations)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RouteStation_Station");
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("Shift");

                entity.Property(e => e.ShiftId).ValueGeneratedNever();

                entity.Property(e => e.TimeEnd).HasColumnType("time(0)");

                entity.Property(e => e.TimeStart).HasColumnType("time(0)");
            });

            modelBuilder.Entity<StartLocation>(entity =>
            {
                entity.ToTable("StartLocation");

                entity.Property(e => e.StartLocationId).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 7)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(8, 7)");

                entity.Property(e => e.SearchDate).HasColumnType("datetime");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StartLocations)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StartLocation_Student");
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.ToTable("Station");

                entity.Property(e => e.StationId).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 7)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(8, 7)");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.StudentId).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NotifyToken).IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .IsUnicode(false)
                    .HasColumnName("PhotoURL");

                entity.Property(e => e.Uid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UId");
            });

            modelBuilder.Entity<StudentNotification>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("PK_StudentNotify");

                entity.ToTable("StudentNotification");

                entity.Property(e => e.NotifyId).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentNotifications)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentNotify_Student");
            });

            modelBuilder.Entity<StudentTrip>(entity =>
            {
                entity.ToTable("StudentTrip");

                entity.Property(e => e.StudentTripId).ValueGeneratedNever();

                entity.Property(e => e.CopyOfRoute).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Rate).HasColumnType("decimal(8, 7)");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.StudentTrips)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentTrip_Station");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentTrips)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentTrip_Student");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.StudentTrips)
                    .HasForeignKey(d => d.TripId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentTrip_Trip");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");

                entity.Property(e => e.TripId).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.TimeEnd).HasColumnType("time(0)");

                entity.Property(e => e.TimeStart).HasColumnType("time(0)");

                entity.HasOne(d => d.Bus)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.BusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_Bus");

                entity.HasOne(d => d.DriverShift)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.DriverShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_DriverShift");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_Route");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
