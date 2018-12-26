using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace Task_Layout_Manager
{
    public partial class MainWindow : MetroWindow
    {
        private List<TaskWindow> _tskwin;
        private readonly String[] args;

        public MainWindow()
        {
            InitializeComponent();
            args = App.mArgs;

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
            if (args != null)
            {
                FillDgv(FileIo.ReadXml(args[0]));
            }
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
                    Check = tw.Check,
                    Icon = tw.Icon,
                    Name = tw.Name,
                    Path = tw.Path,
                    Position = tw.PosX + " : " + tw.PosY,
                    Size = tw.Height + " : " + tw.Width,
                    WindowState = windowstate
                });
            }

            DgvProcessGrid.Items.Refresh();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Dgv_Selection();
            FileIo.SaveXml(_tskwin);
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

        private void Dgv_Selection()
        {
            _tskwin.Clear();

            foreach (MyCommands row in DgvProcessGrid.Items)
            {
                if (row.Check)
                {
                    int winState = 0, x, y, height, width;
                    switch (row.WindowState)
                    {
                        case "Normal":
                            winState = 1;
                            break;

                        case "Minimized":
                            winState = 2;
                            break;

                        case "Maximized":
                            winState = 3;
                            break;
                    }
                    string[] coords = row.Position.Split(':');
                    x = int.Parse(coords[0]);
                    y = int.Parse(coords[1]);

                    string[] size = row.Size.Split(':');
                    height = int.Parse(size[0]);
                    width = int.Parse(size[1]);

                    _tskwin.Add(new TaskWindow(true, row.Name, row.Path, 0, winState, x, y, height, width, row.Icon));
                }
            }
            FillDgv(_tskwin);
        }

        private void BtnRefreshSelection_Click(object sender, RoutedEventArgs e)
        {
            Dgv_Selection();
        }

        private async void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => ProcessManager.MovePorcessWindow(_tskwin));
        }
    }
}