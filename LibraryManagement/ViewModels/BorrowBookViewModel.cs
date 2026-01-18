using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    public partial class BorrowBookViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private BorrowRecord borrowRecord;

        [ObservableProperty]
        private ObservableCollection<BorrowedBook> borrowedBooks;

        [ObservableProperty]
        private int selectedBookQuantity = 1;

        partial void OnBorrowRecordChanged(BorrowRecord value)
        {
            if (value != null)
            {
                value.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(BorrowRecord.BorrowerName) ||
                        e.PropertyName == nameof(BorrowRecord.BorrowerPhone))
                    {
                        SaveCommand.NotifyCanExecuteChanged();
                    }
                };
            }
        }

        // Constructor cho 1 sách
        public BorrowBookViewModel(Book book)
        {
            _dataService = DataService.Instance;
            
            // Khởi tạo BorrowedBooks trước
            BorrowedBooks = new ObservableCollection<BorrowedBook>();
            
            // Thêm sách được chọn vào danh sách mượn
            AddBookToBorrowList(book, 1);
            
            // Khởi tạo BorrowRecord sau
            BorrowRecord = new BorrowRecord();
        }

        // Constructor cho nhiều sách
        public BorrowBookViewModel(System.Collections.Generic.List<Book> books)
        {
            _dataService = DataService.Instance;
            
            // Khởi tạo BorrowedBooks trước
            BorrowedBooks = new ObservableCollection<BorrowedBook>();
            
            // Thêm tất cả các sách vào danh sách mượn
            foreach (var book in books)
            {
                AddBookToBorrowList(book, 1);
            }
            
            // Khởi tạo BorrowRecord sau
            BorrowRecord = new BorrowRecord();
        }
        private void AddBookToBorrowList(Book book, int quantity)
        {
            var borrowedBook = new BorrowedBook
            {
                BookId = book.Id,
                BookTitle = book.Title,
                Author = book.Author,

                Quantity = quantity
            };
            BorrowedBooks.Add(borrowedBook);
            SaveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void UpdateQuantity(BorrowedBook borrowedBook)
        {
            if (borrowedBook.Quantity < 1)
                borrowedBook.Quantity = 1;

            var book = _dataService.GetBookById(borrowedBook.BookId);
            if (book != null && borrowedBook.Quantity > book.AvailableQuantity)
            {
                MessageBox.Show($"Số lượng tối đa có thể mượn: {book.AvailableQuantity}", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                borrowedBook.Quantity = book.AvailableQuantity;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(BorrowRecord.BorrowerName) &&
                   !string.IsNullOrWhiteSpace(BorrowRecord.BorrowerPhone) &&
                   BorrowedBooks.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save(Window window)
        {
            try
            {
                // Tạo một bản ghi BorrowRecord riêng biệt cho từng sách
                foreach (var item in BorrowedBooks)
                {
                    // Tạo bản ghi mượn mới cho từng sách
                    var record = new BorrowRecord
                    {
                        BorrowerName = BorrowRecord.BorrowerName,
                        BorrowerPhone = BorrowRecord.BorrowerPhone,
                        Rank = BorrowRecord.Rank,
                        Position = BorrowRecord.Position,
                        Unit = BorrowRecord.Unit,
                        BorrowDate = BorrowRecord.BorrowDate,
                        DueDate = BorrowRecord.DueDate,
                        Notes = BorrowRecord.Notes,
                        Status = BorrowRecord.DueDate < DateTime.Now ? "Quá hạn" : "Đang mượn"
                    };

                    // Thêm sách vào bản ghi
                    record.BorrowedBooks.Add(new BorrowedBook
                    {
                        BookId = item.BookId,
                        BookTitle = item.BookTitle,
                        Author = item.Author,
                        Quantity = item.Quantity
                    });

                    // Lưu bản ghi
                    _dataService.AddBorrowRecord(record);

                    // Cập nhật số lượng sách khả dụng
                    var book = _dataService.GetBookById(item.BookId);
                    if (book != null)
                    {
                        book.AvailableQuantity -= item.Quantity;
                        _dataService.UpdateBook(book);
                    }
                }

                var bookCount = BorrowedBooks.Count;
                MessageBox.Show($"Đã tạo thành công {bookCount} phiếu mượn sách!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
