using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public User? CurrentUser { get; private set; }

        [RelayCommand]
        private void Login(Window window)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                return;
            }

            try
            {
                using (var db = new LibraryDbContext())
                {
                    var passwordHash = HashPassword(Password);
                    var user = db.Users.FirstOrDefault(u => 
                        u.Username == Username && 
                        u.PasswordHash == passwordHash &&
                        u.IsActive);

                    if (user != null)
                    {
                        CurrentUser = user;
                        user.LastLoginDate = DateTime.Now;
                        db.Users.Update(user);
                        db.SaveChanges();

                        System.Diagnostics.Debug.WriteLine("Login successful, setting DialogResult = true");
                        window.DialogResult = true;
                        System.Diagnostics.Debug.WriteLine("Closing window");
                        window.Close();
                    }
                    else
                    {
                        ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
