using Microsoft.Win32;
using System.IO;

namespace Models.File
{
    /// <summary>
    /// Implementazione concreta di <see cref="MyFile"/> per la gestione di file CSV.
    /// Fornisce metodi per creare, scrivere e leggere file con estensione .csv.
    /// </summary>
    public class FileCSV : MyFile
    {
        /// <summary>
        /// Costruttore
        /// </summary>
        public FileCSV() { }

        private string _name = string.Empty;

        /// <summary>
        /// Nome (percorso completo) del file CSV corrente.
        /// Vuoto finché non viene creato un file con <see cref="NewFile()"/>.
        /// </summary>
        public override string Name { get => _name; }

        /// <summary>
        /// Crea un nuovo file CSV in modalità Create con accesso ReadWrite.
        /// </summary>
        public override void NewFile()
            => NewFile(FileMode.Create, FileAccess.ReadWrite);

        /// <summary>
        /// Crea un nuovo file CSV con la modalità specificata e accesso ReadWrite.
        /// </summary>
        /// <param name="FM">Modalità di apertura del file (es. Create, Append...)</param>
        public override void NewFile(FileMode FM)
            => NewFile(FM, FileAccess.ReadWrite);

        /// <summary>
        /// Apre un <see cref="SaveFileDialog"/> per scegliere dove salvare il file CSV,
        /// poi lo crea con la modalità e l'accesso specificati.
        /// </summary>
        /// <param name="FM">Modalità di apertura del file (es. Create, Append...)</param>
        /// <param name="FA">Tipo di accesso al file (es. Read, Write, ReadWrite)</param>
        /// <exception cref="Exception">Se si verifica un errore durante la creazione</exception>
        public override void NewFile(FileMode FM, FileAccess FA)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "File CSV (*.csv)|*.csv";
                if (sfd.ShowDialog() == true)
                {
                    using FileStream? Stream = new FileStream(sfd.FileName, FM, FA);
                    _name = sfd.FileName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella creazione del file", ex);
            }
        }

        /// <summary>
        /// Scrive testo nel file CSV senza andare a capo.
        /// </summary>
        /// <param name="Text">Il testo da scrivere</param>
        /// <exception cref="Exception">Se il file non è stato creato o si verifica un errore</exception>
        public override void Write(string Text)
        {
            try
            {
                if (Name == string.Empty)
                    throw new Exception("Prima creare il file!");
                using StreamWriter sw = new StreamWriter(Name, append: true);
                sw.Write(Text);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella scrittura su file", ex);
            }
        }

        /// <summary>
        /// Scrive testo nel file CSV e va a capo.
        /// </summary>
        /// <param name="Text">Il testo da scrivere</param>
        /// <exception cref="Exception">Se il file non è stato creato o si verifica un errore</exception>
        public override void WriteLine(string Text)
        {
            try
            {
                if (Name == string.Empty)
                    throw new Exception("Prima creare il file!");
                using StreamWriter sw = new StreamWriter(Name, append: true);
                sw.WriteLine(Text);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella scrittura su file", ex);
            }
        }

        /// <summary>
        /// Legge la prima riga del file CSV.
        /// </summary>
        /// <returns>La prima riga del file come stringa</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public override string ReadLine()
        {
            try
            {
                if (Name == string.Empty)
                    throw new Exception("Prima creare il file!");
                using StreamReader sr = new StreamReader(Name);
                string? line = sr.ReadLine();
                if (line == string.Empty || line == null)
                    throw new Exception("File vuoto");
                return line;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella lettura del file", ex);
            }
        }

        /// <summary>
        /// Legge l'intero contenuto del file CSV come unica stringa.
        /// </summary>
        /// <returns>Il contenuto completo del file</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public override string ReadAllText()
        {
            try
            {
                if (Name == string.Empty)
                    throw new Exception("Prima creare il file!");
                using StreamReader sr = new StreamReader(Name);
                string text = sr.ReadToEnd();
                if (text == string.Empty)
                    throw new Exception("File vuoto");
                return text;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella lettura del file", ex);
            }
        }

        /// <summary>
        /// Legge tutte le righe del file CSV.
        /// </summary>
        /// <returns>Array di stringhe, una per ogni riga del file</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public override string[] ReadAllLines()
        {
            try
            {
                if (Name == string.Empty)
                    throw new Exception("Prima creare il file!");
                using StreamReader sr = new StreamReader(Name);
                string? line;
                List<string> lines = new List<string>();
                while ((line = sr.ReadLine()) != null)
                    lines.Add(line);
                if (lines.Count == 0)
                    throw new Exception("File vuoto");
                return lines.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nella lettura del file", ex);
            }
        }
    }
}