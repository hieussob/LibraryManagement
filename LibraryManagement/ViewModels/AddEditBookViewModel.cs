using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using System;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    public partial class AddEditBookViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly QRCodeService _qrCodeService;
        private readonly bool _isEditMode;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private Book book;

        [ObservableProperty]
        private string windowTitle = "Thêm sách mới";

        public AddEditBookViewModel(Book? existingBook = null)
        {
            _dataService = DataService.Instance;
            _qrCodeService = new QRCodeService();
            
            if (existingBook != null)
            {
                _isEditMode = true;
                book = existingBook.Clone();
                WindowTitle = "Sửa thông tin sách";
            }
            else
            {
                _isEditMode = false;
                book = new Book
                {
                    PublishYear = DateTime.Now.Year,
                    Quantity = 1,
                    AvailableQuantity = 1
                };
                // Tự động tạo QR code cho sách mới
                book.QrCode = _qrCodeService.GenerateQRCode(book.Id);
            }

            // Subscribe to property changes to update CanSave
            Book.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Book.Title) ||
                    e.PropertyName == nameof(Book.Author) ||
                    e.PropertyName == nameof(Book.Quantity) ||
                    e.PropertyName == nameof(Book.QrCode))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            };
        }

        [RelayCommand]
        private void GenerateQRCode()
        {
            // Sử dụng UUID (Id) thay vì ISBN cho QR code
            Book.QrCode = _qrCodeService.GenerateQRCode(Book.Id);
            OnPropertyChanged(nameof(Book));
            MessageBox.Show("Đã tạo mã QR thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ExportQR()
        {
            if (string.IsNullOrEmpty(Book.QrCode))
            {
                MessageBox.Show("Vui lòng tạo mã QR trước khi xuất!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|All files|*.*",
                DefaultExt = "png",
                FileName = $"QR_{Book.Title}_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (_qrCodeService.ExportQRCodeToFile(Book.QrCode, saveFileDialog.FileName))
                {
                    MessageBox.Show($"Xuất mã QR thành công!\nLưu tại: {saveFileDialog.FileName}",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Xuất mã QR thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Book.Title) &&
                   !string.IsNullOrWhiteSpace(Book.Author) &&
                   Book.Quantity > 0;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save(Window window)
        {
            try
            {
                // Tạo QR code nếu chưa có (sử dụng UUID)
                if (string.IsNullOrEmpty(Book.QrCode))
                {
                    Book.QrCode = _qrCodeService.GenerateQRCode(Book.Id);
                }

                if (_isEditMode)
                {
                    _dataService.UpdateBook(Book);
                    MessageBox.Show("Cập nhật sách thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _dataService.AddBook(Book);
                    MessageBox.Show("Thêm sách mới thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

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
