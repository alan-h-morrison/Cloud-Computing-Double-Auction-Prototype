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
        public MainWindow()
        { 
            InitializeComponent();
        }

        public int[] userQuantites = { 8, 2, 3, 1, 8, 7, 5 };
        public int[] userPrices = { 47, 45, 35, 34, 33, 22, 12};

        public int[] providerQuantities = { 5, 5, 1, 2, 4, 8, 3 };
        public int[] providerPrices = { 15, 17, 20, 22, 25, 30, 49 };

        int numUsers = Properties.Settings.Default.NumUsers;
        int numProviders = Properties.Settings.Default.NumProviders;

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            var env = new EnvironmentMas();

            for (int i = 0; i < 7; i++)
            {
                var providerAgent = new CloudProvider(ProviderPosition.Positive, providerQuantities[i], providerPrices[i]);
                env.Add(providerAgent, $"provider{i+1:D2}");
            }

            for (int i = 0; i < 7; i++)
            {
                var userAgent = new CloudUser(UserPosition.Positive, userQuantites[i], userPrices[i]);
                env.Add(userAgent, $"user{i+1:D2}");
            }

            var auctioneerAgent = new CloudAuctioneer();
            env.Add(auctioneerAgent, "auctioneer");

            var envionmentAgent = new CloudEnvironment();
            env.Add(envionmentAgent, "environment");

            env.Start();
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
                env.Start();

                ResultsWindow resultsWindow = new ResultsWindow();
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
                var userAgent = new CloudUser(UserPosition.Positive);

                enviroment.Add(userAgent, $"user_{i + 1:D3}");
            }
        }

        public void ProviderAutoGeneration(EnvironmentMas environment)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider(ProviderPosition.Positive);
                environment.Add(providerAgent, $"provider_{i + 1:D3}");
            }
        }

        public void UserManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numUsers; i++)
            {
                var userAgent = new CloudUser(UserPosition.Positive, quantities[i], prices[i]);
                environment.Add(userAgent, $"user_{i + 1:D3}");
            }            
        }

        public void ProviderManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider(ProviderPosition.Positive, quantities[i], prices[i]);
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
