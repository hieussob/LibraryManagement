using LibraryManagement.ViewModels;
using System.Windows;

namespace LibraryManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}