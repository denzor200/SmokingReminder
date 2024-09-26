// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: http://www.hardcodet.net

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SmokingReminder.Systrayapp
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Enables notifications, if none is already enabled.
        /// </summary>
        public ICommand EnableNotifycationsCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Properties.Settings.Default.enabled == false,
                    CommandAction = () =>
                    {
                        Properties.Settings.Default.enabled = true;
                        Properties.Settings.Default.Save();
                    }
                };
            }
        }

        /// <summary>
        /// Disables notifications. This command is only enabled if notifications are enabled.
        /// </summary>
        public ICommand DisableNotificationsCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        Properties.Settings.Default.enabled = false;
                        Properties.Settings.Default.Save();
                    },
                    CanExecuteFunc = () => Properties.Settings.Default.enabled == true
                };
            }
        }

        public ICommand AboutCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        var about = new About();
                        about.ShowDialog();
                    }
                };
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () => Application.Current.Shutdown()};
            }
        }


        public string ToolTipText0
        {
            get {
                var enabled = Properties.Settings.Default.enabled;
                var interval = Properties.Settings.Default.interval;
                var elapsed = Math.Floor(((App)Application.Current).TimeLeft / 1000 / 60);
                return enabled ?
                    $"Перекур через {elapsed} мин." :
                    "Уведомления о перекурах выключены.";
            }
        }
    }
}
