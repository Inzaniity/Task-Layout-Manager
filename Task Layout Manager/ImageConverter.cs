using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Task_Layout_Manager
{
    internal class ImageConverter
    {
        public static ImageSource IconToImagesource(Icon icn)
        {
            ImageSource image = null;
            try
            {
                if (icn != null)
                {
                    Bitmap bitmap = icn.ToBitmap();
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    image = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return image;
        }
    }
}