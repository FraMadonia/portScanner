using Microsoft.Win32;
using Models.Interface;
using System.IO;

namespace Models.File
{
    /// <summary>
    /// Classe astratta base per la gestione dei file.
    /// Implementa l'interfaccia <see cref="IWrite"/> e fornisce i metodi comuni a tutti i tipi di file.
    /// </summary>
    public abstract class MyFile : IWrite
    {
        /// <summary>
        /// Costruttore
        /// </summary>
        protected MyFile() { }
        /// <summary>
        /// Nome (percorso completo) del file corrente.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Crea un nuovo file con modalità e accesso predefiniti.
        /// </summary>
        public abstract void NewFile();

        /// <summary>
        /// Crea un nuovo file con la modalità specificata.
        /// </summary>
        /// <param name="FM">Modalità di apertura del file (es. Create, Append...)</param>
        public abstract void NewFile(FileMode FM);

        /// <summary>
        /// Crea un nuovo file con modalità e tipo di accesso specificati.
        /// </summary>
        /// <param name="FM">Modalità di apertura del file (es. Create, Append...)</param>
        /// <param name="FA">Tipo di accesso al file (es. Read, Write, ReadWrite)</param>
        public abstract void NewFile(FileMode FM, FileAccess FA);

        /// <summary>
        /// Elimina il file con il nome specificato dal disco.
        /// </summary>
        /// <param name="FileName">Percorso completo del file da eliminare</param>
        /// <exception cref="Exception">Se si verifica un errore durante l'eliminazione</exception>
        public void Delete(string FileName)
        {
            try
            {
                System.IO.File.Delete(FileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'eliminazione del file", ex);
            }
        }

        /// <summary>
        /// Scrive testo nel file senza andare a capo.
        /// </summary>
        /// <param name="Text">Il testo da scrivere</param>
        /// <exception cref="Exception">Se si verifica un errore durante la scrittura</exception>
        public abstract void Write(string Text);

        /// <summary>
        /// Scrive testo nel file e va a capo.
        /// </summary>
        /// <param name="Text">Il testo da scrivere</param>
        /// <exception cref="Exception">Se si verifica un errore durante la scrittura</exception>
        public abstract void WriteLine(string Text);

        /// <summary>
        /// Legge la prima riga del file.
        /// </summary>
        /// <returns>La prima riga del file come stringa</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public abstract string ReadLine();

        /// <summary>
        /// Legge l'intero contenuto del file come unica stringa.
        /// </summary>
        /// <returns>Il contenuto completo del file</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public abstract string ReadAllText();

        /// <summary>
        /// Legge tutte le righe del file.
        /// </summary>
        /// <returns>Array di stringhe, una per ogni riga del file</returns>
        /// <exception cref="Exception">Se il file è vuoto o si verifica un errore</exception>
        public abstract string[] ReadAllLines();
    }
}