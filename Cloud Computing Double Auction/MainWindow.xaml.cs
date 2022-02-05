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

        private void BtnAuction_Click(object sender, RoutedEventArgs e)
        {
            var env = new EnvironmentMas();

            for (int i = 1; i <= AuctionSettings.providers; i++)
            {
                var providerAgent = new CloudProvider();
                env.Add(providerAgent, $"provider{i:D2}");
            }

            for (int i = 1; i <= AuctionSettings.users; i++)
            {
                var userAgent = new CloudUser();
                env.Add(userAgent, $"user{i:D2}");
            }

            var auctioneerAgent = new CloudAuctioneer();
            env.Add(auctioneerAgent, "auctioneer");

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
