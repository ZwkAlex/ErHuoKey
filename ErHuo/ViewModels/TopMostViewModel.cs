using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Utilities;
using Stylet;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ErHuo.ViewModels
{
    public class TopMostViewModel : Screen
    {
        private CancellationTokenSource cts;

        public byte[] ImageBytes;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetAndNotify(ref _title, value);
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetAndNotify(ref _description, value);
            }
        }

        private bool _imageVisible = false;
        public bool ImageVisible
        {
            get { return _imageVisible; }
            set
            {
                _imageVisible = value;
                NotifyOfPropertyChange(nameof(ImageVisibility));
            }
        }

        public Visibility ImageVisibility
        {
            get { return _imageVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        private ImageSource _previewImage;

        public ImageSource PreviewImage
        {
            get { return _previewImage; }
            set
            {
                SetAndNotify(ref _previewImage, value);
            }
        }

        public TopMostViewModel(IContainer container)
        {

        }

        public void ChangeTitle(string title)
        {
            Title = title;
        }

        public bool IsRuning()
        {
            return cts != null;
        }

        public void ChangeDescription(string description)
        {
            Description = description;
        }

        private void PrepareWindow(string title, int timeout)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            CancelTask();
            ChangeTitle(title);
            cts = new CancellationTokenSource(timeout); 
        }

        public void ShowCursorLocationAndWindowTitle(string title, int timeout = 30000)
        {
            PrepareWindow(title, timeout);
            Thread t = new Thread(() =>
            {
                P _p = new P();
                try
                {
                    while (cts != null && !cts.Token.IsCancellationRequested)
                    {
                        CursorPoint cursor = CursorUtil.doGetCursorPos();
                        WindowInfo windowInfo = _p.GetMousePointWindow();
                        ChangeDescription("鼠标位置：" + cursor.ToString() + "\n当前窗口: " + windowInfo.szWindowName);
                        Thread.Sleep(100);
                    }
                }
                catch (ObjectDisposedException)
                {

                }
                finally
                {
                    _p.Dispose();
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void ShowCursorLocation(string title, int timeout = 30000, int xpadding = 50, int ypadding = 50)
        {
            PrepareWindow(title, timeout);
            Thread t = new Thread(() =>
            {
                P _p = new P();
                ImageVisible = true;
                string resourceBasePath = FileManager.CreateResourceBaseDir();
                string capturePath = Path.Combine(resourceBasePath, "capture.bmp");
                try
                {
                    while (cts != null && !cts.Token.IsCancellationRequested)
                    {
                        CursorPoint cursor = CursorUtil.doGetCursorPos();
                        ChangeDescription("鼠标位置：" + cursor.ToString());
                        ImageBytes = _p.CaptureToBytes(cursor.x - xpadding, cursor.y - ypadding, cursor.x + xpadding, cursor.y + ypadding);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PreviewImage = (BitmapSource)new ImageSourceConverter().ConvertFrom(ImageBytes);
                        });
                        Thread.Sleep(100);
                    }
                }
                catch (ObjectDisposedException)
                {

                }
                finally
                {
                    ImageVisible = false;
                    _p.Dispose();
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void UpdateImage(string path)
        {
            BitmapImage _image;
            _image = new BitmapImage();
            _image.BeginInit();
            _image.CacheOption = BitmapCacheOption.None;
            _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _image.UriSource = new Uri(path);
            _image.EndInit();
            _image.Freeze();
            PreviewImage = _image;
        }

        public void CancelTask()
        {
            if (cts != null)
            {
                if (cts.Token.CanBeCanceled)
                    cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }
    }
}
