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
                // Sony orbis-pub-cmd exige estrictamente PNG en formato 24-bit RGB (Color Type 2, sin canal alfa)
                using (Bitmap target = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.Clear(Color.Black);

                        g.DrawImage(src, new Rectangle(0, 0, targetWidth, targetHeight));
                    }

                    target.Save(destPath, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Convierte una imagen en un icono cuadrado 512x512 para PS4 (icon0.png).
        /// Admite modos: Llenar Cuadrado (Estilo PS4 Oficial sin bandas ni duplicados), Fondo Negro Puro o Ambiental.
        /// </summary>
        public static void CreateSquareIcon(string sourcePath, string destPath, IconCropStyle style = IconCropStyle.FillSquare)
        {
            if (!File.Exists(sourcePath)) return;

            using (Image src = Image.FromFile(sourcePath))
            {
                using (Bitmap iconBmp = StudioImageProcessor.CreateSquareIcon(src, 512, style))
                {
                    StudioImageProcessor.SavePng24(iconBmp, destPath);
                }
            }
        }

        /// <summary>
        /// Mantiene compatibilidad: genera icono cuadrado con fondo ambiental difuminado.
        /// </summary>
        public static void CreateSquareIconWithAmbientBackground(string sourcePath, string destPath)
        {
            CreateSquareIcon(sourcePath, destPath, IconCropStyle.FitAmbient);
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
