using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2toPS4Builder.Core
{
    public class CoverResult
    {
        public string RawCoverPath;
        public string SquareIconPath;
        public string BackgroundPath;
        public string DiscIconPath;
        public bool IsFromLocalDataset;
    }

    public static class CoverDownloader
    {
        // Red multi-espejo redundante: si GitHub cae, salta a jsDelivr y luego a Statically
        private static readonly string[] MirrorEndpoints2D = new string[]
        {
            "https://raw.githubusercontent.com/xlenore/ps2-covers/main/covers/default/{0}.jpg",
            "https://cdn.jsdelivr.net/gh/xlenore/ps2-covers@main/covers/default/{0}.jpg",
            "https://cdn.statically.io/gh/xlenore/ps2-covers/main/covers/default/{0}.jpg"
        };

        private static readonly string[] MirrorEndpoints3D = new string[]
        {
            "https://raw.githubusercontent.com/xlenore/ps2-covers/main/covers/3d/{0}.png",
            "https://cdn.jsdelivr.net/gh/xlenore/ps2-covers@main/covers/3d/{0}.png",
            "https://cdn.statically.io/gh/xlenore/ps2-covers/main/covers/3d/{0}.png"
        };

        public static async Task<CoverResult> GetGameMediaAsync(string serial, string gameTitle, string localDatasetDir, string cacheDirectory)
        {
            if (string.IsNullOrEmpty(serial) && string.IsNullOrEmpty(gameTitle)) return null;

            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
            }

            // 1. INTENTAR BUSQUEDA EN DATASET LOCAL (Modo Offline)
            if (!string.IsNullOrEmpty(localDatasetDir) && Directory.Exists(localDatasetDir))
            {
                CoverResult localResult = TryFetchFromLocal(localDatasetDir, serial, cacheDirectory);
                if (localResult != null && !string.IsNullOrEmpty(localResult.SquareIconPath))
                {
                    localResult.IsFromLocalDataset = true;
                    return localResult;
                }
            }

            // 2. FALLBACK A INTERNET SI NO ESTA EN EL DISCO LOCAL
            CoverResult onlineResult = await FetchOnlineAsync(serial, gameTitle, cacheDirectory);
            if (onlineResult != null)
            {
                onlineResult.IsFromLocalDataset = false;
            }
            return onlineResult;
        }

        private static CoverResult TryFetchFromLocal(string localDatasetDir, string serial, string cacheDirectory)
        {
            try
            {
                string oplFolderName = MapSerialToOplFormat(serial);
                string gameFolder = Path.Combine(localDatasetDir, oplFolderName);

                if (!Directory.Exists(gameFolder))
                {
                    gameFolder = Path.Combine(localDatasetDir, serial.Trim().ToUpper());
                }

                if (!Directory.Exists(gameFolder)) return null;

                string coverFile = null;
                string discFile = null;
                string bgFile = null;

                // 1. Prioridad: Carátula frontal completa (_COV o _COV2)
                string[] covCandidates = Directory.GetFiles(gameFolder, "*_COV.*");
                if (covCandidates.Length == 0)
                {
                    covCandidates = Directory.GetFiles(gameFolder, "*_COV2.*");
                }
                if (covCandidates.Length > 0) coverFile = covCandidates[0];

                // 2. Disco original (_ICO)
                string[] icoCandidates = Directory.GetFiles(gameFolder, "*_ICO.*");
                if (icoCandidates.Length > 0) discFile = icoCandidates[0];

                // 3. Fondo (_BG_00, _BG, o capturas _SCR)
                string[] bgCandidates = Directory.GetFiles(gameFolder, "*_BG*.*");
                if (bgCandidates.Length > 0)
                {
                    bgFile = bgCandidates[0];
                }
                else
                {
                    string[] scrCandidates = Directory.GetFiles(gameFolder, "*_SCR*.*");
                    if (scrCandidates.Length > 0) bgFile = scrCandidates[0];
                }

                if (string.IsNullOrEmpty(coverFile) && string.IsNullOrEmpty(discFile) && string.IsNullOrEmpty(bgFile)) return null;

                CoverResult res = new CoverResult();
                string cleanSerial = serial.Replace("-", "_").Replace(".", "_");

                // Generar Icono Cuadrado Ambientado con la Carátula Frontal
                if (!string.IsNullOrEmpty(coverFile))
                {
                    string destCoverIcon = Path.Combine(cacheDirectory, cleanSerial + "_cover_icon0.png");
                    ImageHelper.CreateSquareIconWithAmbientBackground(coverFile, destCoverIcon);
                    res.SquareIconPath = destCoverIcon;
                    res.RawCoverPath = coverFile;
                }

                // Generar versión del Disco por si el usuario quiere alternar
                if (!string.IsNullOrEmpty(discFile))
                {
                    string destDisc = Path.Combine(cacheDirectory, cleanSerial + "_disc_icon0.png");
                    ImageHelper.ConvertAndResize(discFile, destDisc, 512, 512);
                    res.DiscIconPath = destDisc;

                    // Si no había carátula, usar el disco como icono principal
                    if (string.IsNullOrEmpty(res.SquareIconPath))
                    {
                        res.SquareIconPath = destDisc;
                    }
                }

                // Procesar Fondo 1920x1080
                if (!string.IsNullOrEmpty(bgFile))
                {
                    string destBg = Path.Combine(cacheDirectory, cleanSerial + "_pic1.png");
                    ImageHelper.ConvertAndResize(bgFile, destBg, 1920, 1080);
                    res.BackgroundPath = destBg;
                }
                else if (!string.IsNullOrEmpty(coverFile))
                {
                    // Si el dataset local no incluye fondo dedicado, autogenerar fondo cinemático 1920x1080 desde la carátula
                    string destBg = Path.Combine(cacheDirectory, cleanSerial + "_pic1.png");
                    try
                    {
                        ImageHelper.CreateCinematicBackground(coverFile, destBg);
                        res.BackgroundPath = destBg;
                    }
                    catch { }
                }

                return res;
            }
            catch
            {
                return null;
            }
        }

        private static string MapSerialToOplFormat(string serial)
        {
            if (string.IsNullOrEmpty(serial)) return "";
            string clean = serial.Trim().ToUpper();
            Match m = Regex.Match(clean, @"([A-Z]{4})[-_]?(\d{3})(\d{2})");
            if (m.Success)
            {
                return string.Format("{0}_{1}.{2}", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            return clean;
        }

        private static async Task<CoverResult> FetchOnlineAsync(string serial, string gameTitle, string cacheDirectory)
        {
            string cleanSerial = NormalizeSerial(serial);
            string downloadedPath = null;

            if (!string.IsNullOrEmpty(cleanSerial))
            {
                downloadedPath = await TryDownloadAsync(cleanSerial, cacheDirectory);
            }

            if (string.IsNullOrEmpty(downloadedPath) && !string.IsNullOrEmpty(gameTitle))
            {
                string altSerial = TitleDatabase.FindSerialByKeywords(gameTitle);
                if (!string.IsNullOrEmpty(altSerial))
                {
                    string cleanAlt = NormalizeSerial(altSerial);
                    downloadedPath = await TryDownloadAsync(cleanAlt, cacheDirectory);
                }
            }

            if (string.IsNullOrEmpty(downloadedPath) || !File.Exists(downloadedPath))
            {
                return null;
            }

            string icon512Path = Path.Combine(cacheDirectory, Path.GetFileNameWithoutExtension(downloadedPath) + "_icon512.png");
            string pic1080Path = Path.Combine(cacheDirectory, Path.GetFileNameWithoutExtension(downloadedPath) + "_pic1080.png");

            try
            {
                ImageHelper.CreateSquareIconWithAmbientBackground(downloadedPath, icon512Path);
            }
            catch
            {
                icon512Path = downloadedPath;
            }

            try
            {
                ImageHelper.CreateCinematicBackground(downloadedPath, pic1080Path);
            }
            catch
            {
                pic1080Path = null;
            }

            CoverResult result = new CoverResult();
            result.RawCoverPath = downloadedPath;
            result.SquareIconPath = icon512Path;
            result.BackgroundPath = pic1080Path;
            return result;
        }

        private static async Task<string> TryDownloadAsync(string serialFormatted, string cacheDirectory)
        {
            string targetFile2D = Path.Combine(cacheDirectory, serialFormatted + "_raw.jpg");
            if (File.Exists(targetFile2D) && new FileInfo(targetFile2D).Length > 1000)
            {
                return targetFile2D;
            }

            string targetFile3D = Path.Combine(cacheDirectory, serialFormatted + "_3d.png");
            if (File.Exists(targetFile3D) && new FileInfo(targetFile3D).Length > 1000)
            {
                return targetFile3D;
            }

            // 1. Probar todos los espejos para carátula 2D
            foreach (string mirrorPattern in MirrorEndpoints2D)
            {
                string url = string.Format(mirrorPattern, serialFormatted);
                if (await DownloadFileAsync(url, targetFile2D))
                {
                    return targetFile2D;
                }
            }

            // 2. Si no hubo 2D, probar todos los espejos para carátula 3D
            foreach (string mirrorPattern in MirrorEndpoints3D)
            {
                string url = string.Format(mirrorPattern, serialFormatted);
                if (await DownloadFileAsync(url, targetFile3D))
                {
                    return targetFile3D;
                }
            }

            return null;
        }

        private static string NormalizeSerial(string serial)
        {
            if (string.IsNullOrEmpty(serial)) return "";
            string clean = serial.Trim().ToUpper();
            Match m = Regex.Match(clean, @"([A-Z]{4})[-_]?(\d{3})\.?(\d{2})");
            if (m.Success)
            {
                return string.Format("{0}-{1}{2}", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            return clean;
        }

        private static async Task<bool> DownloadFileAsync(string url, string destinationPath)
        {
            try
            {
                using (TimeoutWebClient client = new TimeoutWebClient(4500))
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) PS2toPS4Builder/1.0");
                    await client.DownloadFileTaskAsync(new Uri(url), destinationPath);
                    return File.Exists(destinationPath) && new FileInfo(destinationPath).Length > 1000;
                }
            }
            catch
            {
                if (File.Exists(destinationPath))
                {
                    try { File.Delete(destinationPath); } catch { }
                }
                return false;
            }
        }
    }

    public class TimeoutWebClient : WebClient
    {
        private readonly int _timeoutMs;

        public TimeoutWebClient(int timeoutMs = 4000)
        {
            _timeoutMs = timeoutMs;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (req != null)
            {
                req.Timeout = _timeoutMs;
            }
            return req;
        }
    }
}
