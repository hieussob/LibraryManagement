# Hướng dẫn sử dụng nhanh - Library Management System

## Đã sửa các vấn đề:

### ✅ 1. Sửa lỗi tạo sách
- Đã kiểm tra và xác nhận chức năng tạo sách hoạt động
- Các trường bắt buộc (*): Tiêu đề, Tác giả, ISBN, Số lượng

### ✅ 2. Hiển thị mã QR khi tạo
- **Cửa sổ thêm/sửa sách đã được mở rộng** với cột bên phải hiển thị mã QR
- Khi bạn nhấn "Tạo QR", mã QR sẽ hiển thị ngay lập tức ở cột bên phải
- Mã QR được tự động tạo khi lưu sách (nếu chưa có)
- Mã QR có kích thước 180x180 pixels, dễ nhìn

### ✅ 3. Sửa lỗi quét mã QR
- Đã cải thiện xử lý lỗi khi không có camera
- Hiển thị thông báo rõ ràng nếu không tìm thấy camera
- Xử lý lỗi tốt hơn khi khởi động camera

## Cách sử dụng chi tiết:

### Tạo sách mới:
1. Nhấn nút "➕ Thêm sách"
2. Điền các thông tin bắt buộc:
   - Tiêu đề sách (*)
   - Tác giả (*)
   - ISBN (*) - ví dụ: 978-0-123456-78-9
   - Số lượng (*) - ví dụ: 5
3. Điền các thông tin tùy chọn (nhà xuất bản, năm XB, thể loại, mô tả)
4. **Nhấn "Tạo QR"** để tạo mã QR → Mã QR sẽ xuất hiện ngay bên phải
5. Nhấn "💾 Lưu" để lưu sách

**Lưu ý**: Nếu bạn không nhấn "Tạo QR" thủ công, mã QR sẽ tự động được tạo khi bạn nhấn Lưu.

### Quét mã QR:
1. Từ màn "Quản lý sách", nhấn "📷 Quét QR"
2. Chọn camera từ danh sách (nếu có nhiều camera)
3. Nhấn "▶ Bắt đầu quét"
4. Đưa mã QR vào trước camera
5. Hệ thống tự động tìm sách khi quét được mã

**Lưu ý về camera**:
- Cần có webcam để sử dụng tính năng này
- Nếu không có camera, thông báo "Không tìm thấy camera" sẽ hiển thị
- Nếu gặp lỗi "Lỗi khởi động camera", hãy:
  - Kiểm tra xem camera có đang được sử dụng bởi ứng dụng khác không
  - Thử khởi động lại ứng dụng
  - Kiểm tra quyền truy cập camera của Windows

### Mượn sách:
1. Từ màn "Quản lý sách", chọn sách cần mượn
2. Nhấn "📖 Mượn sách"
3. Nhập thông tin người mượn:
   - Họ tên (*)
   - Số điện thoại (*)
   - Email (tùy chọn)
4. Chọn ngày mượn và ngày hẹn trả
5. Nhấn "💾 Lưu phiếu mượn"

### Quản lý mượn sách:
1. Nhấn "📋 Quản lý mượn sách" trên menu
2. Lọc theo trạng thái: Đang mượn / Đã trả / Quá hạn
3. Tìm kiếm theo tên, SĐT, email
4. Nhấn "👁️ Xem chi tiết" để xem thông tin đầy đủ
5. Nhấn "✅ Trả sách" để hoàn tất trả sách

## Chạy ứng dụng:

```bash
cd LibraryManagement
dotnet run --project LibraryManagement
```

Hoặc mở file .sln bằng Visual Studio và nhấn F5.

## Các thay đổi kỹ thuật:

1. **Thêm Base64ToImageConverter**: Chuyển đổi chuỗi Base64 thành hình ảnh để hiển thị QR code
2. **Mở rộng AddEditBookWindow**: Thêm cột hiển thị QR code (750x700px)
3. **Cải thiện CameraService**: Xử lý lỗi tốt hơn khi không có camera
4. **Thêm OnPropertyChanged**: Cập nhật UI ngay khi tạo QR code

## Dữ liệu mẫu để test:

### Sách 1:
- Tiêu đề: Lập trình C# cơ bản
- Tác giả: Nguyễn Văn A
- ISBN: 978-0-123456-78-9
- Số lượng: 10

### Sách 2:
- Tiêu đề: WPF Application Development
- Tác giả: John Smith  
- ISBN: 978-1-234567-89-0
- Số lượng: 5

Sau khi tạo sách, bạn có thể in mã QR ra để test chức năng quét!
