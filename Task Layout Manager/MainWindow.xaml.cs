using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Task_Layout_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public struct MyCommands
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Position { get; set; }
            public string Size { get; set; }
            public ImageSource Icon { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetProcesses();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetProcesses();
        }

        private void GetProcesses()
        {
            DgvProcessGrid.Items.Clear();
            List<TaskWindow> taskWindows = ProcessManager.GetProcesses();
            foreach (TaskWindow tw in taskWindows)
            {
                ImageSource image = null;
                if (tw.Icon != null)
                {
                    Bitmap bitmap = tw.Icon.ToBitmap();
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    image = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

                DgvProcessGrid.Items.Add(new MyCommands
                {
                    Icon = image,
                    Name = tw.Name,
                    Path = tw.Path,
                    Position = tw.PosX + " : " + tw.PosY,
                    Size = tw.Height + " : " + tw.Width
                });
            }
            DgvProcessGrid.Items.Refresh();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Process[] p = Process.GetProcessesByName("Task Layout Manager");

        //    foreach (var proc in p)
        //    {
        //        ProcessManager.MovePorcessWindow(proc);
        //    }
        //}


        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

    }
}
