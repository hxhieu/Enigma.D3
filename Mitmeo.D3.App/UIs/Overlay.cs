using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mitmeo.D3.App.UIs
{
    public class Overlay : Window
    {
        private readonly Canvas _root;

        public Overlay()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            SnapsToDevicePixels = true;
            SizeToContent = SizeToContent.Manual;
            WindowState = WindowState.Maximized;
            Topmost = true;

            _root = new Canvas();
            Content = _root;
        }

        public void Add(UIElement element, Point point)
        {
            _root.Children.Add(element);
            Canvas.SetTop(element, point.X);
            Canvas.SetLeft(element, point.Y);
        }
    }
}
