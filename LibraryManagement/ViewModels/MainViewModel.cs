using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LibraryManagement.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        [ObservableProperty]
        private string pageTitle = "Quản lý tài liệu của lữ đoàn 215";

        public MainViewModel()
        {
            ShowBookManagement();
        }

        [RelayCommand]
        private void ShowBookManagement()
        {
            CurrentView = new BookManagementViewModel();
            PageTitle = "Quản lý tài liệu của lữ đoàn 215";
        }

        [RelayCommand]
        private void ShowBorrowManagement()
        {
            CurrentView = new BorrowManagementViewModel();
            PageTitle = "Quản lý mượn sách";
        }
    }
}
