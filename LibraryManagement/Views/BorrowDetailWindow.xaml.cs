using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class BorrowDetailWindow : Window
    {
        public BorrowDetailWindow(BorrowRecord record)
        {
            InitializeComponent();
            DataContext = new BorrowDetailViewModel(record);
        }
    }
}
