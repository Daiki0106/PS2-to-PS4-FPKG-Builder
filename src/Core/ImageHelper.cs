using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace PS2toPS4Builder.Core
{
    public static class ImageHelper
    {
        /// <summary>
        /// Procesa y redimensiona cualquier imagen para que cumpla estrictamente
        /// con los requisitos de Sony para PS4:
        /// - icon0.png: exactamente 512x512 (< 1MB)
        /// - pic1.png: exactamente 1920x1080 (< 1MB)
        /// </summary>
        public static void ConvertAndResize(string sourcePath, string destPath, int targetWidth, int targetHeight)
        {
            if (!File.Exists(sourcePath)) return;

            using (Image src = Image.FromFile(sourcePath))
            {
                FileInfo fi = new FileInfo(sourcePath);
                if (src.Width == targetWidth && src.Height == targetHeight && fi.Length < 1000000 && Path.GetExtension(sourcePath).ToLower() == ".png")
                {
                    File.Copy(sourcePath, destPath, true);
                    return;
                }

                PixelFormat format = Image.IsAlphaPixelFormat(src.PixelFormat) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;
                using (Bitmap target = new Bitmap(targetWidth, targetHeight, format))
                {
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;

                        if (!Image.IsAlphaPixelFormat(src.PixelFormat))
                        {
                            g.Clear(Color.Black);
                        }
                        else
                        {
                            g.Clear(Color.Transparent);
                        }

                        g.DrawImage(src, new Rectangle(0, 0, targetWidth, targetHeight));
                    }

                    target.Save(destPath, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Convierte una carátula vertical de PS2 en un icono cuadrado 512x512
        /// con fondo ambiental difuminado estilo PlayStation 4, sin deformar la imagen.
        /// </summary>
        public static void CreateSquareIconWithAmbientBackground(string sourcePath, string destPath)
        {
            if (!File.Exists(sourcePath)) return;

            using (Image src = Image.FromFile(sourcePath))
            {
                using (Bitmap target = new Bitmap(512, 512, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;

                        // 1. Fondo difuminado / oscurecido ocupando todo el cuadrado
                        g.DrawImage(src, new Rectangle(-20, -20, 552, 552));
                        using (Brush darkTint = new SolidBrush(Color.FromArgb(160, 10, 12, 18)))
                        {
                            g.FillRectangle(darkTint, 0, 0, 512, 512);
                        }

                        // 2. Carátula centrada respetando proporción original
                        float aspect = (float)src.Width / src.Height;
                        int fitHeight = 490;
                        int fitWidth = (int)(fitHeight * aspect);
                        if (fitWidth > 490)
                        {
                            fitWidth = 490;
                            fitHeight = (int)(fitWidth / aspect);
                        }

                        int x = (512 - fitWidth) / 2;
                        int y = (512 - fitHeight) / 2;

                        // Sombra sutil detrás de la carátula
                        using (Brush shadow = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                        {
                            g.FillRectangle(shadow, x - 4, y - 4, fitWidth + 8, fitHeight + 8);
                        }

                        // Dibujar carátula nítida
                        g.DrawImage(src, new Rectangle(x, y, fitWidth, fitHeight));

                        // Borde fino elegante
                        using (Pen borderPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
                        {
                            g.DrawRectangle(borderPen, x, y, fitWidth, fitHeight);
                        }
                    }

                    target.Save(destPath, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Genera un fondo panorámico cinematográfico 1920x1080 para PS4 (pic1.png)
        /// a partir de la carátula del juego.
        /// </summary>
        public static void CreateCinematicBackground(string sourcePath, string destPath)
        {
            if (!File.Exists(sourcePath)) return;

            using (Image src = Image.FromFile(sourcePath))
            {
                using (Bitmap target = new Bitmap(1920, 1080, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;

                        // Fondo ambiental ultra escalado
                        g.DrawImage(src, new Rectangle(-100, -100, 2120, 1280));

                        // Degradado oscuro estilo interfaz PlayStation
                        using (LinearGradientBrush gradient = new LinearGradientBrush(
                            new Point(0, 0), new Point(1920, 0),
                            Color.FromArgb(235, 12, 14, 20),
                            Color.FromArgb(120, 15, 20, 30)))
                        {
                            g.FillRectangle(gradient, 0, 0, 1920, 1080);
                        }

                        // Carátula destacada en el tercio derecho
                        float aspect = (float)src.Width / src.Height;
                        int coverHeight = 850;
                        int coverWidth = (int)(coverHeight * aspect);
                        int coverX = 1920 - coverWidth - 140;
                        int coverY = (1080 - coverHeight) / 2;

                        // Sombra exterior
                        using (Brush shadowBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                        {
                            g.FillRectangle(shadowBrush, coverX - 8, coverY - 8, coverWidth + 16, coverHeight + 16);
                        }

                        g.DrawImage(src, new Rectangle(coverX, coverY, coverWidth, coverHeight));

                        using (Pen pen = new Pen(Color.FromArgb(80, 255, 255, 255), 1.5f))
                        {
                            g.DrawRectangle(pen, coverX, coverY, coverWidth, coverHeight);
                        }
                    }

                    target.Save(destPath, ImageFormat.Png);
                }
            }
        }
    }
}
