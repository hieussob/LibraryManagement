using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using ZXing;
using ZXing.Windows.Compatibility;

namespace LibraryManagement.ViewModels
{
    public partial class QRScannerViewModel : ObservableObject
    {
        private readonly CameraService _cameraService;
        private readonly BarcodeReader _barcodeReader;
        private int _frameCount = 0;
        private const int FRAME_SKIP = 5; // Giảm tần suất decode để tránh lag
        private bool _isProcessing = false; // Flag để tránh xử lý đồng thời nhiều frame

        [ObservableProperty]
        private List<string> availableCameras = new List<string>();

        [ObservableProperty]
        private string? selectedCamera;

        [ObservableProperty]
        private string scannedText = string.Empty;

        [ObservableProperty]
        private bool isScanning;

        public event EventHandler<Bitmap>? FrameReceived;
        public event EventHandler? QRCodeScanned; // Event khi quét được QR

        public QRScannerViewModel()
        {
            _cameraService = new CameraService();
            
            // Cấu hình BarcodeReader với options tối ưu
            _barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true
                    // Không giới hạn format - chấp nhận mọi loại mã
                }
            };
            
            LoadCameras();
            _cameraService.FrameCaptured += OnFrameCaptured;
        }

        private void LoadCameras()
        {
            try
            {
                AvailableCameras = _cameraService.GetAvailableCameras();
                if (AvailableCameras.Count > 0)
                    SelectedCamera = AvailableCameras[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách camera: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void StartScanning()
        {
            if (string.IsNullOrEmpty(SelectedCamera))
            {
                MessageBox.Show("Vui lòng chọn camera!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _frameCount = 0; // Reset counter
                var cameraIndex = AvailableCameras.IndexOf(SelectedCamera);
                _cameraService.StartCamera(cameraIndex);
                IsScanning = true;
                ScannedText = "Đang quét...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể khởi động camera: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void StopScanning()
        {
            try
            {
                _cameraService.StopCamera();
            }
            catch { }
            finally
            {
                IsScanning = false;
            }
        }

        private void OnFrameCaptured(object? sender, Bitmap bitmap)
        {
            try
            {
                // Gửi frame đến UI để hiển thị (mỗi frame để mượt)
                FrameReceived?.Invoke(this, bitmap);

                // Decode mỗi 5 frame
                _frameCount++;
                if (_frameCount % FRAME_SKIP != 0)
                    return;

                // Nếu đang xử lý frame trước thì skip
                if (_isProcessing)
                    return;

                _isProcessing = true;

                try
                {
                    System.Diagnostics.Debug.WriteLine($"\n>>> Frame #{_frameCount}");
                    
                    // QUAN TRỌNG: Clone bitmap NGAY trong lock và RESIZE nhỏ lại
                    Bitmap clonedFrame;
                    lock (bitmap)
                    {
                        // Resize về 640x480 để decode nhanh hơn (camera thường 1920x1080 → rất chậm)
                        int targetWidth = 640;
                        int targetHeight = (int)(bitmap.Height * (640.0 / bitmap.Width));
                        
                        clonedFrame = new Bitmap(targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        using (var g = System.Drawing.Graphics.FromImage(clonedFrame))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low; // Nhanh nhất
                            g.DrawImage(bitmap, 0, 0, targetWidth, targetHeight);
                        }
                    }

                    // Decode đồng bộ (không dùng Task.Run)
                    System.Diagnostics.Debug.WriteLine($"  Kích thước resize: {clonedFrame.Width}x{clonedFrame.Height}");
                    
                    var result = _barcodeReader.Decode(clonedFrame);
                    
                    if (result != null && !string.IsNullOrEmpty(result.Text))
                    {
                        System.Diagnostics.Debug.WriteLine($"✓✓✓ DECODE THÀNH CÔNG: {result.Text}\n");
                        
                        // Dùng BeginInvoke để không block
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            ScannedText = result.Text;
                            QRCodeScanned?.Invoke(this, EventArgs.Empty);
                            // Dừng camera sau khi trigger event
                            StopScanning();
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("  ✗ Chưa decode được");
                    }
                    
                    clonedFrame.Dispose();
                }
                finally
                {
                    _isProcessing = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! ERROR: {ex.Message}");
                _isProcessing = false;
            }
        }

        private Result? TryDecodeMultipleWays(Bitmap original)
        {
            // Chỉ dùng ảnh gốc - đơn giản nhất
            return _barcodeReader.Decode(original);
        }

        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            var grayscale = new Bitmap(original.Width, original.Height);
            using (var g = System.Drawing.Graphics.FromImage(grayscale))
            {
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original,
                    new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height,
                    System.Drawing.GraphicsUnit.Pixel, attributes);
            }
            return grayscale;
        }

        private Bitmap AdjustContrast(Bitmap original, float contrast)
        {
            var adjusted = new Bitmap(original.Width, original.Height);
            using (var g = System.Drawing.Graphics.FromImage(adjusted))
            {
                var t = (1.0f - contrast) / 2.0f;
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                        new float[] {contrast, 0, 0, 0, 0},
                        new float[] {0, contrast, 0, 0, 0},
                        new float[] {0, 0, contrast, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {t, t, t, 0, 1}
                    });

                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original,
                    new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height,
                    System.Drawing.GraphicsUnit.Pixel, attributes);
            }
            return adjusted;
        }

        public void Cleanup()
        {
            _cameraService.FrameCaptured -= OnFrameCaptured;
            _cameraService.Dispose();
        }
    }
}
