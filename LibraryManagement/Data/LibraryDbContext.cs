using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace LibraryManagement.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "library.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Author).HasMaxLength(300);
                entity.Property(e => e.Publisher).HasMaxLength(300);
                entity.Property(e => e.Category).HasMaxLength(200);
            });

            // Configure BorrowRecord entity
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorrowerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Rank).HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.Unit).HasMaxLength(200);
                entity.Property(e => e.BorrowerPhone).HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                
                // Ignore the BorrowedBooks collection as it's loaded separately
                entity.Ignore(e => e.BorrowedBooks);
            });

            // Configure BorrowedBook entity
            modelBuilder.Entity<BorrowedBook>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorrowRecordId).IsRequired();
                entity.Property(e => e.BookId).IsRequired();
                entity.Property(e => e.BookTitle).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Author).HasMaxLength(300);
                entity.Property(e => e.Quantity).IsRequired();
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            });
        }
    }
}
