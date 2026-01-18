using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LibraryManagement.Services
{
    public class CameraService : IDisposable
    {
        private FilterInfoCollection? _videoDevices;
        private VideoCaptureDevice? _videoSource;
        public event EventHandler<Bitmap>? FrameCaptured;

        public List<string> GetAvailableCameras()
        {
            try
            {
                _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (_videoDevices.Count == 0)
                {
                    return new List<string> { "Không tìm thấy camera" };
                }
                return _videoDevices.Cast<FilterInfo>().Select(f => f.Name).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting cameras: {ex.Message}");
                return new List<string> { "Lỗi khi tải camera" };
            }
        }

        public void StartCamera(int deviceIndex = 0)
        {
            try
            {
                if (_videoDevices == null || _videoDevices.Count == 0)
                {
                    throw new Exception("Không có camera nào được tìm thấy");
                }

                if (deviceIndex >= _videoDevices.Count)
                    deviceIndex = 0;

                _videoSource = new VideoCaptureDevice(_videoDevices[deviceIndex].MonikerString);
                _videoSource.NewFrame += VideoSource_NewFrame;
                _videoSource.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khởi động camera: {ex.Message}");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var bitmap = (Bitmap)eventArgs.Frame.Clone();
                FrameCaptured?.Invoke(this, bitmap);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error capturing frame: {ex.Message}");
            }
        }

        public void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
                _videoSource.NewFrame -= VideoSource_NewFrame;
            }
        }

        public void Dispose()
        {
            StopCamera();
            _videoSource = null;
        }
    }
}
