using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

using iGP11.Library;
using iGP11.Tool.Bootstrapper;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(Icon), typeof(BitmapImage))]
    public class IconToBitmapImageConverter : MarkupExtension,
                                              IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var icon = value as Icon;
                if (icon != null)
                {
                    return GetBitmapImage(icon);
                }

                var bitmap = value as Bitmap;
                return bitmap != null
                           ? GetBitmapImage(bitmap)
                           : null;
            }
            catch (Exception exception)
            {
                Logger.Current.Log(LogLevel.Error, $"bitmap image creation failed; exception: {exception}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static BitmapImage GetBitmapImage(Icon icon)
        {
            using (var bitmap = icon.ToBitmap())
            {
                return GetBitmapImage(bitmap);
            }
        }

        private static BitmapImage GetBitmapImage(Image bitmap)
        {
            using (var outputStream = new MemoryStream())
            {
                bitmap.Save(outputStream, ImageFormat.Png);
                outputStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = outputStream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}