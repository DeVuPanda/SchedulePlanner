using System;
using System.Collections.Generic;
using ScheduleDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace ScheduleInfrastructure;

public partial class UniversityPracticeContext : DbContext
{
    public UniversityPracticeContext()
    {
    }

    public UniversityPracticeContext(DbContextOptions<UniversityPracticeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<DaysOfWeek> DaysOfWeeks { get; set; }

    public virtual DbSet<FinalSchedule> FinalSchedules { get; set; }

    public virtual DbSet<MaxPairsPerDay> MaxPairsPerDays { get; set; }

    public virtual DbSet<PairNumber> PairNumbers { get; set; }

    public virtual DbSet<SchedulePreference> SchedulePreferences { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=Denis\\SQLEXPRESS; Database=UniversityPractice; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC07AC4D0759");

            entity.HasIndex(e => e.RoomNumber, "UQ__Classroo__AE10E07A4939CB49").IsUnique();

            entity.Property(e => e.RoomNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DaysOfWeek>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DaysOfWe__3214EC0712B15AF5");

            entity.ToTable("DaysOfWeek");

            entity.HasIndex(e => e.DayName, "UQ__DaysOfWe__04F2C90B173BA74E").IsUnique();

            entity.Property(e => e.DayName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FinalSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FinalSch__3214EC07D2C7ED1B");

            entity.ToTable("FinalSchedule");

            entity.HasIndex(e => new { e.TeacherId, e.DayOfWeekId, e.PairNumberId }, "FinalSchedule_index_0").IsUnique();

            entity.HasIndex(e => new { e.ClassroomId, e.DayOfWeekId, e.PairNumberId }, "FinalSchedule_index_1").IsUnique();

            entity.Property(e => e.IsClassroomAssigned).HasDefaultValue(false);

            entity.HasOne(d => d.Classroom).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalSche__Class__693CA210");

            entity.HasOne(d => d.DayOfWeek).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.DayOfWeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalSche__DayOf__6A30C649");

            entity.HasOne(d => d.PairNumber).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.PairNumberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalSche__PairN__6B24EA82");

            entity.HasOne(d => d.Subject).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalSche__Subje__68487DD7");

            entity.HasOne(d => d.Teacher).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalSche__Teach__6754599E");
        });

        modelBuilder.Entity<MaxPairsPerDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MaxPairs__3214EC07D200000F");

            entity.ToTable("MaxPairsPerDay");

            entity.HasIndex(e => e.MaxPairs, "UQ__MaxPairs__69208AF036EB1226").IsUnique();
        });

        modelBuilder.Entity<PairNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PairNumb__3214EC07128BC65E");

            entity.HasIndex(e => e.Description, "UQ__PairNumb__4EBBBAC95DDEA633").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SchedulePreference>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC0760746077");

            entity.HasOne(d => d.DayOfWeek).WithMany(p => p.SchedulePreferences)
                .HasForeignKey(d => d.DayOfWeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleP__DayOf__6477ECF3");

            entity.HasOne(d => d.MaxPairsPerDay).WithMany(p => p.SchedulePreferences)
                .HasForeignKey(d => d.MaxPairsPerDayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleP__MaxPa__66603565");

            entity.HasOne(d => d.PairNumber).WithMany(p => p.SchedulePreferences)
                .HasForeignKey(d => d.PairNumberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleP__PairN__656C112C");

            entity.HasOne(d => d.Subject).WithMany(p => p.SchedulePreferences)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleP__Subje__6383C8BA");

            entity.HasOne(d => d.Teacher).WithMany(p => p.SchedulePreferences)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleP__Teach__628FA481");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subjects__3214EC076FF474BA");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Teacher).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subjects__Teache__619B8048");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC072074A2FE");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E4AF17B7").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__60A75C0F");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07400A6CA5");

            entity.HasIndex(e => e.RoleName, "UQ__UserRole__8A2B6160CBFE92A4").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        //modelBuilder.Entity<Group>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK__Group__3214EC07XXXXXXX");

        //    entity.HasIndex(e => e.GroupName, "UQ__Group__XXXXXXX").IsUnique();

        //    entity.Property(e => e.GroupName)
        //        .HasMaxLength(100)
        //        .IsUnicode(false);

        //    entity.Property(e => e.Course)
        //        .HasMaxLength(100)
        //        .IsUnicode(false);

        //    entity.Property(e => e.Speciality)
        //        .HasMaxLength(100)
        //        .IsUnicode(false);

        //    entity.HasMany(g => g.Subjects)
        //        .WithOne()
        //        .HasForeignKey("GroupId")
        //        .OnDelete(DeleteBehavior.ClientSetNull);

        //    entity.HasMany(g => g.SchedulePreferences)
        //        .WithOne()
        //        .HasForeignKey("GroupId")
        //        .OnDelete(DeleteBehavior.ClientSetNull);

        //    entity.HasMany(g => g.FinalSchedules)
        //        .WithOne()
        //        .HasForeignKey("GroupId")
        //        .OnDelete(DeleteBehavior.ClientSetNull);
        //});


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
