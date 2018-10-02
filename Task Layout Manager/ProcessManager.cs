using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Task_Layout_Manager
{
    using System.Windows;
    using System.Windows.Shapes;

    internal class ProcessManager
    {
        private static readonly List<TaskWindow> TaskWindows = new List<TaskWindow>();

        public const int GclHiconsm = -34;
        public const int GclHicon = -14;
        public const int IconSmall = 0;
        public const int IconBig = 1;
        public const int IconSmall2 = 2;
        public const int WmGeticon = 0x7F;

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rect rectangle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        public struct Windowplacement
        {
            public int length;
            public int Flags;
            public int ShowCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static List<TaskWindow> GetProcesses()
        {
            TaskWindows.Clear();
            
            Process[] procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                IntPtr hWnd;
                Windowplacement placement = new Windowplacement();
                if ((hWnd = proc.MainWindowHandle) != IntPtr.Zero)
                {
                    GetWindowPlacement(proc.MainWindowHandle, ref placement);
                    GetWindowRect(hWnd, out var rct);

                    //ShowCmd = Windowstate
                    // 1 = Normal, 2 = Minimized, 3 = Maximized
                    if (placement.ShowCmd == 1 || placement.ShowCmd == 3)
                    {
                        int width = rct.Right - rct.Left + 1;
                        int height = rct.Bottom - rct.Top + 1;

                        if (width > 1 && height > 1)
                        {
                            TaskWindow taskWindow = new TaskWindow(proc.ProcessName, proc.MainModule.FileName,
                                placement.Flags, placement.ShowCmd, rct.Left, rct.Top, height, width, GetAppIcon(hWnd));
                            TaskWindows.Add(taskWindow);

                            //debugging
                            var s = Screen.FromHandle(hWnd).DeviceName;
                            //Console.WriteLine("{0} | {1}", proc.ProcessName, hWnd);
                            Console.WriteLine("X: {0} | Y: {1} | Screen: {2}", rct.Left, rct.Top, s);
                            Console.WriteLine("Height: {0} | Width: {1}", rct.Bottom - rct.Top + 1, rct.Right - rct.Left + 1);
                        }
                    }
                }
            }
            return TaskWindows;
        }

        public static Icon GetAppIcon(IntPtr hwnd)
        {
            IntPtr iconHandle = SendMessage(hwnd, WmGeticon, IconSmall2, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WmGeticon, IconSmall, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WmGeticon, IconBig, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GclHicon);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GclHiconsm);

            if (iconHandle == IntPtr.Zero)
                return null;

            Icon icn = Icon.FromHandle(iconHandle);

            return icn;
        }

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        public static void MovePorcessWindow(Process p)
        {
            MoveWindow(p.MainWindowHandle, 0, 0, 500, 500, true);
        }
    }
}