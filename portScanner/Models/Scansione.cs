using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Media;

namespace portScanner.Models
{
    /// <summary>
    /// Protocollo di rete usato per la scansione.
    /// È un [Flags] enum, quindi si possono combinare più valori:
    /// es. Protocollo.TCP | Protocollo.UDP
    /// </summary>
    [Flags]
    public enum Protocollo
    {
        Unknow = 0, // nessun protocollo impostato
        TCP = 1,    // Transmission Control Protocol (orientato alla connessione)
        UDP = 2     // User Datagram Protocol (senza connessione)
    }

    /// <summary>
    /// Stato rilevato di una porta durante la scansione.
    /// </summary>
    public enum StatoPorta
    {
        Unknown = 0, // stato non ancora determinato
        Aperta,      // c'è un servizio in ascolto
        Chiusa,      // nessun servizio in ascolto, l'host risponde con rifiuto
        Filtrata     // un firewall blocca la risposta
    }

    /// <summary>
    /// Rappresenta il risultato della scansione di una singola porta.
    /// </summary>
    public class Scansione
    {
        // ───────────────────────────────────────────
        // CAMPI PRIVATI
        // ───────────────────────────────────────────

        private int _porta;
        private int _latenza;

        // ───────────────────────────────────────────
        // PROPRIETÀ
        // ───────────────────────────────────────────
        /// <summary>
        /// Indirizzo IP o Hostname della scansione
        /// </summary>
        public string Indirizzo_Hostname { get; set; }
        /// <summary>
        /// Numero della porta (1-65535).
        /// Lancia un'eccezione se il valore è fuori range.
        /// </summary>
        public int Porta
        {
            get => _porta;
            set => _porta = value < 0 || value > 65535
                ? throw new Exception("Porta inesistente D:")
                : value;
        }

        /// <summary>
        /// Protocollo usato per la scansione (TCP, UDP o entrambi).
        /// </summary>
        public Protocollo Protocollo { get; set; } = Protocollo.Unknow;

        /// <summary>
        /// Stato della porta rilevato durante la scansione.
        /// </summary>
        public StatoPorta Stato { get; set; } = StatoPorta.Unknown;

        /// <summary>
        /// Nome del servizio in ascolto sulla porta (es. "Apache", "OpenSSH").
        /// Ottenuto tramite banner grabbing o dizionario porte note.
        /// </summary>
        public string Servizio { get; set; } = string.Empty;

        /// <summary>
        /// Versione del servizio in ascolto (es. "2.4.51", "8.9").
        /// Ottenuta leggendo il banner del servizio.
        /// </summary>
        public string VersioneServizio { get; set; } = string.Empty;

        /// <summary>
        /// Data e ora in cui è stata eseguita la scansione di questa porta.
        /// </summary>
        public DateTime Data { get; set; } = DateTime.Now;

        /// <summary>
        /// Messaggio di benvenuto inviato automaticamente dal servizio
        /// quando ci si connette alla porta (es. "SSH-2.0-OpenSSH_8.9").
        /// </summary>
        public string Banner { get; set; } = string.Empty;

        /// <summary>
        /// Tempo di risposta della porta in millisecondi.
        /// Lancia un'eccezione se il valore è negativo.
        /// </summary>
        public int Latenza
        {
            get => _latenza;
            set => _latenza = value < 0
                ? throw new Exception("Latenza negativa :/")
                : value;
        }

        /// <summary>
        /// Note aggiuntive sulla porta (generate automaticamente o scritte dall'utente).
        /// </summary>
        public string Note { get; set; } = string.Empty;

        public string StatoColore
        {
            get
            {
                switch (Stato)
                {
                    case StatoPorta.Aperta:
                        return "#10B981";
                    case StatoPorta.Chiusa:
                        return "#EF4444";
                    case StatoPorta.Filtrata:
                        return "#F59E0B";
                    default:
                        return "#4B5563";
                }
            }
        }
        public string StatoIcona
        {
            get
            {
                switch (Stato)
                {
                    case StatoPorta.Aperta:
                        return "pack://application:,,,/img/open.png";
                    case StatoPorta.Chiusa:
                        return "pack://application:,,,/img/closed.png";
                    case StatoPorta.Filtrata:
                        return "pack://application:,,,/img/filtered.png";
                    default:
                        return "pack://application:,,,/img/unknow.png";
                }
            }
        }

        // ───────────────────────────────────────────
        // PORTE CRITICHE
        // ───────────────────────────────────────────

        /// <summary>
        /// Lista di porte considerate critiche dal punto di vista della sicurezza.
        /// static = condivisa tra tutte le istanze, readonly = non modificabile.
        /// </summary>
        private static readonly List<int> _porteCritiche = new()
        {
            21,   // FTP  - trasferisce credenziali in chiaro
            23,   // Telnet - protocollo non sicuro
            445,  // SMB - condivisione file Windows, spesso vulnerabile
            512,  // rexec - esecuzione remota non sicura
            513,  // rlogin - login remoto non sicuro
            514,  // rsh - shell remota non sicura
            1433, // SQL Server - database esposto
            3389  // RDP - desktop remoto Windows, bersaglio di brute force
        };

        /// <summary>
        /// Restituisce true se la porta è aperta ED è nella lista delle porte critiche.
        /// Proprietà calcolata automaticamente, non ha bisogno di essere impostata.
        /// </summary>
        public bool IsCritical => _porteCritiche.Contains(Porta) && Stato == StatoPorta.Aperta;

        // ───────────────────────────────────────────
        // COSTRUTTORI
        // ───────────────────────────────────────────

        /// <summary>
        /// Costruttore vuoto. Tutte le proprietà assumono i valori di default.
        /// </summary>
        public Scansione() { }

        /// <summary>
        /// Costruttore parametrizzato. Solo la porta è obbligatoria,
        /// tutti gli altri parametri sono opzionali.
        /// </summary>
        public Scansione(string indirizzo_Hostname, int porta,
                         Protocollo protocollo = Protocollo.Unknow,
                         StatoPorta stato = StatoPorta.Unknown,
                         string servizio = "",
                         string banner = "",
                         int latenza = 0,
                         string versioneServizio = "",
                         string note = "")
        {
            Indirizzo_Hostname = indirizzo_Hostname;
            Porta = porta;
            Protocollo = protocollo;
            Stato = stato;
            Servizio = servizio;
            Banner = banner;
            Latenza = latenza;
            VersioneServizio = versioneServizio;
            Note = note;
        }

        // ───────────────────────────────────────────
        // METODI
        // ───────────────────────────────────────────

        /// <summary>
        /// Restituisce una stringa con tutti i dati della scansione separati da virgola.
        /// Utile per il log e l'export CSV.
        /// </summary>
        public override string ToString()
        {
            return $"{Indirizzo_Hostname},{Porta},{Protocollo},{Stato},{Servizio},{VersioneServizio},{Data},{Banner},{Latenza},{Note}";
        }

        /// <summary>
        /// Due scansioni sono uguali se hanno la stessa porta e lo stesso protocollo.
        /// (TCP porta 80 e UDP porta 80 sono considerate diverse)
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Scansione other) return false;
            return Indirizzo_Hostname == other.Indirizzo_Hostname && Porta == other.Porta && Protocollo == other.Protocollo;
        }

        /// <summary>
        /// Operatore == con gestione del null su entrambi i lati.
        /// </summary>
        public static bool operator ==(Scansione? a, Scansione? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }
        /// <summary>
        /// Operatore != opposto di ==.
        /// </summary>
        public static bool operator !=(Scansione? a, Scansione? b) => !(a == b);

        /// <summary>
        /// Necessario quando si sovrascrive Equals.
        /// Combina porta e protocollo per generare un codice univoco.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Indirizzo_Hostname, Porta, Protocollo);
        }

        public async Task ScanTCP()
        {
            Protocollo = Protocollo.TCP;
            try
            {
                using (TcpClient tcpClient = new())
                {
                    using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                    {
                        await tcpClient.ConnectAsync(Indirizzo_Hostname, Porta, cts.Token);
                        Stato = StatoPorta.Aperta;
                    }
                }
            }
            catch (OperationCanceledException)
            {
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
        public static async Task<Scansione> ScanTCP(string indirizzo, int porta)
        {
            Scansione s = new(indirizzo, porta);
            await s.ScanTCP();
            return s;
        }
    }

}
