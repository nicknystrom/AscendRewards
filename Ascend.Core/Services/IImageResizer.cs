using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    public interface IImageResizer
    {
        Image ResizeImage(
            Image sourceImg,
            System.Drawing.Size destinationSize,
            Stream ms,
            ImageFormat destinationFormat);
    }
}
