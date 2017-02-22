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
    [ValueConversion(typeof(byte[]), typeof(BitmapImage))]
    public class TextureToBitmapImageConverter : MarkupExtension,
                                                 IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var texture = value as byte[];
            if ((texture == null) || texture.IsNullOrEmpty())
            {
                return null;
            }

            try
            {
                using (var inputStream = new MemoryStream(texture))
                using (var image = Image.FromStream(inputStream))
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, ImageFormat.Png);
                    outputStream.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = outputStream;
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
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
    }
}