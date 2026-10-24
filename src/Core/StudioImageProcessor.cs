using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace PS2toPS4Builder.Core
{
    public static class StudioImageProcessor
    {
        public static string ExtractSerialFromText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            Match m = Regex.Match(text.ToUpper(), @"([A-Z]{4})[_\-\s]?(\d{3})[\.\-_]?(\d{2})");
            if (m.Success)
            {
                string letters = m.Groups[1].Value;
                string part1 = m.Groups[2].Value;
                string part2 = m.Groups[3].Value;
                return string.Format("{0}_{1}.{2}", letters, part1, part2);
            }

            Match m2 = Regex.Match(text.ToUpper(), @"([A-Z]{4})[_\-\s]?(\d{5})");
            if (m2.Success)
            {
                string letters = m2.Groups[1].Value;
                string digits = m2.Groups[2].Value;
                return string.Format("{0}_{1}.{2}", letters, digits.Substring(0, 3), digits.Substring(3, 2));
            }

            return "";
        }

        public static Bitmap CreateSquareIcon(Image src, int size = 512, bool useAmbientBackground = true)
        {
            Bitmap canvas = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float ratio = (float)src.Width / src.Height;

                if (Math.Abs(ratio - 1.0f) < 0.05f)
                {
                    g.DrawImage(src, new Rectangle(0, 0, size, size));
                }
                else if (useAmbientBackground)
                {
                    using (Bitmap blurred = ResizeBitmap(src, 64, 64))
                    {
                        g.DrawImage(blurred, new Rectangle(0, 0, size, size));
                    }
                    using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(160, 10, 10, 15)))
                    {
                        g.FillRectangle(darkBrush, 0, 0, size, size);
                    }

                    int targetW, targetH;
                    if (ratio > 1.0f)
                    {
                        targetW = (int)(size * 0.94f);
                        targetH = (int)(targetW / ratio);
                    }
                    else
                    {
                        targetH = (int)(size * 0.94f);
                        targetW = (int)(targetH * ratio);
                    }

                    int posX = (size - targetW) / 2;
                    int posY = (size - targetH) / 2;

                    using (SolidBrush shadow = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
                    {
                        g.FillRectangle(shadow, posX - 4, posY - 2, targetW + 8, targetH + 8);
                    }

                    g.DrawImage(src, new Rectangle(posX, posY, targetW, targetH));
                }
                else
                {
                    int targetW, targetH;
                    if (ratio > 1.0f)
                    {
                        targetW = size;
                        targetH = (int)(size / ratio);
                    }
                    else
                    {
                        targetH = size;
                        targetW = (int)(size * ratio);
                    }
                    int posX = (size - targetW) / 2;
                    int posY = (size - targetH) / 2;
                    g.Clear(Color.FromArgb(20, 20, 20));
                    g.DrawImage(src, new Rectangle(posX, posY, targetW, targetH));
                }
            }
            return canvas;
        }

        public static Bitmap CreateWidescreenBackground(Image src, int width = 1920, int height = 1080)
        {
            Bitmap canvas = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float targetRatio = (float)width / height;
                float srcRatio = (float)src.Width / src.Height;

                if (Math.Abs(srcRatio - targetRatio) < 0.05f)
                {
                    g.DrawImage(src, new Rectangle(0, 0, width, height));
                }
                else if (srcRatio > targetRatio)
                {
                    int cropW = (int)(src.Height * targetRatio);
                    int cropX = (src.Width - cropW) / 2;
                    g.DrawImage(src, new Rectangle(0, 0, width, height), cropX, 0, cropW, src.Height, GraphicsUnit.Pixel);
                }
                else
                {
                    using (Bitmap thumb = ResizeBitmap(src, 96, 54))
                    {
                        g.DrawImage(thumb, new Rectangle(0, 0, width, height));
                    }
                    using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(170, 8, 8, 12)))
                    {
                        g.FillRectangle(darkBrush, 0, 0, width, height);
                    }

                    int centerH = (int)(height * 0.90f);
                    int centerW = (int)(centerH * srcRatio);
                    int posX = (width - centerW) / 2;
                    int posY = (height - centerH) / 2;

                    using (SolidBrush shadow = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                    {
                        g.FillRectangle(shadow, posX - 10, posY - 5, centerW + 20, centerH + 15);
                    }

                    g.DrawImage(src, new Rectangle(posX, posY, centerW, centerH));
                }
            }
            return canvas;
        }

        public static Bitmap ResizeBitmap(Image src, int w, int h)
        {
            Bitmap b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.DrawImage(src, 0, 0, w, h);
            }
            return b;
        }

        public static void SavePng24(Bitmap bmp, string path)
        {
            using (Bitmap rgb24 = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(rgb24))
                {
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                }
                rgb24.Save(path, ImageFormat.Png);
            }
        }

        public static void SavePng32(Bitmap bmp, string path)
        {
            bmp.Save(path, ImageFormat.Png);
        }
    }
}
