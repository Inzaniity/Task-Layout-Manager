using System.Drawing;
using System.Windows.Media;

namespace Task_Layout_Manager
{
    public class TaskWindow
    {
        public bool Check { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Flags { get; set; }
        public int ShowCmd { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageSource Icon { get; set; }

        public TaskWindow(bool check ,string name, string path, int flags, int showCmd, int posX, int posY, int height, int width, ImageSource icon)
        {
            Check = check;
            Name = name;
            Path = path;
            Flags = flags;
            ShowCmd = showCmd;
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            Icon = icon;
        }
    }
}
