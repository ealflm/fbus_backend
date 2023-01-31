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
        public virtual DbSet<FavoriteRoute> FavoriteRoutes { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<RouteStation> RouteStations { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<StartLocation> StartLocations { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentNotification> StudentNotifications { get; set; }
        public virtual DbSet<StudentTrip> StudentTrips { get; set; }
        public virtual DbSet<TrackingLocation> TrackingLocations { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId).ValueGeneratedNever();

                entity.Property(e => e.NotifyToken).IsUnicode(false);

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
                    .HasMaxLength(50);

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
                    .HasMaxLength(128)
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
                    .HasMaxLength(128)
                    .IsFixedLength();
            });

            modelBuilder.Entity<DriverNotification>(entity =>
            {
                entity.HasKey(e => e.NotificationId)
                    .HasName("PK_DriverNotify");

                entity.ToTable("DriverNotification");

                entity.Property(e => e.NotificationId).ValueGeneratedNever();

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

            modelBuilder.Entity<FavoriteRoute>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.StudentId })
                    .HasName("PK_FavoriteTrip");

                entity.ToTable("FavoriteRoute");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Time).HasColumnType("time(0)");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.FavoriteRoutes)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteTrip_Route");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.FavoriteRoutes)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteTrip_Student");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");

                entity.Property(e => e.RouteId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Distance).HasColumnType("decimal(16, 7)");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<RouteStation>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.StationId });

                entity.ToTable("RouteStation");

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.Property(e => e.StationId).HasColumnName("StationID");

                entity.Property(e => e.Distance).HasColumnType("decimal(16, 7)");

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

                entity.Property(e => e.ShiftId)
                    .ValueGeneratedNever()
                    .HasColumnName("ShiftID");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.DriverId).HasColumnName("DriverID");

                entity.Property(e => e.RequestTime).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Shifts)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shift_Driver");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.Shifts)
                    .HasForeignKey(d => d.TripId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shift_Trip");
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

                entity.Property(e => e.Latitude).HasColumnType("decimal(17, 7)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(17, 7)");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.StudentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.DateBan).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("date");

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
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudentNotification>(entity =>
            {
                entity.HasKey(e => e.NotificationId)
                    .HasName("PK_StudentNotify");

                entity.ToTable("StudentNotification");

                entity.Property(e => e.NotificationId).ValueGeneratedNever();

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

            modelBuilder.Entity<TrackingLocation>(entity =>
            {
                entity.ToTable("TrackingLocation");

                entity.Property(e => e.TrackingLocationId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 7)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(18, 7)");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.TrackingLocations)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackingLocation_Driver");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");

                entity.Property(e => e.TripId).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.TimeEnd).HasColumnType("time(0)");

                entity.Property(e => e.TimeStart).HasColumnType("time(0)");

                entity.HasOne(d => d.BusVehicle)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.BusVehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_Bus");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.DriverId)
                    .HasConstraintName("FK_Trip_Driver");

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
