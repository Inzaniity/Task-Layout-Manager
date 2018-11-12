﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Task_Layout_Manager
{
    public partial class MainWindow : MetroWindow
    {
        private List<TaskWindow> _tskwin;

        public MainWindow()
        {
            InitializeComponent();

            Style rowStyle = new Style(typeof(DataGridRow));

            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                new MouseButtonEventHandler(Row_DoubleClick)));
            DgvProcessGrid.RowStyle = rowStyle;
        }

        private static void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            // Some operations with this row
        }

        public struct MyCommands
        {
            public bool Check { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Position { get; set; }
            public string Size { get; set; }
            public ImageSource Icon { get; set; }
            public string WindowState { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FillDgv(ProcessManager.GetProcesses());
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FillDgv(ProcessManager.GetProcesses());
        }

        private void FillDgv(List<TaskWindow> taskWindows)
        {
            _tskwin = taskWindows;
            DgvProcessGrid.Items.Clear();
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

                string windowstate = null;

                switch (tw.ShowCmd)
                {
                    case 1:
                        windowstate = "Normal";
                        break;

                    case 2:
                        windowstate = "Minimized";
                        break;

                    case 3:
                        windowstate = "Maximized";
                        break;
                }

                DgvProcessGrid.Items.Add(new MyCommands
                {
                    Icon = image,
                    Name = tw.Name,
                    Path = tw.Path,
                    Position = tw.PosX + " : " + tw.PosY,
                    Size = tw.Height + " : " + tw.Width,
                    WindowState = windowstate
                });
            }

            DgvProcessGrid.Items.Refresh();
        }

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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (MyCommands row in DgvProcessGrid.Items)
            {
                Console.Write(row.Check + " | ");
                Console.Write(row.Name + " | ");
                Console.Write(row.Path + " | ");
                Console.Write(row.Position + " | ");
                Console.Write(row.Size + " | ");
                Console.Write(row.WindowState + " | ");

                Console.WriteLine();
            }

            //FileIo.SaveXml(_tskwin);
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory,
                Filter = "tlm files (*.tlm)|*.tlm|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FillDgv(FileIo.ReadXml(openFileDialog.FileName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgvProcessGrid.Items.Clear();
        }
    }
}