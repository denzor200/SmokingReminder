// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: http://www.hardcodet.net

using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls.Primitives;
using System.Timers;
using System;
using Hardcodet.Wpf.TaskbarNotification.Interop;


namespace SmokingReminder.Systrayapp
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private ExtendedTimer timer;
        private static double PopupHorizontalOffset = -20;

        public double TimeLeft { get { return timer != null ? timer.TimeLeft : 0; } }
        public DateTime LastSmoking { get; private set; } = DateTime.MinValue;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon) FindResource("NotifyIcon");
            
            notifyIcon.TrayMouseMove += (o, s) =>
            {
                notifyIcon.DataContext = new NotifyIconViewModel();
            };
            notifyIcon.TrayPopupOpen += (o, s) => {
                PlacePopupAtBottom();
            };


            //show balloon at startup
            var balloon = new WelcomeBalloon();
            notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 12000);

            var enabled = SmokingReminder.Systrayapp.Properties.Settings.Default.enabled;
            var interval = SmokingReminder.Systrayapp.Properties.Settings.Default.interval;
            timer = new ExtendedTimer((int)interval * 1000 * 60);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = enabled;

            SmokingReminder.Systrayapp.Properties.Settings.Default.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "enabled")
                    {
                        var enabled = SmokingReminder.Systrayapp.Properties.Settings.Default.enabled;
                        timer.Enabled = enabled;
                    }
                    else if (e.PropertyName == "interval")
                    {
                        var interval = SmokingReminder.Systrayapp.Properties.Settings.Default.interval;
                        var oldInterval = SmokingReminder.Systrayapp.Properties.Settings.Default.old_interval;
                        var deltaInterval = interval - oldInterval;
                        var timeLeft = (int)TimeLeft;
                        if (oldInterval > 0)
                        {
                            var newInterval = timeLeft + deltaInterval * 1000 * 60;
                            while (newInterval <= 0)
                                newInterval += interval * 1000 * 60;
                            timer.Stop();
                            timer.Dispose();
                            timer = new ExtendedTimer(newInterval);
                            timer.Elapsed += OnTimedEventLeft;
                            timer.Enabled = enabled;
                        }
                    }
                };

        }

        public void PlacePopupAtBottom()
        {
            notifyIcon.TrayPopupResolved.Placement = PlacementMode.Bottom;

            // place popup above system taskbar
            var point = ExtendedTrayInfo.GetTrayLocation(0);
            notifyIcon.TrayPopupResolved.Placement = PlacementMode.AbsolutePoint;
            notifyIcon.TrayPopupResolved.HorizontalOffset = point.X;
            notifyIcon.TrayPopupResolved.VerticalOffset = point.Y;

            notifyIcon.TrayPopupResolved.HorizontalOffset += PopupHorizontalOffset;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            LastSmoking = DateTime.Now;
            DoNotification();
        }

        private void OnTimedEventLeft(Object source, ElapsedEventArgs e)
        {
            LastSmoking = DateTime.Now;
            DoNotification();
            DoTimerRefresh();
        }

        public void DoUnsceduledSmoke()
        {
            LastSmoking = DateTime.Now;
            DoTimerRefresh();
        }

        private void DoNotification()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var balloon = new FancyBalloon();
                    notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, (int)balloon.TotalVisibleTime);
                });
        }

        private void DoTimerRefresh()
        {
            var enabled = SmokingReminder.Systrayapp.Properties.Settings.Default.enabled;
            var interval = SmokingReminder.Systrayapp.Properties.Settings.Default.interval;
            timer.Stop();
            timer.Dispose();
            timer = new ExtendedTimer((int)interval * 1000 * 60);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = enabled;
        }
    }
}
