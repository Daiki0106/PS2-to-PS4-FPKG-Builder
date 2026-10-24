using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PS2toPS4Builder.Core
{
    public class RemoteInstaller
    {
        private TcpListener _server;
        private CancellationTokenSource _cts;
        private string _pkgFilePath;
        private int _port;

        public event Action<string> OnLog;
        public event Action<long, long> OnTransferProgress;

        public RemoteInstaller(int port = 8690)
        {
            _port = port;
        }

        private void Log(string msg)
        {
            Action<string> handler = OnLog;
            if (handler != null) handler(msg);
        }

        public static string GetLocalIpAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    if (endPoint != null) return endPoint.Address.ToString();
                }
            }
            catch { }

            // Fallback
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127."))
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        public async Task<bool> CheckPs4ConnectionAsync(string ps4Ip)
        {
            try
            {
                string url = string.Format("http://{0}:12800/api/is_exists", ps4Ip.Trim());
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 3000;
                req.Method = "POST";
                byte[] body = Encoding.UTF8.GetBytes("{\"title_id\":\"CUSA00001\"}");
                req.ContentType = "application/json";
                req.ContentLength = body.Length;

                using (Stream stream = await req.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(body, 0, body.Length);
                }

                using (HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync())
                {
                    return (resp.StatusCode == HttpStatusCode.OK);
                }
            }
            catch (WebException wex)
            {
                // Si respondió el servidor (incluso 404/500), la PS4 está escuchando en el puerto 12800
                if (wex.Response != null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendInstallCommandAsync(string ps4Ip, string pkgPath)
        {
            if (!File.Exists(pkgPath))
            {
                Log("[ERROR] El archivo PKG no existe: " + pkgPath);
                return false;
            }

            _pkgFilePath = pkgPath;
            string localIp = GetLocalIpAddress();

            // 1. Iniciar servidor HTTP ligero local
            StartServer();

            string pkgFileName = Uri.EscapeDataString(Path.GetFileName(pkgPath));
            string downloadUrl = string.Format("http://{0}:{1}/pkg/{2}", localIp, _port, pkgFileName);

            Log(string.Format("[RPI] Servidor de entrega listo en: {0}", downloadUrl));
            Log(string.Format("[RPI] Enviando comando de instalacion a PS4 ({0}:12800)...", ps4Ip));

            try
            {
                string endpoint = string.Format("http://{0}:12800/api/install", ps4Ip.Trim());
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 8000;

                string jsonPayload = string.Format("{{\"type\":\"direct\",\"packages\":[\"{0}\"]}}", downloadUrl);
                byte[] data = Encoding.UTF8.GetBytes(jsonPayload);
                request.ContentLength = data.Length;

                using (Stream reqStream = await request.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string resultJson = await reader.ReadToEndAsync();
                    Log("[RPI] Respuesta de PS4: " + resultJson);

                    if (resultJson.Contains("success") || response.StatusCode == HttpStatusCode.OK)
                    {
                        Log("[SUCCESS] ¡Comando aceptado por la PS4! La descarga comenzará en la consola.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("[ERROR] No se pudo enviar el paquete a la PS4: " + ex.Message);
                Log("[TIP] Asegúrate de que GoldHEN esté activo en tu PS4 y la opción 'Package Installer' (o Remote Package Installer) esté encendida.");
                StopServer();
                return false;
            }

            return false;
        }

        private void StartServer()
        {
            StopServer();

            _cts = new CancellationTokenSource();
            _server = new TcpListener(IPAddress.Any, _port);
            _server.Start();

            Task.Run(() => AcceptClientsAsync(_cts.Token));
        }

        public void StopServer()
        {
            try
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts = null;
                }
                if (_server != null)
                {
                    _server.Stop();
                    _server = null;
                }
            }
            catch { }
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _server != null)
            {
                try
                {
                    TcpClient client = await _server.AcceptTcpClientAsync();
                    Task ignored = Task.Run(() => HandleClientAsync(client, token));
                }
                catch
                {
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    byte[] buffer = new byte[8192];
                    int read = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (read <= 0) return;

                    string headerStr = Encoding.ASCII.GetString(buffer, 0, read);

                    if (!File.Exists(_pkgFilePath))
                    {
                        byte[] notFound = Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n");
                        await stream.WriteAsync(notFound, 0, notFound.Length, token);
                        return;
                    }

                    FileInfo fi = new FileInfo(_pkgFilePath);
                    long totalLength = fi.Length;
                    long startByte = 0;
                    long endByte = totalLength - 1;
                    bool isRange = false;

                    // Soporte de Range Header (HTTP 206)
                    Match rangeMatch = Regex.Match(headerStr, @"Range:\s*bytes=(\d+)-(\d*)", RegexOptions.IgnoreCase);
                    if (rangeMatch.Success)
                    {
                        isRange = true;
                        startByte = long.Parse(rangeMatch.Groups[1].Value);
                        if (!string.IsNullOrEmpty(rangeMatch.Groups[2].Value))
                        {
                            endByte = long.Parse(rangeMatch.Groups[2].Value);
                        }
                    }

                    long contentLength = endByte - startByte + 1;
                    StringBuilder respHeader = new StringBuilder();

                    if (isRange)
                    {
                        respHeader.AppendLine("HTTP/1.1 206 Partial Content");
                        respHeader.AppendLine(string.Format("Content-Range: bytes {0}-{1}/{2}", startByte, endByte, totalLength));
                    }
                    else
                    {
                        respHeader.AppendLine("HTTP/1.1 200 OK");
                    }

                    respHeader.AppendLine("Content-Type: application/octet-stream");
                    respHeader.AppendLine(string.Format("Content-Length: {0}", contentLength));
                    respHeader.AppendLine("Accept-Ranges: bytes");
                    respHeader.AppendLine("Connection: keep-alive");
                    respHeader.AppendLine();

                    byte[] headerBytes = Encoding.ASCII.GetBytes(respHeader.ToString());
                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length, token);

                    // Transmitir datos con búfer de alta velocidad (1 MB)
                    using (FileStream fs = new FileStream(_pkgFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Seek(startByte, SeekOrigin.Begin);
                        byte[] chunk = new byte[1024 * 1024];
                        long bytesRemaining = contentLength;

                        while (bytesRemaining > 0 && !token.IsCancellationRequested)
                        {
                            int toRead = (int)Math.Min(chunk.Length, bytesRemaining);
                            int bytesRead = await fs.ReadAsync(chunk, 0, toRead, token);
                            if (bytesRead <= 0) break;

                            await stream.WriteAsync(chunk, 0, bytesRead, token);
                            bytesRemaining -= bytesRead;

                            long sentBytes = totalLength - bytesRemaining;
                            Action<long, long> progHandler = OnTransferProgress;
                            if (progHandler != null) progHandler(sentBytes, totalLength);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
