// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: http://www.hardcodet.net

using System.Windows.Controls;

namespace SmokingReminder.Systrayapp
{

    public class WelcomeBalloonDataContext
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string Reminder { get; set; } 

        public static WelcomeBalloonDataContext GetInstance()
        {
            var enabled = Properties.Settings.Default.enabled;
            var interval = Properties.Settings.Default.interval;
            return new WelcomeBalloonDataContext() {
                Message = enabled ?
                    "Уведомления о перекурах включены" :
                    "Уведомления о перекурах выключены",
                Title = enabled ?
                    $"Интервал между перекурами {interval} мин." :
                    "",
                Reminder = enabled ?
                    "Вы можете выбрать другой интервал или выключить уведомления совсем нажав красную иконку пачки\r\nсигарет в трее." :
                    "Вы можете включить уведомления нажав красную иконку\r\nпачки сигарет в трее."
            };
        }
    }

    /// <summary>
    /// Interaction logic for WelcomeBalloon.xaml
    /// </summary>
    public partial class WelcomeBalloon : UserControl
    {
        public WelcomeBalloon()
        {
            InitializeComponent();
            DataContext = WelcomeBalloonDataContext.GetInstance();
        }

    }
}