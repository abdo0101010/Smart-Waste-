using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Models;

public partial class smartwasteContext : DbContext
{
    public smartwasteContext()
    {
    }

    public smartwasteContext(DbContextOptions<smartwasteContext> options)
        : base(options)
    {

    }
  public virtual DbSet<SupportTickets> SupportTickets { get; set; }
    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<HubStaff> HubStaffs { get; set; }

    public virtual DbSet<PickupRequest> PickupRequests { get; set; }

    public virtual DbSet<Recycler> Recyclers { get; set; }

    public virtual DbSet<RequestItem> RequestItems { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRedemption> UserRedemptions { get; set; }

    public virtual DbSet<WasteCategory> WasteCategories { get; set; }
    public virtual DbSet<Admin> Admins { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6AA4CCE99");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Request).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_Request");
            modelBuilder.Entity<PickupRequest>()
         .HasOne(p => p.User)
         .WithMany(u => u.PickupRequests)
         .HasForeignKey(p => p.UserId)
         .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HubStaff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__HubStaff__96D4AAF74529BE54");

            entity.ToTable("HubStaff");

            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Sorter");
        });

        modelBuilder.Entity<PickupRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__PickupRe__33A8519AFA03C46A");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.EstimatedPoints)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FinalPoints)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.HubStaffId).HasColumnName("HubStaffID");
            entity.Property(e => e.PickupDate).HasColumnType("datetime");
            entity.Property(e => e.Qrcode)
                .HasMaxLength(100)
                .HasColumnName("QRCode");
            entity.Property(e => e.RecyclerId).HasColumnName("RecyclerID");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VerificationDate).HasColumnType("datetime");

            entity.HasOne(d => d.HubStaff).WithMany(p => p.PickupRequests)
                .HasForeignKey(d => d.HubStaffId)
                .HasConstraintName("FK_Requests_Staff");

            entity.HasOne(d => d.Recycler).WithMany(p => p.PickupRequests)
                .HasForeignKey(d => d.RecyclerId)
                .HasConstraintName("FK_Requests_Recycler");

            entity.HasOne(d => d.User).WithMany(p => p.PickupRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_User");
        });

        modelBuilder.Entity<Recycler>(entity =>
        {
            entity.HasKey(e => e.RecyclerId).HasName("PK__Recycler__CF8A55D5FEB653EF");

            entity.HasIndex(e => e.Phone, "UQ__Recycler__5C7E359E9AF9A312").IsUnique();

            entity.Property(e => e.RecyclerId).HasColumnName("RecyclerID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Rating)
                .HasDefaultValue(5.0m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Offline");
            entity.Property(e => e.VehicleInfo).HasMaxLength(100);
        });

        modelBuilder.Entity<RequestItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__RequestI__727E83EB51E2336E");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Source).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.RequestItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Items_Category");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestItems)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Items_Request");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.RewardId).HasName("PK__Rewards__8250159966275F67");

            entity.Property(e => e.RewardId).HasColumnName("RewardID");
            entity.Property(e => e.CostPoints).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC6EBB6AB5");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534AFF072C8").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.WalletPoints)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<UserRedemption>(entity =>
        {
            entity.HasKey(e => e.RedemptionId).HasName("PK__UserRede__410680D10B55621C");

            entity.Property(e => e.RedemptionId).HasColumnName("RedemptionID");
            entity.Property(e => e.RedeemedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RewardId).HasColumnName("RewardID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);

            entity.HasOne(d => d.Reward).WithMany(p => p.UserRedemptions)
                .HasForeignKey(d => d.RewardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Redemption_Reward");

            entity.HasOne(d => d.User).WithMany(p => p.UserRedemptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Redemption_User");
        });

        modelBuilder.Entity<WasteCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__WasteCat__19093A2BB1FAC898");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.PointsPerUnit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitType)
                .HasMaxLength(20)
                .HasDefaultValue("Item");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
