using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MovieApp.Models
{
    public partial class JAMDContext : DbContext
    {
        public JAMDContext()
        {
        }

        public JAMDContext(DbContextOptions<JAMDContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAdmin> TblAdmin { get; set; }
        public virtual DbSet<TblComment> TblComment { get; set; }
        public virtual DbSet<TblMovie> TblMovie { get; set; }
        public virtual DbSet<TblUser> TblUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=JAMD;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblAdmin>(entity =>
            {
                entity.HasKey(e => e.AdminId);

                entity.ToTable("tblAdmin");

                entity.Property(e => e.AdminId)
                    .HasColumnName("adminID")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblAdmin)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblAdmin_tblUser");
            });

            modelBuilder.Entity<TblComment>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("tblComment");

                entity.Property(e => e.CommentId)
                    .HasColumnName("commentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MovieId).HasColumnName("movieID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblComment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblComment_tblUser");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.TblComment)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblComment_tblMovie");
            });

            modelBuilder.Entity<TblMovie>(entity =>
            {
                entity.HasKey(e => e.MovieId);

                entity.ToTable("tblMovie");

                entity.Property(e => e.MovieId)
                    .HasColumnName("movieID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Director)
                    .HasColumnName("director")
                    .IsUnicode(false);

                entity.Property(e => e.Dislikes).HasColumnName("dislikes");

                entity.Property(e => e.Likes).HasColumnName("likes");

                entity.Property(e => e.MovieName)
                    .HasColumnName("movieName")
                    .IsUnicode(false);

                entity.Property(e => e.PosterLink)
                    .HasColumnName("posterLink")
                    .IsUnicode(false);

                entity.Property(e => e.Synopsis)
                    .HasColumnName("synopsis")
                    .HasColumnType("text");

                entity.Property(e => e.TrailerLink)
                    .HasColumnName("trailerLink")
                    .IsUnicode(false);

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tblUser");

                entity.Property(e => e.UserId)
                    .HasColumnName("userID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("userName")
                    .IsUnicode(false);
            });
        }
    }
}
