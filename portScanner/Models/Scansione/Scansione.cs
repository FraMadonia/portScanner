using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;

namespace portScanner.Models.Scansione
{
    [Flags]
    public enum Protocollo
    {
        Unknow = 0,
        TCP = 1,
        UDP = 2
    }

    public enum StatoPorta
    {
        Unknown = 0,
        Aperta,
        Chiusa,
        Filtrata
    }

    public class Scansione
    {
        private int _porta;
        private int _latenza;
        private int _timeout = 1500;

        public string Indirizzo_Hostname { get; set; }

        public int Porta
        {
            get => _porta;
            set => _porta = value < 0 || value > 65535
                ? throw new Exception("Porta inesistente D:")
                : value;
        }

        public Protocollo Protocollo { get; set; } = Protocollo.Unknow;
        public StatoPorta Stato { get; set; } = StatoPorta.Unknown;
        public string Servizio { get; set; } = string.Empty;
        public string VersioneServizio { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;
        public string Banner { get; set; } = string.Empty;

        public int Latenza
        {
            get => _latenza;
            set => _latenza = value < 0
                ? throw new Exception("Latenza negativa :/")
                : value;
        }
        public int Timeout
        {
            get => _timeout;
            set => _timeout = value < 0
                ? throw new Exception("Timeout negativo :/")
                : value;
        }

        public string Note { get; set; } = string.Empty;

        public string StatoColore => Stato switch
        {
            StatoPorta.Aperta => "#10B981",
            StatoPorta.Chiusa => "#EF4444",
            StatoPorta.Filtrata => "#F59E0B",
            _ => "#4B5563"
        };

        public string StatoIcona => Stato switch
        {
            StatoPorta.Aperta => "pack://application:,,,/img/open.png",
            StatoPorta.Chiusa => "pack://application:,,,/img/closed.png",
            StatoPorta.Filtrata => "pack://application:,,,/img/filtered.png",
            _ => "pack://application:,,,/img/unknow.png"
        };

        private static readonly List<int> _porteCritiche = new()
        {
            21, 23, 445, 512, 513, 514, 1433, 3389
        };

        public bool IsCritical => _porteCritiche.Contains(Porta) && Stato == StatoPorta.Aperta;

        public Scansione() { }

        public Scansione(string indirizzo_Hostname, int porta,
                         Protocollo protocollo = Protocollo.Unknow,
                         StatoPorta stato = StatoPorta.Unknown,
                         string servizio = "",
                         string banner = "",
                         int latenza = 0,
                         string versioneServizio = "",
                         string note = "",
                         int timeout = 1500)
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
            Timeout = timeout;
        }

        public override string ToString()
        {
            return $"{Indirizzo_Hostname},{Porta},{Protocollo},{Stato},{Servizio},{VersioneServizio},{Data},{Banner},{Latenza},{Timeout},{Note}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Scansione other) return false;
            return Indirizzo_Hostname == other.Indirizzo_Hostname
                && Porta == other.Porta
                && Protocollo == other.Protocollo
                && Timeout == other.Timeout;
        }

        public static bool operator ==(Scansione? a, Scansione? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Scansione? a, Scansione? b) => !(a == b);

        public override int GetHashCode() =>
            HashCode.Combine(Indirizzo_Hostname, Porta, Protocollo, Timeout);

    }
}