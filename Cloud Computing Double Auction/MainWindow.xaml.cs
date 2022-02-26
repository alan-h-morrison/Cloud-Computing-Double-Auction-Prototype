using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cloud_Computing_Double_Auction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int numUsers = Properties.Settings.Default.NumUsers;
        private int numProviders = Properties.Settings.Default.NumProviders;

        public double elapsedTime;

        public MainWindow()
        { 
            InitializeComponent();
        }
   
        private void BtnAuction_Click(object sender, RoutedEventArgs e)
        {
            var env = new EnvironmentMas();
      
            if(Properties.Settings.Default.ManualUser == true)
            {
                string strQuantities = Properties.Settings.Default.UserQuantities;
                int[] usrQuantities = strQuantities.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                string strPrices = Properties.Settings.Default.UserPrices;
                int[] usrPrices = strPrices.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                UserManualGeneration(env, usrQuantities, usrPrices);
            }
            else
            {
                UserAutoGeneration(env);
            }

            if (Properties.Settings.Default.ManualProvider == true)
            {
                string strQuantities = Properties.Settings.Default.ProviderQuantites;
                int[] proQuantities = strQuantities.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                string strPrices = Properties.Settings.Default.ProviderPrices;
                int[] proPrices = strPrices.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                ProviderManualGeneration(env, proQuantities, proPrices);
            }
            else
            {
                ProviderAutoGeneration(env);
            }

            var auctioneerAgent = new CloudAuctioneer();
            env.Add(auctioneerAgent, "auctioneer");

            var envionmentAgent = new CloudEnvironment();
            env.Add(envionmentAgent, "environment");

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                
                env.Start();
                // the code that you want to measure comes here
                watch.Stop();

                elapsedTime = (watch.ElapsedMilliseconds/ 100.0);

                ResultsWindow resultsWindow = new ResultsWindow(elapsedTime);
                resultsWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("The double auction resulted in no winning users or providers. Please run again or adjust settings appropriately");
            }
        }

        public void UserAutoGeneration(EnvironmentMas enviroment)
        {
            for (int i = 0; i < numUsers; i++)
            {
                var userAgent = new CloudUser();

                enviroment.Add(userAgent, $"user_{i + 1:D3}");
            }
        }

        public void ProviderAutoGeneration(EnvironmentMas environment)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider();
                environment.Add(providerAgent, $"provider_{i + 1:D3}");
            }
        }

        public void UserManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numUsers; i++)
            {
                var userAgent = new CloudUser(quantities[i], prices[i]);
                environment.Add(userAgent, $"user_{i + 1:D3}");
            }            
        }

        public void ProviderManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider(quantities[i], prices[i]);
                environment.Add(providerAgent, $"provider_{i + 1:D3}");
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsMenu = new Settings();
            settingsMenu.Show();
            this.Close();
        }
    }
}
