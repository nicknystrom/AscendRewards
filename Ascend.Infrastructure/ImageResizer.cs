using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Ascend.Core.Services;

namespace Ascend.Infrastructure
{
    public class ImageResizer : IImageResizer
    {
        public Image ResizeImage(
            Image sourceImg,
            Size destinationSize,
            Stream ms,
            ImageFormat destinationFormat)
        {
            var sourceWidth = sourceImg.Width;
            var sourceHeight = sourceImg.Height;
            var destX = 0;
            var destY = 0;

            float nPercent;

            var nPercentW = (destinationSize.Width / (float)sourceWidth);
            var nPercentH = (destinationSize.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((destinationSize.Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((destinationSize.Height -
                              (sourceHeight * nPercent)) / 2);
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var result = new Bitmap(destinationSize.Width, destinationSize.Height,
                              PixelFormat.Format24bppRgb);
            try
            {
                result.SetResolution(sourceImg.HorizontalResolution,
                                    sourceImg.VerticalResolution);
            }
            catch
            {
                // seems to fail often on Mono
            }

            var g = Graphics.FromImage(result);
            g.Clear(Color.White);
            g.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            g.DrawImage(sourceImg,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(0, 0, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            g.Dispose();

            if (null != ms)
            {
                if (destinationFormat == ImageFormat.Jpeg)
                {
                    SaveJpeg(result, 90, ms);
                }
                else
                {
                    result.Save(ms, destinationFormat);
                }
            }

            return result;
        }

        public static void SaveJpeg(Image source, int quality, Stream destination)
        {
            var jpeg = ImageCodecInfo.GetImageEncoders().Single(x => x.FormatDescription == "JPEG");

            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            source.Save(destination, jpeg, encoderParameters);
        }
    }
}
