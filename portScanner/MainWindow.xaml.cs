using Microsoft.Win32;
using portScanner.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace portScanner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Scansione> Scansioni;
        CancellationTokenSource Source;
        Stopwatch timer;
        DispatcherTimer dispatcherTimer;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Source = new CancellationTokenSource();
            Scansioni = new List<Scansione>();
            timer = new Stopwatch();
            dispatcherTimer = new DispatcherTimer();
        }

        private async void BtnStartScan_Click(object sender, RoutedEventArgs e)
        {
            // Reset UI
            LblStatScanned.Text = "0";
            LblStatProgress.Text = "0%";
            LblStatOpen.Text = "0";
            LblStatClosed.Text = "0";
            LblStatFiltered.Text = "0";
            DtgResults.Items.Clear();
            Scansioni.Clear();
            PbarScanProgress.Value = 0;
            timer.Reset();
            BtnStopScan.IsEnabled = true;
            BtnStartScan.IsEnabled = false;

            Source = new CancellationTokenSource();
            CancellationToken token = Source.Token;

            int PortStart = 0;
            int PortEnd = 0;
            List<int> Porte = new();

            try
            {
                if (TxtCustomPorts.Text != "")
                {
                    string[] p = TxtCustomPorts.Text.Split(',');
                    foreach (string porta in p)
                        Porte.Add(int.Parse(porta.Trim()));
                }
                else
                {
                    if (CmbPortPreset.SelectedIndex != 0)
                    {
                        switch (CmbPortPreset.SelectedIndex)
                        {
                            case 1: PortStart = 1; PortEnd = 1023; break;
                            case 2: PortStart = 1024; PortEnd = 49151; break;
                            case 3:
                                Porte = new List<int>
                                {
                                    80, 443, 22, 21, 23, 53, 3389,
                                    25, 110, 143, 587, 465, 993, 995,
                                    3306, 5432, 1433, 27017, 6379,
                                    8080, 8443, 8000, 8008, 8081, 8888,
                                    135, 137, 138, 139, 445,
                                    69, 111, 123, 161, 179, 500, 646,
                                    389, 636, 3268,
                                    512, 513, 514, 873, 1080, 4899, 5631,
                                    1194, 1723, 5800, 5900,
                                    548, 2049, 990,
                                    79, 88, 119, 194, 427, 444, 515, 543, 544,
                                    554, 563, 631, 992, 1521, 1900, 2082, 2083,
                                    2121, 2181, 2375, 2376, 3000, 3128, 4444, 5000,
                                    5060, 5985, 5986, 6000, 6379, 6443, 7001, 7070,
                                    8009, 8161, 9000, 9090, 9200, 9300, 9418, 9999,
                                    10000, 49152
                                };
                                break;
                            case 4: PortStart = 1; PortEnd = 65535; break;
                            case 5: Porte = new List<int> { 80, 443, 8080, 8443 }; break;
                            case 6: Porte = new List<int> { 1433, 1521, 3306, 5432, 27017 }; break;
                            default: throw new Exception("Nessuna porta selezionata");
                        }
                    }
                    else
                    {
                        PortStart = int.Parse(TxtPortStart.Text);
                        PortEnd = int.Parse(TxtPortEnd.Text);
                    }
                }

                if (Porte.Count == 0)
                    for (int i = PortStart; i <= PortEnd; i++)
                        Porte.Add(i);

                // Contatori locali
                int aperte = 0, chiuse = 0, filtrate = 0, scansionate = 0;

                // Timer UI
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
                dispatcherTimer.Tick += (s, args) =>
                    LblElapsedTime.Text = timer.Elapsed.ToString(@"hh\:mm\:ss");

                LblScanStatus.Text = "Scansione in corso...";
                timer.Start();
                dispatcherTimer.Start();

                if (ChkTCP.IsChecked == true)
                {
                    string target = TxtTarget.Text;
                    int maxParallel = Convert.ToInt32(SldThread.Value);

                    var semaphore = new SemaphoreSlim(maxParallel);

                    var tasks = Porte.Select(async p =>
                    {
                        await semaphore.WaitAsync(token);
                        try
                        {
                            token.ThrowIfCancellationRequested();

                            Scansione s = new Scansione(target, p);

                            await s.ScanTCPAsync(token);

                            await Dispatcher.InvokeAsync(() =>
                            {
                                switch (s.Stato)
                                {
                                    case StatoPorta.Aperta:
                                        aperte++;
                                        LblStatOpen.Text = aperte.ToString();
                                        break;
                                    case StatoPorta.Chiusa:
                                        chiuse++;
                                        LblStatClosed.Text = chiuse.ToString();
                                        break;
                                    case StatoPorta.Filtrata:
                                        filtrate++;
                                        LblStatFiltered.Text = filtrate.ToString();
                                        break;
                                }
                                scansionate++;
                                LblStatScanned.Text = scansionate.ToString();
                                double percentuale = 100.0 * scansionate / Porte.Count;
                                LblStatProgress.Text = Math.Round(percentuale, 2) + "%";
                                PbarScanProgress.Value = percentuale;
                                Scansioni.Add(s);
                                DtgResults.Items.Add(s);
                            });
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    // Aspetta che tutti i task finiscano
                    await Task.WhenAll(tasks);
                }

                // Scansione completata
                timer.Stop();
                dispatcherTimer.Stop();
                BtnStartScan.IsEnabled = true;
                BtnStopScan.IsEnabled = false;
                LblScanStatus.Text = "Scansione terminata";
            }
            catch (OperationCanceledException)
            {
                timer.Stop();
                dispatcherTimer.Stop();
                BtnStopScan.IsEnabled = false;
                BtnStartScan.IsEnabled = true;
                LblScanStatus.Text = "Scansione annullata";
                MessageBox.Show("Operazione cancellata");
            }
            catch (Exception ex)
            {
                timer.Stop();
                dispatcherTimer.Stop();
                BtnStopScan.IsEnabled = false;
                BtnStartScan.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnStopScan_Click(object sender, RoutedEventArgs e)
        {
            if (!Source.IsCancellationRequested)
            {
                Source.Cancel();
                BtnStopScan.Content = "▶ RIPRENDI";
                BtnStartScan.Content = "▶ AVVIA NUOVA";
                timer.Stop();
                dispatcherTimer.Stop();
                LblScanStatus.Text = "In attesa...";
                BtnStartScan.IsEnabled = true;
            }
            else
            {
                Source.Dispose();
                timer.Start();
                dispatcherTimer.Start();
                LblScanStatus.Text = "Scansione in corso...";
                BtnStopScan.Content = "⏹ FERMA";
                BtnStartScan.Content = "▶ AVVIA";
                BtnStartScan.IsEnabled = false;
            }
        }

        private void SldThread_Change(object sender, RoutedEventArgs e)
        {
            if (LblThreadValue != null)
                LblThreadValue.Content = "Thread: " + SldThread.Value;
        }

        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "File CSV(*.csv)|*.csv";
                if (sfd.ShowDialog() == true)
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                        foreach (Scansione line in Scansioni)
                            sw.WriteLine(line);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export non riuscito: " + ex.Message);
            }
        }
    }
}