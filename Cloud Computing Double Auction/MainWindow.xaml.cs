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

            for (int i = 1; i <= Settings.numProviders; i++)
            {
                var providerAgent = new CloudProvider(ProviderPosition.Positive);
                env.Add(providerAgent, $"provider{i:D2}");
            }

            for (int i = 1; i <= Settings.numUsers; i++)
            {
                var userAgent = new CloudUser(UserPosition.Positive);
                env.Add(userAgent, $"user{i:D2}");
            }

            var auctioneerAgent = new CloudAuctioneer();
            env.Add(auctioneerAgent, "auctioneer");

            var envionmentAgent = new CloudEnvironment();
            env.Add(envionmentAgent, "environment");

            env.Start();

        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsMenu = new Settings();
            settingsMenu.Show();
            this.Close();
        }
    }
}
