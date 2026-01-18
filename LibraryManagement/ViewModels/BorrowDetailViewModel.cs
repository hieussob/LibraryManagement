using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    public partial class BorrowDetailViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private BorrowRecord borrowRecord;

        [ObservableProperty]
        private ObservableCollection<BorrowedBook> borrowedBooks;

        [ObservableProperty]
        private bool canReturn;

        public BorrowDetailViewModel(BorrowRecord record)
        {
            _dataService = DataService.Instance;
            borrowRecord = record;
            borrowedBooks = new ObservableCollection<BorrowedBook>(record.BorrowedBooks);
            canReturn = record.Status == "Đang mượn" || record.Status == "Quá hạn";
        }

        [RelayCommand]
        private void ReturnBooks(Window window)
        {
            if (BorrowRecord.Status == "Đã trả")
            {
                MessageBox.Show("Phiếu mượn này đã được trả!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Xác nhận trả sách?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Cập nhật số lượng sách khả dụng
                    foreach (var borrowedBook in BorrowRecord.BorrowedBooks)
                    {
                        var book = _dataService.GetBookById(borrowedBook.BookId);
                        if (book != null)
                        {
                            book.AvailableQuantity += borrowedBook.Quantity;
                            _dataService.UpdateBook(book);
                        }
                    }

                    // Cập nhật trạng thái phiếu mượn
                    BorrowRecord.ReturnDate = DateTime.Now;
                    BorrowRecord.Status = "Đã trả";
                    _dataService.UpdateBorrowRecord(BorrowRecord);

                    MessageBox.Show("Trả sách thành công!", "Thông báo",
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
}
