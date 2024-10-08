﻿// Some interop code taken from Mike Marshall's AnyForm

using System.Drawing;

namespace SmokingReminder.Systrayapp
{
    /// <summary>
    /// Resolves the current tray position.
    /// </summary>
    public static class ExtendedTrayInfo
    {
        /// <summary>
        /// Gets the position of the system tray.
        /// </summary>
        /// <returns>Tray coordinates.</returns>
        public static Hardcodet.Wpf.TaskbarNotification.Interop.Point GetTrayLocation(int space = 2)
        {
            var info = new Hardcodet.Wpf.TaskbarNotification.Interop.AppBarInfo();
            info.GetSystemTaskBarPosition();

            Rectangle rcWorkArea = info.WorkArea;

            int x = 0, y = 0;
            switch (info.Edge)
            {
                case Hardcodet.Wpf.TaskbarNotification.Interop.AppBarInfo.ScreenEdge.Left:
                    x = rcWorkArea.Right + space;
                    y = rcWorkArea.Bottom;
                    break;
                case Hardcodet.Wpf.TaskbarNotification.Interop.AppBarInfo.ScreenEdge.Bottom:
                    x = rcWorkArea.Right;
                    y = rcWorkArea.Bottom - rcWorkArea.Height - space;
                    break;
                case Hardcodet.Wpf.TaskbarNotification.Interop.AppBarInfo.ScreenEdge.Top:
                    x = rcWorkArea.Right;
                    y = rcWorkArea.Top + rcWorkArea.Height + space;
                    break;
                case Hardcodet.Wpf.TaskbarNotification.Interop.AppBarInfo.ScreenEdge.Right:
                    x = rcWorkArea.Right - rcWorkArea.Width - space;
                    y = rcWorkArea.Bottom;
                    break;
            }

            return GetDeviceCoordinates(new Hardcodet.Wpf.TaskbarNotification.Interop.Point { X = x, Y = y });
        }

        /// <summary>
        /// Recalculates OS coordinates in order to support WPFs coordinate
        /// system if OS scaling (DPIs) is not 100%.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static Hardcodet.Wpf.TaskbarNotification.Interop.Point GetDeviceCoordinates(Hardcodet.Wpf.TaskbarNotification.Interop.Point point)
            => Hardcodet.Wpf.TaskbarNotification.Interop.SystemInfo.ScaleWithDpi(point);
    }
}
