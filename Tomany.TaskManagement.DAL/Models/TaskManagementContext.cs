using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tomany.TaskManagement.DAL.Models;

public partial class TaskManagementContext : DbContext
{
    public TaskManagementContext()
    {
    }

    public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskHistory> TaskHistories { get; set; }

    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

    private string? GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var cs = GetConnectionString();
            if (!string.IsNullOrWhiteSpace(cs))
            {
                optionsBuilder.UseSqlServer(cs);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__account__46A222CDC8F8F185");

            entity.ToTable("account", tb => tb.HasTrigger("trg_UpdateTimestamp"));

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__profile__46A222CD580B70F7");

            entity.ToTable("profile", tb => tb.HasTrigger("trg_UpdateProfileTimestamp"));

            entity.Property(e => e.AccountId)
                .ValueGeneratedNever()
                .HasColumnName("account_id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Account).WithOne(p => p.Profile)
                .HasForeignKey<Profile>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__profile__account__2A4B4B5E");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__project__BC799E1FD75C63E2");

            entity.ToTable("project");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.ProjectDescription)
                .HasColumnType("text")
                .HasColumnName("project_description");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("project_status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__project__create___2F10007B");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__project___59F479D9308E540F");

            entity.ToTable("project_member");

            entity.Property(e => e.ProjectMemberId).HasColumnName("project_member_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RoleInProject)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_in_project");

            entity.HasOne(d => d.Account).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__project_m__accou__38996AB5");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__project_m__proje__37A5467C");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__task__0492148D7D1DFE36");

            entity.ToTable("task");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.LinkSubmission)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("link_submission");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.TaskDescription)
                .HasColumnType("text")
                .HasColumnName("task_description");
            entity.Property(e => e.TaskName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("task_name");
            entity.Property(e => e.TaskStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("task_status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__task__assigned_t__34C8D9D1");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__task__project_id__33D4B598");
        });

        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__task_his__096AA2E9F50C765C");

            entity.ToTable("task_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.ProgressPercent).HasColumnName("progress_percent");
            entity.Property(e => e.StatusChange)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status_change");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskHistories)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__task_hist__task___412EB0B6");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaskHistories)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__task_hist__updat__4222D4EF");
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__user_act__9E2397E084C7001D");

            entity.ToTable("user_activity_log");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("action_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ip_address");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_agent");

            entity.HasOne(d => d.Account).WithMany(p => p.UserActivityLogs)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_acti__accou__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
