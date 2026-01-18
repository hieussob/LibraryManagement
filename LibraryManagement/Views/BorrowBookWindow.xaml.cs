using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class BorrowBookWindow : Window
    {
        public BorrowBookWindow(Book book)
        {
            InitializeComponent();
            DataContext = new BorrowBookViewModel(book);
        }

        public BorrowBookWindow(List<Book> books)
        {
            InitializeComponent();
            DataContext = new BorrowBookViewModel(books);
        }
    }
}
