using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace portScanner.Models.Scansione
{
    internal class ScanUDP : Scansione
    {
        public ScanUDP() : base() { }

        public ScanUDP(string indirizzo_Hostname, int porta,
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

        public async Task ScanUDPAsync(CancellationToken externalToken = default)
        {
            Protocollo = Protocollo.UDP;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
                cts.CancelAfter(TimeSpan.FromMilliseconds(Timeout));

                using UdpClient udpClient = new();

                Stopwatch sw = Stopwatch.StartNew();
                // Manda un pacchetto vuoto
                await udpClient.SendAsync(Array.Empty<byte>(), 0, Indirizzo_Hostname, Porta)
                               .WaitAsync(cts.Token);

                // Aspetta risposta
                var risposta = await udpClient.ReceiveAsync(cts.Token)
                                              .AsTask()
                                              .WaitAsync(cts.Token);

                sw.Stop();

                Latenza = (int)sw.ElapsedMilliseconds;
                Stato = StatoPorta.Aperta;

                if (risposta.Buffer.Length > 0)
                    Banner = Encoding.ASCII.GetString(risposta.Buffer).Trim();
            }
            catch (SocketException sex) when (sex.SocketErrorCode == SocketError.ConnectionReset)
            {
                // Il sistema remoto ha risposto con ICMP "porta chiusa"
                Stato = StatoPorta.Chiusa;
            }
            catch (OperationCanceledException)
            {
                if (externalToken.IsCancellationRequested)
                    throw;

                // Timeout senza risposta → probabilmente filtrata
                Stato = StatoPorta.Filtrata;
            }
            catch
            {
                Stato = StatoPorta.Unknown;
            }
        }

        public static async Task<ScanUDP> ScanUDPAsync(string indirizzo, int porta,
            CancellationToken token = default)
        {
            ScanUDP s = new(indirizzo, porta);
            await s.ScanUDPAsync(token);
            return s;
        }
    }
}