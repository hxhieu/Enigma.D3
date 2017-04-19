﻿using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mitmeo.D3.App.UIs
{
    public class AlertSettings
    {
        public double UpdateInterval { get; set; } = 1000d;
        public int Size { get; set; } = 40;
        public Brush Colour { get; set; } = Brushes.White;
        public Rect Position { get; set; }
    }

    public class Alert : Label
    {
        private DispatcherTimer _timer;
        private readonly AlertSettings _settings;

        public Alert(object content, Func<object> update = null, AlertSettings settings = null)
        {
            _settings = settings ?? new AlertSettings();

            FontSize = _settings.Size;
            Foreground = _settings.Colour;
            Content = content;

            SetUpdate(update);
        }

        public void SetUpdate(Func<object> update)
        {
            if (update == null) return;

            if (_timer != null)
            {
                _timer.Stop();
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(_settings.UpdateInterval);
            _timer.Tick += (s, e) =>
            {
                Content = update();
            };

            _timer.Start();
        }
    }
}
