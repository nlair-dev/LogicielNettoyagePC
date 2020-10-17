using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace LogicielNettoyagePC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActu();
            CheckVersion();
            GetDate();
        }

        // Vérification d'une nouvelle actualité
        private void CheckActu()
        {
            string url = "http://localhost/siteWeb/actu.txt";
            using (WebClient client = new WebClient())
            {
                string actu = client.DownloadString(url);
                if(actu != string.Empty)
                {
                    lbl_actu.Content = actu;
                    lbl_actu.Visibility = Visibility.Visible;
                    rectangle_actu.Visibility = Visibility.Visible;
                }
            }
        }

        // Vérification de la version
        private void CheckVersion()
        {
            string url = "http://localhost/siteWeb/version.txt";
            using (WebClient client = new WebClient())
            {
                string derniereVersion = client.DownloadString(url);
                if(version != derniereVersion)
                {
                    MessageBox.Show("Une nouvelle mise à jour est disponible", "Nouvelle mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Calcul de la taille d'un dossier
        private long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        // Vider un dossier
        private void ClearTempData(DirectoryInfo di)
        {
            foreach(FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                } catch(Exception)
                {
                    continue;
                }
            }

            foreach(DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName);
                } catch(Exception)
                {
                    continue;
                }
            }
        }

        private void Button_Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "Nettoyage en cours";
            Clipboard.Clear(); // Nettoyage du presse papier

            // Nettoyage des fichiers temporaires de windows
            try
            {
                ClearTempData(winTemp); 
            } catch(Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }


            // Nettoyage des fichiers temporaires des applications
            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }

            btnClean.Content = "Nettoyage terminé";
            lbl_espace.Content = "0Mb";
        }

        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Votre logiciel est à jour", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Historique_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO : Créer page historique","Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_SiteWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://zestedesavoir.com/")
                {
                    UseShellExecute = true
                });
            } catch(Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyserForlders();
        }

        private void AnalyserForlders()
        {
            Console.WriteLine("Début de l'analyse...");
            long totalSize = 0;

            try
            {
                totalSize += DirSize(winTemp) / 1000000;
                totalSize += DirSize(appTemp) / 1000000;
            } catch(Exception ex)
            {
                Console.WriteLine("Impossible d'analyser les dossiers: " + ex.Message);
            }
            

            lbl_espace.Content = totalSize + " Mb";
            lbl_titre.Content = "Analyse effectuée!";
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            lbl_date.Content = date;
            SaveDate(date);
        }

        // Sauvegarde la date de dernière analyse
        private void SaveDate(string date)
        {
            File.WriteAllText("date.txt", date);
        }

        private void GetDate()
        {
            string date = File.ReadAllText("date.txt");
            if(date != String.Empty)
            {
                lbl_date.Content = date;
            }
        }
    }
}
