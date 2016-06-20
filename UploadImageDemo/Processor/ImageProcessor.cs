using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace UploadImageDemo.Processor
{
    public class ImageProcessor
    {
        public Stream Watermark(Stream stream)
        {
            MemoryStream outStream = new MemoryStream();

            using (MagickImage water = new MagickImage(@"D:\PsWorkspace\savory - 2015\savory_128_64_water_1.png"))
            {
                using (MagickImage image = new MagickImage(stream))
                {
                    image.Composite(water, Gravity.Southeast, CompositeOperator.Atop);

                    image.Interlace = Interlace.Line;

                    image.Write(outStream);
                }
            }

            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        /// <summary>
        /// 使用指定尺寸裁切一张图片
        /// </summary>
        /// <param name="stream">原始图片数据流</param>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <returns>裁剪后的图片数据流</returns>
        public Stream ResizeImage(Stream stream, int width, int height)
        {
            return ResizeImage(stream, width, height, ResizePolicy.Auto);
        }

        /// <summary>
        /// 使用指定尺寸裁切一张图片
        /// 支持jpg png
        /// </summary>
        /// <param name="stream">原始图片数据流</param>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <param name="policy">
        /// <see cref="Ranta.WechatManager.Core.ResizePolicy"/>
        /// </param>
        /// <returns>裁剪后的图片数据流</returns>
        public Stream ResizeImage(Stream stream, int width, int height, ResizePolicy policy)
        {
            MemoryStream outStream = new MemoryStream();

            using (MagickImage originalImage = new MagickImage(stream))
            {
                using (MemoryStream jpgStream = new MemoryStream())
                {
                    originalImage.Write(jpgStream, MagickFormat.Jpg);

                    jpgStream.Seek(0, SeekOrigin.Begin);

                    using (MagickImage jpgImage = new MagickImage(jpgStream))
                    {
                        jpgImage.Interlace = Interlace.Line;

                        MagickGeometry size = Resize(jpgImage.Width, jpgImage.Height, width, height);

                        jpgImage.Crop(size.Width, size.Height, Gravity.Center);

                        var targetSize = new MagickGeometry(width, height);
                        targetSize.IgnoreAspectRatio = true;

                        switch (policy)
                        {
                            case ResizePolicy.Quick:
                                jpgImage.AdaptiveResize(targetSize);

                                break;
                            case ResizePolicy.HighQuality:
                                jpgImage.Resize(targetSize);

                                break;
                            case ResizePolicy.Auto:
                            default:
                                if ((Math.Abs((size.Width - width) / (float)size.Width) <= 0.5) &&
                                    (Math.Abs((size.Height - height) / (float)size.Height) <= 0.5))
                                {
                                    jpgImage.AdaptiveResize(targetSize);
                                }
                                else
                                {
                                    jpgImage.Resize(targetSize);
                                }

                                break;
                        }

                        jpgImage.Write(outStream);

                        outStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            return outStream;
        }

        /// <summary>
        /// 计算实际应该裁剪的尺寸
        /// 该方法忽略原始纵横比,根据目标长度和宽度的纵横比计算得到最佳的裁剪尺寸
        /// </summary>
        /// <param name="originalWidth">原始宽度</param>
        /// <param name="originalHeight">原始高度</param>
        /// <param name="expectWidth">期待宽度</param>
        /// <param name="expectHeight">期待高度</param>
        /// <returns>最佳裁剪尺寸</returns>
        private MagickGeometry Resize(int originalWidth, int originalHeight, int expectWidth, int expectHeight)
        {
            MagickGeometry actualSize = new MagickGeometry();

            int targetHeight = originalWidth * expectHeight / expectWidth;

            int targetWidth = originalHeight * expectWidth / expectHeight;

            if (targetWidth == expectWidth)
            {
                actualSize.Width = targetWidth;
                actualSize.Height = targetHeight;
            }
            else if (targetWidth < originalWidth)
            {
                actualSize.Width = targetWidth;
                actualSize.Height = originalHeight;
            }
            else
            {
                actualSize.Width = originalWidth;
                actualSize.Height = targetHeight;
            }

            return actualSize;
        }
    }
}