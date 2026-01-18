# Hệ thống quản lý thư viện - Library Management System

Ứng dụng WPF quản lý sách và mượn trả sách với tính năng tạo và quét mã QR.

## Tính năng chính

### 1. Quản lý sách
- ✅ Thêm sách mới
- ✅ Sửa thông tin sách
- ✅ Xóa sách
- ✅ Tìm kiếm sách theo tiêu đề, tác giả, ISBN, thể loại
- ✅ Tự động tạo mã QR cho mỗi sách (dựa trên ISBN)
- ✅ Hiển thị số lượng sách và số lượng còn lại

### 2. Mượn sách
- ✅ Tạo phiếu mượn sách từ màn quản lý sách
- ✅ Nhập thông tin người mượn (họ tên, số điện thoại, email)
- ✅ Chọn ngày mượn và ngày hẹn trả
- ✅ Hỗ trợ mượn nhiều cuốn sách trong một phiếu
- ✅ Tự động cập nhật số lượng sách khả dụng

### 3. Quản lý mượn sách
- ✅ Xem danh sách tất cả phiếu mượn
- ✅ Sắp xếp theo ngày mượn (mới nhất trước)
- ✅ Lọc theo trạng thái (Đang mượn, Đã trả, Quá hạn)
- ✅ Tìm kiếm theo tên người mượn, số điện thoại, email
- ✅ Xem chi tiết từng phiếu mượn
- ✅ Trả sách (cập nhật trạng thái và số lượng sách)

### 4. Tính năng QR Code
- ✅ Tự động tạo mã QR khi thêm sách mới
- ✅ Tạo lại mã QR thủ công
- ✅ Quét mã QR bằng webcam để tìm sách nhanh

## Công nghệ sử dụng

- **Framework**: .NET 8.0 WPF
- **MVVM**: CommunityToolkit.Mvvm
- **QR Code**: QRCoder (tạo mã), ZXing.Net (đọc mã)
- **Camera**: AForge.NET
- **Lưu trữ**: JSON files (trong thư mục Data)

## Cấu trúc dự án

```
LibraryManagement/
├── Models/
│   ├── Book.cs                 # Model sách
│   └── BorrowRecord.cs         # Model phiếu mượn
├── Services/
│   ├── DataService.cs          # Quản lý dữ liệu
│   ├── QRCodeService.cs        # Tạo mã QR
│   └── CameraService.cs        # Xử lý camera
├── ViewModels/
│   ├── MainViewModel.cs        # ViewModel chính
│   ├── BookManagementViewModel.cs
│   ├── AddEditBookViewModel.cs
│   ├── BorrowBookViewModel.cs
│   ├── BorrowManagementViewModel.cs
│   ├── BorrowDetailViewModel.cs
│   └── QRScannerViewModel.cs
├── Views/
│   ├── BookManagementView.xaml
│   ├── AddEditBookWindow.xaml
│   ├── BorrowBookWindow.xaml
│   ├── BorrowManagementView.xaml
│   ├── BorrowDetailWindow.xaml
│   └── QRScannerWindow.xaml
└── Converters/
    └── Converters.cs           # Value converters
```

## Hướng dẫn sử dụng

### Quản lý sách

1. **Thêm sách mới**:
   - Click nút "➕ Thêm sách"
   - Điền thông tin: Tiêu đề (*), Tác giả (*), ISBN (*), v.v.
   - Click "Tạo QR" để tạo mã QR (hoặc sẽ tự động tạo khi lưu)
   - Click "💾 Lưu"

2. **Sửa sách**:
   - Chọn sách trong danh sách
   - Click "✏️ Sửa"
   - Chỉnh sửa thông tin
   - Click "💾 Lưu"

3. **Xóa sách**:
   - Chọn sách cần xóa
   - Click "🗑️ Xóa"
   - Xác nhận xóa

4. **Tìm kiếm**:
   - Gõ từ khóa vào ô tìm kiếm
   - Hệ thống tự động lọc theo tiêu đề, tác giả, ISBN, thể loại

5. **Quét QR**:
   - Click "📷 Quét QR"
   - Chọn camera
   - Click "▶ Bắt đầu quét"
   - Đưa mã QR vào camera
   - Hệ thống tự động tìm sách

### Mượn sách

1. **Tạo phiếu mượn**:
   - Từ màn Quản lý sách, chọn sách cần mượn
   - Click "📖 Mượn sách"
   - Nhập thông tin người mượn (*)
   - Chọn ngày mượn và ngày hẹn trả
   - Điều chỉnh số lượng nếu cần
   - Click "💾 Lưu phiếu mượn"

### Quản lý mượn sách

1. **Xem danh sách**:
   - Click "📋 Quản lý mượn sách" trên menu
   - Danh sách hiển thị theo ngày mượn (mới nhất trước)

2. **Lọc và tìm kiếm**:
   - Chọn trạng thái: Tất cả / Đang mượn / Đã trả / Quá hạn
   - Gõ tên, SĐT hoặc email để tìm kiếm

3. **Xem chi tiết và trả sách**:
   - Chọn phiếu mượn
   - Click "👁️ Xem chi tiết"
   - Xem thông tin đầy đủ
   - Click "✅ Trả sách" để hoàn tất

## Dữ liệu

Dữ liệu được lưu trong thư mục `Data/` (tự động tạo):
- `books.json`: Danh sách sách
- `borrow_records.json`: Danh sách phiếu mượn

## Lưu ý

- Các trường có dấu (*) là bắt buộc
- ISBN phải là duy nhất
- Không thể xóa sách đang được mượn
- Mã QR được tạo dựa trên ISBN
- Cần có webcam để sử dụng tính năng quét QR

## Build và Run

```bash
cd LibraryManagement
dotnet build
dotnet run --project LibraryManagement
```

Hoặc mở solution bằng Visual Studio và nhấn F5.

## Tác giả

Được xây dựng bằng WPF và .NET 8.0
