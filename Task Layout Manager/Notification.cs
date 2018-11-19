﻿using System;
using System.Linq;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Task_Layout_Manager
{
    internal class Notification
    {
        public static MessageOptions CreateOptions()
        {
            return new MessageOptions
            {
                FontSize = 12, // set notification font size
                ShowCloseButton = true, // set the option to show or hide notification close button
                Tag = "Any object or value which might matter in callbacks",
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                UnfreezeOnMouseLeave = true
            };
        }

        public static Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive),
                corner: Corner.BottomCenter,
                offsetX: 10,
                offsetY: -30);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(1));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public static void ShowNotification(string msg, string type)
        {
            switch (type)
            {
                case "i":
                    notifier.ShowInformation(msg, CreateOptions());

                    break;

                case "s":
                    notifier.ShowSuccess(msg, CreateOptions());

                    break;

                case "w":
                    notifier.ShowWarning(msg, CreateOptions());

                    break;

                case "e":
                    notifier.ShowError(msg, CreateOptions());

                    break;
            }
        }
    }
}
