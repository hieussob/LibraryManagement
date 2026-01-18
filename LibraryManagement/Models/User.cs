using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        [Key]
        private string id = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string passwordHash = string.Empty;

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string role = "User"; // "Admin" hoặc "User"

        [ObservableProperty]
        private DateTime createdDate = DateTime.Now;

        [ObservableProperty]
        private DateTime? lastLoginDate;

        [ObservableProperty]
        private bool isActive = true;

        public User()
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString();
        }
    }
}
