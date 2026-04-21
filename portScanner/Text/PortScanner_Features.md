# 🔍 Port Scanner WPF — Spiegazione delle Feature

---

## ✅ Required Features

### 1. Scansione porte su IP/hostname specifico
L'utente inserisce un indirizzo IP (es. `192.168.1.1`) o un hostname (es. `google.com`) e il programma tenta di connettersi a ciascuna porta di quell'host per vedere se è raggiungibile.

### 2. Visualizzazione porte aperte/chiuse
Per ogni porta scansionata, il risultato viene mostrato nell'interfaccia: verde/aperta se la connessione riesce, rossa/chiusa se viene rifiutata o non risponde.

### 3. Gestione range porte da scansire
L'utente può definire un intervallo, ad esempio dalla porta `1` alla `1024`, invece di scansionare tutte le 65535 porte. Si inseriscono una porta di inizio e una di fine.

### 4. Interfaccia grafica per inserimento parametri
Una finestra WPF con campi di testo dove l'utente inserisce IP, range di porte, timeout ecc., con un pulsante "Avvia scansione".

### 5. Visualizzazione risultati in tempo reale
I risultati appaiono man mano che le porte vengono scansionate, senza aspettare la fine. Si usa tipicamente una `ListView` aggiornata in modo asincrono.

---

## 🔵 Optional Features

### 6. Scansione UDP oltre TCP
TCP verifica la connessione con un handshake (più affidabile). UDP è senza connessione, quindi è più difficile da rilevare — si invia un pacchetto e si aspetta una risposta o un errore ICMP.

### 7. Service fingerprinting
Dopo aver trovato una porta aperta, il programma invia una piccola richiesta e analizza la risposta per capire che servizio è in ascolto (es. porta 80 → HTTP, porta 22 → SSH).

### 8. Export risultati in JSON/CSV
Al termine della scansione, l'utente può salvare i risultati su file `.json` o `.csv` per analisi successive o documentazione.

### 9. Salvataggio cronologia scansioni
Le scansioni passate vengono salvate localmente (es. in un file SQLite o JSON) e l'utente può riaprirle e confrontarle in seguito.

### 10. Multi-threading per scansioni veloci
Invece di scansionare una porta alla volta (lento), si usano più thread in parallelo (es. con `Task` e `SemaphoreSlim` in C#) per scansionare centinaia di porte contemporaneamente.

### 11. Timeout configurabile
L'utente decide quanti millisecondi aspettare per ogni porta prima di considerarla chiusa. Un timeout basso = scansione veloce ma meno precisa. Un timeout alto = più lenta ma più affidabile.

### 12. Scansione subnet intera
Invece di un singolo IP, si scansiona un'intera rete, ad esempio `192.168.1.0/24` (256 host), cercando quali macchine sono attive e quali porte hanno aperte.

### 13. Whois lookup per IP
Per un dato IP, il programma interroga i server WHOIS pubblici per ottenere informazioni su chi è il proprietario di quell'indirizzo (organizzazione, paese, ISP).

### 14. Rilevamento OS (OS fingerprinting)
Analizzando le risposte di rete (TTL, finestra TCP, ordine delle opzioni), si cerca di indovinare il sistema operativo del target (es. Windows, Linux). Tecnica usata da tool come Nmap.

### 15. Ping sweep per discovery host attivi
Prima di scansionare le porte, si invia un ping ICMP a tutti gli IP di una rete per scoprire quali host sono online, evitando di perdere tempo su IP inattivi.

### 16. Scansione SYN stealth (half-open scan)
Invece di completare il TCP handshake (SYN → SYN-ACK → ACK), si invia solo il SYN e si ascolta la risposta. Se arriva SYN-ACK la porta è aperta, ma la connessione non viene mai completata — più difficile da loggare. Richiede privilegi di rete elevati (raw socket).

### 17. Rilevamento versioni servizi (Service versioning)
Simile al fingerprinting, ma più approfondito: si cerca di identificare non solo il tipo di servizio ma anche la sua versione (es. `Apache 2.4.51`, `OpenSSH 8.2`), utile per trovare vulnerabilità note.

### 18. Filtri e ordinamento risultati
Nella lista dei risultati, l'utente può filtrare (es. mostra solo porte aperte) oppure ordinare per numero di porta, stato, nome servizio ecc.

### 19. Notifiche per porte critiche/note
Se viene trovata aperta una porta considerata sensibile (es. `23` Telnet, `3389` RDP, `445` SMB), il programma evidenzia il risultato o mostra un avviso all'utente.

### 20. Statistiche scansione
Al termine viene mostrato un riepilogo: quante porte scansionate, quante aperte, quante chiuse, tempo totale impiegato, velocità media ecc.

---

*Documento generato per il progetto Port Scanner WPF*
