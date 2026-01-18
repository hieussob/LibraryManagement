using LibraryManagement.ViewModels;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LibraryManagement.Views
{
    public partial class QRScannerWindow : Window
    {
        private QRScannerViewModel? _viewModel;
        public string ScannedIsbn { get; private set; } = string.Empty;
        private int _uiFrameCount = 0;
        private const int UI_FRAME_SKIP = 2;
        private Bitmap? _lastFrame;

        public QRScannerWindow()
        {
            InitializeComponent();
            _viewModel = new QRScannerViewModel();
            DataContext = _viewModel;
            
            _viewModel.FrameReceived += ViewModel_FrameReceived;
            _viewModel.QRCodeScanned += ViewModel_QRCodeScanned;
        }

        private void ViewModel_QRCodeScanned(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (_viewModel != null && !string.IsNullOrEmpty(_viewModel.ScannedText))
                {
                    ScannedIsbn = _viewModel.ScannedText;
                    DialogResult = true;
                }
            });
        }

        private void ViewModel_FrameReceived(object? sender, Bitmap bitmap)
        {
            _uiFrameCount++;
            if (_uiFrameCount % UI_FRAME_SKIP != 0)
                return;

            _lastFrame?.Dispose();
            _lastFrame = new Bitmap(bitmap);

            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (CameraPreview.Source is BitmapImage oldImage)
                    {
                        CameraPreview.Source = null;
                        oldImage.StreamSource?.Dispose();
                    }

                    using var memory = new MemoryStream();
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    CameraPreview.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error displaying frame: {ex.Message}");
                }
            });
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && !string.IsNullOrEmpty(_viewModel.ScannedText))
            {
                ScannedIsbn = _viewModel.ScannedText;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Chưa quét được mã QR nào!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TestFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Chọn ảnh QR code",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"\n Đang test decode file: {openFileDialog.FileName}");
                    
                    using (var bitmap = new Bitmap(openFileDialog.FileName))
                    {
                        System.Diagnostics.Debug.WriteLine($" Kích thước: {bitmap.Width}x{bitmap.Height}");
                        
                        var reader = new ZXing.Windows.Compatibility.BarcodeReader
                        {
                            AutoRotate = true,
                            Options = new ZXing.Common.DecodingOptions
                            {
                                TryHarder = true,
                                TryInverted = true
                            }
                        };

                        var result = reader.Decode(bitmap);
                        
                        if (result != null && !string.IsNullOrEmpty(result.Text))
                        {
                            System.Diagnostics.Debug.WriteLine($" DECODE THÀNH CÔNG!");
                            System.Diagnostics.Debug.WriteLine($"Format: {result.BarcodeFormat}");
                            System.Diagnostics.Debug.WriteLine($"Nội dung: {result.Text}\n");
                            
                            if (_viewModel != null)
                            {
                                _viewModel.ScannedText = result.Text;
                            }
                            
                            MessageBox.Show($"Decode thành công!\n\nFormat: {result.BarcodeFormat}\nNội dung: {result.Text}", 
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(" KHÔNG DECODE ĐƯỢC\n");
                            MessageBox.Show("Không decode được QR code từ ảnh này.\n\nThử ảnh QR code rõ nét hơn.", 
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"!!! ERROR: {ex.Message}");
                    MessageBox.Show($"Lỗi đọc file: {ex.Message}", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CaptureFrameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastFrame == null)
            {
                MessageBox.Show("Chưa có frame nào từ camera!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var fileName = $"CapturedFrame_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                
                _lastFrame.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                
                System.Diagnostics.Debug.WriteLine($"\n Frame đã lưu: {filePath}");
                System.Diagnostics.Debug.WriteLine($" Kích thước: {_lastFrame.Width}x{_lastFrame.Height}");
                
                var reader = new ZXing.Windows.Compatibility.BarcodeReader
                {
                    AutoRotate = true,
                    Options = new ZXing.Common.DecodingOptions
                    {
                        TryHarder = true,
                        TryInverted = true
                    }
                };

                var result = reader.Decode(_lastFrame);
                
                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    System.Diagnostics.Debug.WriteLine($" DECODE THÀNH CÔNG từ captured frame!");
                    System.Diagnostics.Debug.WriteLine($" Nội dung: {result.Text}\n");
                    
                    MessageBox.Show($"Frame đã lưu tại:\n{filePath}\n\nDECODE THÀNH CÔNG!\nNội dung: {result.Text}", 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($" KHÔNG DECODE ĐƯỢC từ captured frame\n");
                    
                    MessageBox.Show($"Frame đã lưu tại:\n{filePath}\n\nNHƯNG KHÔNG DECODE ĐƯỢC QR!\n\nKiểm tra:\n- QR code có rõ nét không?\n- QR code có đủ lớn trong frame không?\n- Ánh sáng có đủ không?", 
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! ERROR capture: {ex.Message}");
                MessageBox.Show($"Lỗi capture frame: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _lastFrame?.Dispose();
            _viewModel?.Cleanup();
        }
    }
}
