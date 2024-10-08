﻿// hardcodet.net NotifyIcon for WPF
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
                var ago = Math.Floor((DateTime.Now - lastSmoking).TotalMinutes);
                if (ago == 0) {
                    result.AppendLine($"Последний перекур был только что.");
                }
                else
                {
                    result.AppendLine($"Последний перекур был в {lastSmoking.TimeOfDay.ToString(@"hh\:mm")} ({ago} мин. назад).");
                }
                smokingText = "Следующий перекур\r\n";
            }
            elapsed = Math.Floor(elapsed);
            if (elapsed == 0)
            {
                result.Append($"{smokingText}меньше чем через минуту.");
            }
            else
            {
                result.Append($"{smokingText}через {elapsed} мин.");
            }
            return result.ToString();
        }

        public static FancyPopupDataContext GetInstance()
        {
            var enabled = Properties.Settings.Default.enabled;
            var interval = Properties.Settings.Default.interval;
            var isScheduledButtonEnabled = enabled;
            var elapsed = Math.Floor(((App)Application.Current).TimeLeft / 1000 / 60);
            var lastSmoking = ((App)Application.Current).LastSmoking;
            if (lastSmoking != DateTime.MinValue)
            {
                var ago = Math.Floor((DateTime.Now - lastSmoking).TotalMinutes);
                if (ago == 0)
                {
                    isScheduledButtonEnabled = false;
                }
            }
            return new FancyPopupDataContext()
            {
                Description = enabled ?
                    MakeDescription(elapsed) :
                    "Уведомления о перекурах\r\nвыключены...",
                SwitchButtonText = enabled ? "Выключить уведомления" : "Включить уведомления",
                IsScheduledButtonEnabled = isScheduledButtonEnabled
            };
        }
    };
    /// <summary>
    /// Interaction logic for FancyPopup.xaml
    /// </summary>
    public partial class FancyPopup : UserControl
    {
        private System.Timers.Timer timer;

        public FancyPopup()
        {
            InitializeComponent();
            DataContext = FancyPopupDataContext.GetInstance();
            timer = new Timer(1000 * 10);
            timer.Elapsed += OnTimer;
            timer.Enabled = true;
            timer.AutoReset = true;
            this.IsVisibleChanged += (object s, DependencyPropertyChangedEventArgs e) => {
                if ((bool)e.NewValue == true && (bool)e.OldValue == false)
                {
                    var interval = Properties.Settings.Default.interval;
                    IntervalShortUpDown.Value = interval;
                }
            };
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
            Properties.Settings.Default.old_interval = (short)(e.OldValue != null ? (short)e.OldValue : 0);
            Properties.Settings.Default.interval = (short)e.NewValue;
            Properties.Settings.Default.Save();
            DataContext = FancyPopupDataContext.GetInstance();
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
                    DataContext = FancyPopupDataContext.GetInstance();
                });
        }
    }
}