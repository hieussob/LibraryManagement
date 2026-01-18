using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public partial class Book : ObservableObject
    {
        [ObservableProperty]
        [Key]
        private string id = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string author = string.Empty;

        [ObservableProperty]
        private string publisher = string.Empty;

        [ObservableProperty]
        private int publishYear;

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private int availableQuantity;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string qrCode = string.Empty; // Base64 string của QR code image

        [ObservableProperty]
        private DateTime addedDate = DateTime.Now;

        [ObservableProperty]
        [NotMapped]
        private bool isSelected = false; // Để checkbox chọn nhiều sách

        public Book()
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString();
        }

        public Book Clone()
        {
            return new Book
            {
                Id = this.Id,
                Title = this.Title,
                Author = this.Author,
                Publisher = this.Publisher,
                PublishYear = this.PublishYear,
                Category = this.Category,
                Quantity = this.Quantity,
                AvailableQuantity = this.AvailableQuantity,
                Description = this.Description,
                QrCode = this.QrCode,
                AddedDate = this.AddedDate
            };
        }
    }
}
