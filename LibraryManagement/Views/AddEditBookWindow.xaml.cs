using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class AddEditBookWindow : Window
    {
        public AddEditBookWindow(Book? book = null)
        {
            InitializeComponent();
            DataContext = new AddEditBookViewModel(book);
        }
    }
}
