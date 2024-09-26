// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: https://github.com/denzor200/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmokingReminder.Systrayapp
{
    /// <summary>
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"Сведения {appName} {appVersion}";
            AppnameTextBlock.Text = $"{appName} {appVersion}";
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
