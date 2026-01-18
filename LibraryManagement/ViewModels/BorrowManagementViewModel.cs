using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    public partial class BorrowManagementViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private ObservableCollection<BorrowRecord> borrowRecords;

        [ObservableProperty]
        private ObservableCollection<BorrowRecord> filteredRecords;

        [ObservableProperty]
        private BorrowRecord? selectedRecord;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string filterStatus = "Tất cả";

        [ObservableProperty]
        private string filterName = string.Empty;

        [ObservableProperty]
        private string filterRank = string.Empty;

        [ObservableProperty]
        private string filterPosition = string.Empty;

        [ObservableProperty]
        private string filterUnit = string.Empty;

        [ObservableProperty]
        private string filterPhone = string.Empty;

        public ObservableCollection<string> StatusFilters { get; } = new ObservableCollection<string>
        {
            "Tất cả", "Đang mượn", "Đã trả", "Quá hạn"
        };

        public BorrowManagementViewModel()
        {
            _dataService = DataService.Instance;
            borrowRecords = _dataService.BorrowRecords;
            
            // Sắp xếp theo ngày mượn (mới nhất trước)
            var sorted = borrowRecords.OrderByDescending(r => r.BorrowDate).ToList();
            filteredRecords = new ObservableCollection<BorrowRecord>(sorted);
        }

        partial void OnSearchTextChanged(string value) => FilterRecords();
        partial void OnFilterStatusChanged(string value) => FilterRecords();
        partial void OnFilterNameChanged(string value) => FilterRecords();
        partial void OnFilterRankChanged(string value) => FilterRecords();
        partial void OnFilterPositionChanged(string value) => FilterRecords();
        partial void OnFilterUnitChanged(string value) => FilterRecords();
        partial void OnFilterPhoneChanged(string value) => FilterRecords();

        private void FilterRecords()
        {
            var filtered = BorrowRecords.AsEnumerable();

            // Lọc theo text tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(r =>
                    r.BorrowerName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                    r.BorrowerPhone.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                );
            }

            // Lọc theo trạng thái
            if (FilterStatus != "Tất cả")
            {
                filtered = filtered.Where(r => r.Status == FilterStatus);
            }

            // Column-specific filters
            if (!string.IsNullOrWhiteSpace(FilterName))
                filtered = filtered.Where(r => r.BorrowerName.Contains(FilterName, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterRank))
                filtered = filtered.Where(r => r.Rank.Contains(FilterRank, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterPosition))
                filtered = filtered.Where(r => r.Position.Contains(FilterPosition, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterUnit))
                filtered = filtered.Where(r => r.Unit.Contains(FilterUnit, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterPhone))
                filtered = filtered.Where(r => r.BorrowerPhone.Contains(FilterPhone, System.StringComparison.OrdinalIgnoreCase));

            // Sắp xếp theo ngày mượn (mới nhất trước)
            var sorted = filtered.OrderByDescending(r => r.BorrowDate).ToList();
            FilteredRecords = new ObservableCollection<BorrowRecord>(sorted);
        }

        [RelayCommand]
        private void ViewDetail()
        {
            if (SelectedRecord == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn để xem chi tiết!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new BorrowDetailWindow(SelectedRecord);
            if (window.ShowDialog() == true)
            {
                FilterRecords();
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            FilterRecords();
        }

        [RelayCommand]
        private void ViewDetailRow(BorrowRecord record)
        {
            if (record == null) return;

            var window = new BorrowDetailWindow(record);
            if (window.ShowDialog() == true)
            {
                FilterRecords();
            }
        }

        [RelayCommand]
        private void ReturnBookRow(BorrowRecord record)
        {
            if (record == null) return;

            if (record.Status == "Đã trả")
            {
                MessageBox.Show("Đã trả rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Xác nhận trả sách của {record.BorrowerName}?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                record.ReturnDate = System.DateTime.Now;
                record.Status = "Đã trả";
                
                // Tăng số lượng sách còn lại
                foreach (var borrowedBook in record.BorrowedBooks)
                {
                    var bookInLibrary = _dataService.Books.FirstOrDefault(b => b.Id == borrowedBook.BookId);
                    if (bookInLibrary != null)
                    {
                        bookInLibrary.AvailableQuantity += borrowedBook.Quantity;
                        _dataService.UpdateBook(bookInLibrary);
                    }
                }

                _dataService.UpdateBorrowRecord(record);
                FilterRecords();
                MessageBox.Show("Trả sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
