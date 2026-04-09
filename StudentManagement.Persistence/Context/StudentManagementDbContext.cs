using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Persistence.Entities;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Persistence.Context;

public partial class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("tblRefreshTokens");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            
            entity.Ignore(e => e.CreatedDate);
            entity.Ignore(e => e.IsDeleted);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.UserId).HasConstraintName("FK_tblRefreshTokens_tblUsers");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("tblStudents");
            entity.HasIndex(e => e.CreatedDate, "IX_tblStudents_CreatedDate")
                .IsDescending()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("tblUsers");
            entity.HasIndex(e => e.Email, "UQ_tblUsers_Email").IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Role).HasDefaultValue("User");
            
            entity.Ignore(e => e.Username);
            entity.Ignore(e => e.IsDeleted);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
