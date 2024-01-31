using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using DIcon = System.Drawing.Icon;

namespace VolumeMixer.Classes
{
    public class Utils
    {
        public static DIcon GetIconFromFile(string _filePath)
        {
            if (string.IsNullOrEmpty(_filePath)) return null;
            return DIcon.ExtractAssociatedIcon(_filePath);
        }
        public static BitmapSource GetIconToBitmapImage(DIcon _icon)
        {
            if (_icon == null) return null;
            return Imaging.CreateBitmapSourceFromHBitmap(_icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
