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
        // Number of cloud users and providers participating in the auction
        private int numUsers = Properties.Settings.Default.NumUsers;
        private int numProviders = Properties.Settings.Default.NumProviders;

        public double elapsedTime;

        public MainWindow()
        { 
            InitializeComponent();
        }
   
        // When a user clicks the "Run Auction" button, the MAS begins using variables determined by the settings
        private void BtnAuction_Click(object sender, RoutedEventArgs e)
        {
            // Create new environment for all agents to parcipate within
            var env = new EnvironmentMas();
      
            // Checks if settings to see if the auction is set to operate with manual entry of user bids
            if(Properties.Settings.Default.ManualUser == true)
            {
                // User quantities are sotred as an int array
                string strQuantities = Properties.Settings.Default.UserQuantities;
                int[] usrQuantities = strQuantities.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                // User prices are sotred as an int array
                string strPrices = Properties.Settings.Default.UserPrices;
                int[] usrPrices = strPrices.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                // Calls method to generate the user agents with appropriate bid quantites and prices
                UserManualGeneration(env, usrQuantities, usrPrices);
            }
            else
            {
                UserAutoGeneration(env);
            }

            // checks settings to see if the auction is set to operate with manual entry of provider bids
            if (Properties.Settings.Default.ManualProvider == true)
            {
                // Provider quantities are sotred as an int array
                string strQuantities = Properties.Settings.Default.ProviderQuantites;
                int[] proQuantities = strQuantities.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                
                // Provider quantities are sotred as an int array
                string strPrices = Properties.Settings.Default.ProviderPrices;
                int[] proPrices = strPrices.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                // Calls method to generate the user agents with appropriate bid quantites and prices
                ProviderManualGeneration(env, proQuantities, proPrices);
            }
            else
            {
                // If the user bids are not set to be manually generated, they are automatically generated 
                ProviderAutoGeneration(env);
            }

            // Cloud auctioneer agent created and added into MAS environment
            var auctioneerAgent = new CloudAuctioneer();
            env.Add(auctioneerAgent, "auctioneer");

            // Environment agent created and added into MAS environment
            var envionmentAgent = new CloudEnvironment();
            env.Add(envionmentAgent, "environment");

            // try-catch loop to catch and display to the user any error which may occur whilst the MAS runs
            try
            {
                // Stopwatch started to track the run time for the auction
                var watch = System.Diagnostics.Stopwatch.StartNew();
                
                // Runs the MAS for double auction allocation of cloud resources
                env.Start();

                // After the MAS has concluded the stopwatch is stopped and the time takes is stored
                watch.Stop();
                elapsedTime = (watch.ElapsedMilliseconds/ 100.0);

                // Results window is created to show the results from running the MAS
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

        // When a user clicks the "Settings" button, the settings menu is opened and the main window is closed
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsMenu = new Settings();
            settingsMenu.Show();
            this.Close();
        }

        // Creates the user agents with bid quantites and prices set manually by the user in the settings menu
        public void UserManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numUsers; i++)
            {
                var userAgent = new CloudUser(quantities[i], prices[i]);
                environment.Add(userAgent, $"user_{i + 1:D3}");
            }
        }

        // Creates the user agents with bid quantites and prices set manually by the user in the settings menu
        public void ProviderManualGeneration(EnvironmentMas environment, int[] quantities, int[] prices)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider(quantities[i], prices[i]);
                environment.Add(providerAgent, $"provider_{i + 1:D3}");
            }
        }

        // Method ensures that user bids automatically generate random values based upon the minimum and maximum values set in the settings menu
        public void UserAutoGeneration(EnvironmentMas enviroment)
        {
            for (int i = 0; i < numUsers; i++)
            {
                var userAgent = new CloudUser();

                enviroment.Add(userAgent, $"user_{i + 1:D3}");
            }
        }

        // Method ensures that provider bids automatically generate random values based upon the minimum and maximum values set in the settings menu
        public void ProviderAutoGeneration(EnvironmentMas environment)
        {
            for (int i = 0; i < numProviders; i++)
            {
                var providerAgent = new CloudProvider();
                environment.Add(providerAgent, $"provider_{i + 1:D3}");
            }
        }
    }
}
