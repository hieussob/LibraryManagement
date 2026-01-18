# 🔧 Hướng dẫn sử dụng sau khi sửa lỗi

## ✅ Đã sửa xong:

### 1. **Nút Lưu (Add) bây giờ hoạt động**
**Trước đây**: Nút bị disable, không bấm được
**Bây giờ**: 
- Nút sẽ tự động **enable khi bạn nhập đủ thông tin bắt buộc**
- Thông tin bắt buộc:
  - ✓ Tiêu đề sách
  - ✓ Tác giả  
  - ✓ ISBN
  - ✓ Số lượng > 0

**Cách test**:
1. Mở ứng dụng (đóng ứng dụng cũ trước)
2. Nhấn "➕ Thêm sách"
3. Ban đầu nút "💾 Lưu" sẽ bị disable (mờ)
4. Nhập lần lượt:
   - Tiêu đề: "Lập trình C#"
   - Tác giả: "Nguyễn Văn A"
   - ISBN: "978-0-12345-678-9"
   - Số lượng: "5"
5. **Nút Lưu sẽ tự động enable** → Bấm được!

---

### 2. **QR Code hiển thị rõ ràng**
**Trước đây**: Bấm "Tạo QR" không thấy mã QR xuất hiện
**Bây giờ**: 
- Mã QR hiển thị **ngay lập tức** ở cột bên phải
- Có border và padding đẹp hơn
- Khi chưa có QR: hiển thị text "Chưa có mã QR - Nhấn 'Tạo QR' để tạo"

**Cách test**:
1. Trong màn thêm sách, nhập ISBN: "978-0-12345-678-9"
2. Nhấn nút "Tạo QR"
3. **Mã QR xuất hiện ngay ở cột phải** (170x170 px)
4. Thông báo "Đã tạo mã QR thành công!"

---

## 📋 Cách chạy lại ứng dụng:

### Bước 1: Đóng ứng dụng cũ
Tìm và đóng cửa sổ LibraryManagement đang chạy

### Bước 2: Build lại
```bash
cd D:\Documents\WPF\LibraryManagement
dotnet build
```

### Bước 3: Chạy
```bash
dotnet run --project LibraryManagement
```

Hoặc nhấn F5 trong Visual Studio

---

## 🎯 Test kịch bản hoàn chỉnh:

### Test 1: Thêm sách với QR code
1. Mở ứng dụng
2. Nhấn "➕ Thêm sách"
3. Quan sát: Nút "💾 Lưu" đang disable (xám)
4. Nhập thông tin:
   ```
   Tiêu đề: Lập trình WPF
   Tác giả: Microsoft Press
   ISBN: 978-0-12345-678-9
   Nhà XB: Microsoft
   Năm XB: 2024
   Thể loại: Công nghệ
   Số lượng: 10
   ```
5. **Kiểm tra**: Nút "💾 Lưu" đã sáng lên → Có thể bấm
6. Nhấn "Tạo QR"
7. **Kiểm tra**: Mã QR xuất hiện bên phải ngay lập tức
8. Nhấn "💾 Lưu"
9. Sách được thêm vào danh sách

### Test 2: Kiểm tra validation
1. Mở "➕ Thêm sách"
2. Chỉ nhập Tiêu đề, để trống các trường khác
3. **Kiểm tra**: Nút "💾 Lưu" vẫn disable
4. Nhập thêm Tác giả
5. **Kiểm tra**: Nút "💾 Lưu" vẫn disable
6. Nhập thêm ISBN và Số lượng
7. **Kiểm tra**: Nút "💾 Lưu" enable → Có thể lưu

### Test 3: QR Code tự động
1. Thêm sách mới nhưng KHÔNG nhấn "Tạo QR"
2. Nhập đủ thông tin và nhấn "💾 Lưu"
3. **Kiểm tra**: Mã QR vẫn được tạo tự động khi lưu

---

## 🔍 Chi tiết kỹ thuật đã sửa:

### 1. AddEditBookViewModel.cs
```csharp
// Thêm NotifyCanExecuteChangedFor để tự động cập nhật trạng thái nút
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private Book book;

// Subscribe property changes để update SaveCommand
Book.PropertyChanged += (s, e) =>
{
    if (e.PropertyName == nameof(Book.Title) ||
        e.PropertyName == nameof(Book.Author) ||
        e.PropertyName == nameof(Book.Isbn) ||
        e.PropertyName == nameof(Book.Quantity))
    {
        SaveCommand.NotifyCanExecuteChanged();
    }
};
```

### 2. Converters.cs
```csharp
// Thêm StringToBoolConverter để check string không rỗng
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, ...)
    {
        return !string.IsNullOrEmpty(value as string);
    }
}
```

### 3. AddEditBookWindow.xaml
```xml
<!-- Hiển thị QR code với border đẹp -->
<Border Visibility="{Binding Book.QrCode, Converter={StaticResource StringToBoolConverter}}">
    <Image Source="{Binding Book.QrCode, Converter={StaticResource Base64ToImageConverter}}" 
           Width="170" Height="170"/>
</Border>

<!-- Placeholder khi chưa có QR -->
<Border Visibility="..." Background="#F9F9F9">
    <TextBlock Text="Chưa có mã QR&#x0a;Nhấn 'Tạo QR' để tạo"/>
</Border>
```

---

## ❓ Nếu vẫn gặp vấn đề:

1. **Nút Lưu vẫn không bấm được**:
   - Kiểm tra đã nhập đủ: Tiêu đề, Tác giả, ISBN, Số lượng
   - Số lượng phải > 0

2. **QR không hiển thị**:
   - Kiểm tra đã nhập ISBN chưa
   - Thử đóng và mở lại cửa sổ thêm sách
   - Kiểm tra ISBN không có ký tự đặc biệt

3. **Build lỗi "file is locked"**:
   - Đóng hoàn toàn ứng dụng LibraryManagement đang chạy
   - Chạy lại `dotnet build`

---

**Tóm lại**: 
- ✅ Nút Lưu: Tự động enable khi đủ thông tin
- ✅ QR Code: Hiển thị ngay khi nhấn "Tạo QR"
- ✅ Giao diện: Đẹp hơn với border và spacing hợp lý
