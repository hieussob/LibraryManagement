using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using LibraryManagement.Views;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace LibraryManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set shutdown mode to explicit shutdown
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Initialize database and create default admin user
            InitializeDatabase();

            // Show login window first
            var loginWindow = new LoginWindow();
            System.Diagnostics.Debug.WriteLine("Showing login window...");
            var loginResult = loginWindow.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"Login result: {loginResult}");
            
            if (loginResult == true)
            {
                System.Diagnostics.Debug.WriteLine("Creating MainWindow...");
                // Login successful, show main window
                var mainWindow = new MainWindow();
                System.Diagnostics.Debug.WriteLine("Showing MainWindow...");
                mainWindow.Show();
                System.Diagnostics.Debug.WriteLine("MainWindow shown");
                
                // Change shutdown mode to close when main window closes
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                MainWindow = mainWindow;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Login cancelled or failed, shutting down");
                // Login cancelled or failed, exit application
                Shutdown();
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var db = new LibraryDbContext())
                {
                    db.Database.EnsureCreated();

                    // Create default admin user if no users exist
                    if (!db.Users.Any())
                    {
                        var adminUser = new User
                        {
                            Username = "admin",
                            PasswordHash = LoginViewModel.HashPassword("admin123"),
                            FullName = "Quản trị viên",
                            Role = "Admin",
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        };
                        db.Users.Add(adminUser);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo cơ sở dữ liệu: {ex.Message}", 
                              "Lỗi", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }
    }
}

