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
    public partial class BookManagementViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private ObservableCollection<Book> books;

        [ObservableProperty]
        private ObservableCollection<Book> filteredBooks;

        [ObservableProperty]
        private Book? selectedBook;

        [ObservableProperty]
        private ObservableCollection<Book> selectedBooks = new ObservableCollection<Book>();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string filterTitle = string.Empty;

        [ObservableProperty]
        private string filterAuthor = string.Empty;

        [ObservableProperty]
        private string filterPublisher = string.Empty;

        [ObservableProperty]
        private string filterYear = string.Empty;

        [ObservableProperty]
        private string filterCategory = string.Empty;

        [ObservableProperty]
        private string filterQuantity = string.Empty;

        [ObservableProperty]
        private string filterAvailable = string.Empty;

        public BookManagementViewModel()
        {
            _dataService = DataService.Instance;
            books = _dataService.Books;
            filteredBooks = new ObservableCollection<Book>(books);
        }

        partial void OnFilterTitleChanged(string value) => FilterBooks();
        partial void OnFilterAuthorChanged(string value) => FilterBooks();
        partial void OnFilterPublisherChanged(string value) => FilterBooks();
        partial void OnFilterYearChanged(string value) => FilterBooks();
        partial void OnFilterCategoryChanged(string value) => FilterBooks();
        partial void OnFilterQuantityChanged(string value) => FilterBooks();
        partial void OnFilterAvailableChanged(string value) => FilterBooks();

        partial void OnSearchTextChanged(string value)
        {
            FilterBooks();
        }

        private void FilterBooks()
        {
            var filtered = Books.AsEnumerable();

            // Global search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(b =>
                    b.Title.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                    b.Category.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                );
            }

            // Column-specific filters
            if (!string.IsNullOrWhiteSpace(FilterTitle))
                filtered = filtered.Where(b => b.Title.Contains(FilterTitle, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterAuthor))
                filtered = filtered.Where(b => b.Author.Contains(FilterAuthor, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterPublisher))
                filtered = filtered.Where(b => b.Publisher.Contains(FilterPublisher, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterYear))
                filtered = filtered.Where(b => b.PublishYear.ToString().Contains(FilterYear));

            if (!string.IsNullOrWhiteSpace(FilterCategory))
                filtered = filtered.Where(b => b.Category.Contains(FilterCategory, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterQuantity))
                filtered = filtered.Where(b => b.Quantity.ToString().Contains(FilterQuantity));

            if (!string.IsNullOrWhiteSpace(FilterAvailable))
                filtered = filtered.Where(b => b.AvailableQuantity.ToString().Contains(FilterAvailable));

            FilteredBooks = new ObservableCollection<Book>(filtered.ToList());
        }

        [RelayCommand]
        private void AddBook()
        {
            var window = new AddEditBookWindow();
            if (window.ShowDialog() == true)
            {
                FilterBooks();
            }
        }

        [RelayCommand]
        private void EditBook()
        {
            var selectedBook = FilteredBooks.FirstOrDefault(b => b.IsSelected);
            if (selectedBook == null)
            {
                MessageBox.Show("Vui lòng chọn một sách cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra nếu có nhiều hơn 1 sách được chọn
            if (FilteredBooks.Count(b => b.IsSelected) > 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn một sách để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new AddEditBookWindow(selectedBook);
            if (window.ShowDialog() == true)
            {
                selectedBook.IsSelected = false;
                FilterBooks();
            }
        }

        [RelayCommand]
        private void DeleteBook()
        {
            var selectedBooks = FilteredBooks.Where(b => b.IsSelected).ToList();
            if (selectedBooks.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var bookNames = string.Join(", ", selectedBooks.Select(b => b.Title));
            var result = MessageBox.Show($"Bạn có chắc muốn xóa {selectedBooks.Count} sách:\n{bookNames}?", 
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var book in selectedBooks)
                {
                    _dataService.DeleteBook(book.Id);
                }
                FilterBooks();
            }
        }

        [RelayCommand]
        private void BorrowBook()
        {
            // Lấy danh sách sách được chọn qua checkbox
            var selectedBooksList = FilteredBooks.Where(b => b.IsSelected).ToList();
            
            // Nếu không có sách nào được chọn
            if (selectedBooksList.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sách để mượn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra xem có sách nào hết không
            var unavailableBooks = selectedBooksList.Where(b => b.AvailableQuantity <= 0).ToList();
            if (unavailableBooks.Any())
            {
                var bookNames = string.Join(", ", unavailableBooks.Select(b => b.Title));
                MessageBox.Show($"Những sách sau hiện không còn để mượn:\n{bookNames}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mở window với danh sách sách đã chọn (có thể 1 hoặc nhiều sách)
            var window = selectedBooksList.Count == 1 
                ? new BorrowBookWindow(selectedBooksList[0])
                : new BorrowBookWindow(selectedBooksList);
                
            if (window.ShowDialog() == true)
            {
                // Bỏ chọn tất cả sách sau khi mượn thành công
                foreach (var book in selectedBooksList)
                {
                    book.IsSelected = false;
                }
                FilterBooks();
            }
        }

        [RelayCommand]
        private void ScanQR()
        {
            var window = new QRScannerWindow();
            if (window.ShowDialog() == true && !string.IsNullOrEmpty(window.ScannedIsbn))
            {
                // ScannedIsbn chứa UUID (Id) của sách
                var book = _dataService.GetBookById(window.ScannedIsbn);
                if (book != null)
                {
                    // Cập nhật SearchText để tìm sách vừa quét
                    SearchText = book.Title; // Tìm theo tiêu đề sẽ rõ ràng hơn UUID
                    FilterBooks();
                    
                    MessageBox.Show($"Đã tìm thấy sách: {book.Title}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sách với mã này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            FilterBooks();
        }

        [RelayCommand]
        private void EditBookRow(Book book)
        {
            if (book == null) return;

            var window = new AddEditBookWindow(book);
            if (window.ShowDialog() == true)
            {
                FilterBooks();
            }
        }

        [RelayCommand]
        private void DeleteBookRow(Book book)
        {
            if (book == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sách:\n{book.Title}?", 
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _dataService.DeleteBook(book.Id);
                FilterBooks();
            }
        }

        [RelayCommand]
        private void BorrowBookRow(Book book)
        {
            if (book == null) return;

            // Kiểm tra xem sách còn không
            if (book.AvailableQuantity <= 0)
            {
                MessageBox.Show($"Sách '{book.Title}' hiện không còn để mượn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new BorrowBookWindow(book);
            if (window.ShowDialog() == true)
            {
                FilterBooks();
            }
        }
    }
}
