using MahApps.Metro.Controls;
using System;
using System.Windows;


namespace Task_Layout_Manager
{
    /// <summary>
    /// Interaktionslogik für IgnoredProcessesWindow.xaml
    /// </summary>
    public partial class IgnoredProcessesWindow : MetroWindow
    {
        public static string[] ignoredProcesses;

        public IgnoredProcessesWindow()
        {
            InitializeComponent();
        }

        private void Btn_MoveToIgnore_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_AllProcs.SelectedItem != null && (string)ListView_AllProcs.SelectedItem != "")
            {
                ListView_IgnoredProcs.Items.Add(ListView_AllProcs.SelectedItem);
                ListView_AllProcs.Items.Remove(ListView_AllProcs.SelectedItem);
                SaveIgnoredProcs();
            }
        }

        private void Btn_RemoveFromIgnore_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_IgnoredProcs.SelectedItem != null && (string)ListView_IgnoredProcs.SelectedItem != "")
            {
                ListView_AllProcs.Items.Add(ListView_IgnoredProcs.SelectedItem);
                ListView_IgnoredProcs.Items.Remove(ListView_IgnoredProcs.SelectedItem);
                SaveIgnoredProcs();
            }
        }

        private void ListView_AllProcs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListView_AllProcs.SelectedItem != null && (string)ListView_AllProcs.SelectedItem != "")
            {
                ListView_IgnoredProcs.Items.Add(ListView_AllProcs.SelectedItem);
                ListView_AllProcs.Items.Remove(ListView_AllProcs.SelectedItem);
                SaveIgnoredProcs();
            }
        }

        private void ListView_IgnoredProcs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListView_IgnoredProcs.SelectedItem != null && (string)ListView_IgnoredProcs.SelectedItem != "")
            {
                ListView_AllProcs.Items.Add(ListView_IgnoredProcs.SelectedItem);
                ListView_IgnoredProcs.Items.Remove(ListView_IgnoredProcs.SelectedItem);
                SaveIgnoredProcs();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ignoredProcesses = Properties.Settings.Default.ignoredProcesses.Split(',');

            foreach (string s in ignoredProcesses)
            {
                ListView_IgnoredProcs.Items.Add(s);
            }


            foreach (TaskWindow tw in ProcessManager.GetProcesses(true))
            {
                if (Array.IndexOf(ignoredProcesses, tw.Name) == -1)
                {
                    ListView_AllProcs.Items.Add(tw.Name);
                }
            }
        }

        private void SaveIgnoredProcs()
        {
            string s = "";
            foreach (var temp in ListView_IgnoredProcs.Items)
            {
                if(temp != "")
                    s += temp + ",";
            }
            s = s.Remove(s.Length - 1);
            Properties.Settings.Default.ignoredProcesses = s;
            Properties.Settings.Default.Save();
        }
    }
}
