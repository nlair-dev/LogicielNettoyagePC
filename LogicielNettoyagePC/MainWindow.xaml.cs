using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace LogicielNettoyagePC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
        }

        // Calcul de la taille d'un dossier
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        // Vider un dossier
        public void ClearTempData(DirectoryInfo di)
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
            espace.Content = "0Mb";
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

        public void AnalyserForlders()
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
            

            espace.Content = totalSize + " Mb";
            titre.Content = "Analyse effectuée!";
            date.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
