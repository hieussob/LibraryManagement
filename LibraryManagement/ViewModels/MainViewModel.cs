using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LibraryManagement.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        [ObservableProperty]
        private string pageTitle = "Quản lý thư viện";

        public MainViewModel()
        {
            ShowBookManagement();
        }

        [RelayCommand]
        private void ShowBookManagement()
        {
            CurrentView = new BookManagementViewModel();
            PageTitle = "Quản lý sách";
        }

        [RelayCommand]
        private void ShowBorrowManagement()
        {
            CurrentView = new BorrowManagementViewModel();
            PageTitle = "Quản lý mượn sách";
        }
    }
}
