using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace portScanner.Models.Scansione
{
    internal class ScanTCP : Scansione
    {
        public ScanTCP() : base() { }
        public ScanTCP(string indirizzo_Hostname, int porta,
                         Protocollo protocollo = Protocollo.Unknow,
                         StatoPorta stato = StatoPorta.Unknown,
                         string servizio = "",
                         string banner = "",
                         int latenza = 0,
                         string versioneServizio = "",
                         string note = "", int Timeout = 1500) 
                                : base(indirizzo_Hostname, porta, protocollo, 
                                stato, servizio, banner, latenza, versioneServizio, note, Timeout)
        { }
        public async Task ScanTCPAsync(CancellationToken externalToken = default)
        {
            Protocollo = Protocollo.TCP;
            try
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(Timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    externalToken,
                    timeoutCts.Token
                );

                using TcpClient tcpClient = new();

                Stopwatch sw = Stopwatch.StartNew();
                await tcpClient.ConnectAsync(Indirizzo_Hostname, Porta, linkedCts.Token);
                sw.Stop();

                Latenza = (int)sw.ElapsedMilliseconds;

                Stato = StatoPorta.Aperta;
                using var bannerCts = new CancellationTokenSource(1000);
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, bannerCts.Token);
                if (bytesRead > 0)
                {
                    string raw = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                    // Tieni solo caratteri stampabili
                    Banner = new string(raw.Where(c => !char.IsControl(c)).ToArray());
                    if (Banner.Length == 0) Banner = string.Empty;
                }
            }
            catch (OperationCanceledException)
            {
                if (externalToken.IsCancellationRequested)
                    throw;

                Stato = StatoPorta.Filtrata;
            }
            catch (SocketException)
            {
                Stato = StatoPorta.Chiusa;
            }
            catch (Exception)
            {
                Stato = StatoPorta.Unknown;
            }
        }

        public static async Task<ScanTCP> ScanTCPAsync(string indirizzo, int porta,
            CancellationToken token = default)
        {
            ScanTCP s = new(indirizzo, porta);
            await s.ScanTCPAsync(token);
            return s;
        }
    }
}
