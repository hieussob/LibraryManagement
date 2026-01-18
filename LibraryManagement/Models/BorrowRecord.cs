using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public partial class BorrowRecord : ObservableObject
    {
        [ObservableProperty]
        [Key]
        private string id = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string borrowerName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string borrowerPhone = string.Empty;

        [ObservableProperty]
        private string rank = string.Empty;

        [ObservableProperty]
        private string position = string.Empty;

        [ObservableProperty]
        private string unit = string.Empty;

        [ObservableProperty]
        private DateTime borrowDate = DateTime.Now;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Now.AddDays(14);

        [ObservableProperty]
        private DateTime? returnDate;

        [ObservableProperty]
        private string status = "Đang mượn"; // "Đang mượn", "Đã trả", "Quá hạn"

        [ObservableProperty]
        private string notes = string.Empty;

        public List<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

        [NotMapped]
        public bool IsValid => !string.IsNullOrWhiteSpace(BorrowerName) && 
                               !string.IsNullOrWhiteSpace(BorrowerPhone);

        public BorrowRecord()
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString();
        }
    }

    public partial class BorrowedBook : ObservableObject
    {
        [ObservableProperty]
        private string bookId = string.Empty;

        [ObservableProperty]
        private string bookTitle = string.Empty;

        [ObservableProperty]
        private string author = string.Empty;

        [ObservableProperty]
        private int quantity = 1;
    }
}
