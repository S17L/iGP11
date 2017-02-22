using System.Drawing;

using iGP11.Tool.Properties;

namespace iGP11.Tool.Model
{
    public class EmptyImageProvider : IImageProvider
    {
        public byte[] Get()
        {
            return (byte[])new ImageConverter().ConvertTo(Resources.no_image, typeof(byte[]));
        }
    }
}