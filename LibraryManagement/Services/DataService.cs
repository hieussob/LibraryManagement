using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibraryManagement.Services
{
    public class DataService
    {
        private static DataService? _instance;
        public static DataService Instance => _instance ??= new DataService();

        public ObservableCollection<Book> Books { get; private set; }
        public ObservableCollection<BorrowRecord> BorrowRecords { get; private set; }

        private DataService()
        {
            Books = new ObservableCollection<Book>();
            BorrowRecords = new ObservableCollection<BorrowRecord>();

            // Initialize database
            using (var db = new LibraryDbContext())
            {
                db.Database.EnsureCreated();
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    // Load books
                    var books = db.Books.ToList();
                    Books.Clear();
                    foreach (var book in books)
                        Books.Add(book);

                    // Load borrow records with borrowed books
                    var records = db.BorrowRecords
                        .Include(r => r.BorrowedBooks)
                        .ToList();
                    BorrowRecords.Clear();
                    foreach (var record in records)
                        BorrowRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public void AddBook(Book book)
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    db.Books.Add(book);
                    db.SaveChanges();
                }
                Books.Add(book);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding book: {ex.Message}");
                throw;
            }
        }

        public void UpdateBook(Book book)
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    db.Books.Update(book);
                    db.SaveChanges();
                }

                var existing = Books.FirstOrDefault(b => b.Id == book.Id);
                if (existing != null)
                {
                    var index = Books.IndexOf(existing);
                    Books[index] = book;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating book: {ex.Message}");
                throw;
            }
        }

        public void DeleteBook(string bookId)
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book != null)
                    {
                        db.Books.Remove(book);
                        db.SaveChanges();
                    }
                }

                var existingBook = Books.FirstOrDefault(b => b.Id == bookId);
                if (existingBook != null)
                    Books.Remove(existingBook);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting book: {ex.Message}");
                throw;
            }
        }

        public void AddBorrowRecord(BorrowRecord record)
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    db.BorrowRecords.Add(record);
                    db.SaveChanges();
                }
                BorrowRecords.Add(record);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding borrow record: {ex.Message}");
                throw;
            }
        }

        public void UpdateBorrowRecord(BorrowRecord record)
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    // Load existing record with borrowed books
                    var existingRecord = db.BorrowRecords
                        .Include(r => r.BorrowedBooks)
                        .FirstOrDefault(r => r.Id == record.Id);

                    if (existingRecord != null)
                    {
                        // Update properties
                        db.Entry(existingRecord).CurrentValues.SetValues(record);

                        // Update owned collection (BorrowedBooks)
                        existingRecord.BorrowedBooks.Clear();
                        foreach (var book in record.BorrowedBooks)
                        {
                            existingRecord.BorrowedBooks.Add(book);
                        }

                        db.SaveChanges();
                    }
                }

                var existing = BorrowRecords.FirstOrDefault(r => r.Id == record.Id);
                if (existing != null)
                {
                    var index = BorrowRecords.IndexOf(existing);
                    BorrowRecords[index] = record;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating borrow record: {ex.Message}");
                throw;
            }
        }

        public Book? GetBookById(string id)
        {
            return Books.FirstOrDefault(b => b.Id == id);
        }
    }
}
