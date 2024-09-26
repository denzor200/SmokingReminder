// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: http://www.hardcodet.net

using System;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace SmokingReminder.Systrayapp
{
    public class FancyPopupDataContext {
        public string Description { get; set; }
        public string SwitchButtonText { get; set; }
        public bool IsScheduledButtonEnabled { get; set; }

        private static string MakeDescription(double elapsed)
        {
            var result = new StringBuilder();
            var lastSmoking = ((App)Application.Current).LastSmoking;
            var smokingText = "Перекур ";
            if (lastSmoking != DateTime.MinValue)
            {
                var ago = DateTime.Now - lastSmoking;
                result.AppendLine($"Последний перекур был в {lastSmoking.TimeOfDay.ToString(@"hh\:mm")} ({Math.Floor(ago.TotalMinutes)} минут назад).");
                smokingText = "Следующий перекур\r\n";
            }
            result.Append($"{smokingText}через {elapsed} мин.");
            return result.ToString();
        }

        public static FancyPopupDataContext GetInstance()
        {
            var enabled = Properties.Settings.Default.enabled;
            var interval = Properties.Settings.Default.interval;
            var elapsed = Math.Floor(((App)Application.Current).TimeLeft / 1000 / 60);
            return new FancyPopupDataContext()
            {
                Description = enabled ?
                    MakeDescription(elapsed) :
                    "Уведомления о перекурах\r\nвыключены...",
                SwitchButtonText = enabled ? "Выключить уведомления" : "Включить уведомления",
                IsScheduledButtonEnabled = enabled
            };
        }
    };
    /// <summary>
    /// Interaction logic for FancyPopup.xaml
    /// </summary>
    public partial class FancyPopup : UserControl
    {
        private System.Timers.Timer timer;
        private short oldInterval;
        private short interval;
        private bool intervalChanged = false;

        public FancyPopup()
        {
            InitializeComponent();
            DataContext = FancyPopupDataContext.GetInstance();
            var interval = Properties.Settings.Default.interval;
            IntervalShortUpDown.Value = interval;
            timer = new Timer(1000 * 10);
            timer.Elapsed += OnTimer;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        private void OnSwitchClick(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.enabled)
            {
                Properties.Settings.Default.enabled = false;
            }
            else
            {
                Properties.Settings.Default.enabled = true;
            }
            Properties.Settings.Default.Save();
            DataContext = FancyPopupDataContext.GetInstance();
        }

        private void OnIntervalValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!intervalChanged)
                oldInterval = (short)(e.OldValue != null ? (short)e.OldValue : 0);
            interval = (short)e.NewValue;
            intervalChanged = true;
        }

        private void OnUnscheduledClick(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).DoUnsceduledSmoke();
            DataContext = FancyPopupDataContext.GetInstance();
        }

        private void OnTimer(object o, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    if (intervalChanged)
                    {
                        Properties.Settings.Default.old_interval = oldInterval;
                        Properties.Settings.Default.interval = interval;
                        Properties.Settings.Default.Save();
                        intervalChanged = false;
                    }
                    DataContext = FancyPopupDataContext.GetInstance();
                });
        }
    }
}