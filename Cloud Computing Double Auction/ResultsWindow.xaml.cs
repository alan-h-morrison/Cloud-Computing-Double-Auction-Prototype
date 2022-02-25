using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Cloud_Computing_Double_Auction
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(double time)
        {
            InitializeComponent();

            DisplayStatistics();

            lblTime.Text = $"{time}s";
        }

        public void DisplayStatistics()    
        {
            // Display User/Provider initial bids and winning users/providers data in appropriate data grids
            var userData = CloudEnvironment.ListUserDetails;
            var provData = CloudEnvironment.ListProvDetails;
            var winningUserData = CloudEnvironment.ListWinningUsers;
            var winningProviderData = CloudEnvironment.ListWinningProviders;

            ProcessData(userData, dgInitUserData);
            ProcessData(provData, dgInitProvData);

            // If the auction is succesful
            if (winningUserData.Count != 0 || winningProviderData.Count != 0)
            {
                double totalUsers = Properties.Settings.Default.NumUsers;
                double totalProviders = Properties.Settings.Default.NumProviders;

                ProcessData(winningUserData, dgWinningUsers);
                ProcessData(winningProviderData, dgWinningProv);

                lblWinningUsers.Text = winningUserData.Count().ToString();
                lblWinningProviders.Text = winningProviderData.Count().ToString();

                double percentWinning = (winningProviderData.Count() + winningUserData.Count) / (totalUsers + totalProviders);
                lblPercentWinning.Text = String.Format("{0:P1}", percentWinning);

                lblNumUsers.Text = totalUsers.ToString();
                lblNumProviders.Text = totalProviders.ToString();
                lblTotal.Text = (totalUsers + totalProviders).ToString();

                lblStatus.Text = "Success";
                lblStatus.Foreground = Brushes.Green;
                lblStatus.TextDecorations = TextDecorations.Underline;

                lblSurplus.Text = CloudEnvironment.AuctionStats.TotalTradeSurplus.ToString();
                lblUserUnitPrice.Text = CloudEnvironment.AuctionStats.UserPricePerUnit.ToString();
                lblProvUnitPrice.Text = CloudEnvironment.AuctionStats.ProviderPricePerUnit.ToString();

                double totalUserUtility = 0;
                double totalProvUtilit = 0;
                double totalUserQuantity = 0;
                double totalUserFinalQuantity = 0;

                double totalProvQuantity = 0;
                double totalProvFinalQuantity = 0;



                foreach (var user in winningUserData)
                {
                    totalUserUtility = totalUserUtility + user.Utility;
                    totalUserQuantity = totalUserQuantity + user.Quantity;
                    totalUserFinalQuantity = totalUserFinalQuantity + user.FinalQuantity;
                }

                foreach (var provider in winningProviderData)
                {
                    totalProvUtilit = totalProvUtilit + provider.Utility;
                    totalProvQuantity = totalProvQuantity + provider.Quantity;
                    totalProvFinalQuantity = totalProvFinalQuantity + provider.FinalQuantity;
                }

                lblTotalUserUtility.Text = totalUserUtility.ToString();
                lblTotalProvUtilty.Text = totalProvUtilit.ToString();

                double averageUserUtility = Math.Round(totalUserUtility / winningUserData.Count(), 1);
                double averageProvUtility = Math.Round(totalProvUtilit / winningProviderData.Count(), 1);

                lblAverageUserUtilty.Text = averageUserUtility.ToString();
                lblAverageProvUtilty.Text = averageProvUtility.ToString();

                double percentWinningUser = winningUserData.Count() / totalUsers;
                lblPercentUsers.Text = String.Format("{0:P1}", percentWinningUser);

                double percentWinningProvider = winningProviderData.Count() / totalProviders;
                lblPercentProviders.Text = String.Format("{0:P1}", percentWinningProvider);

                double percentUserQuantity = totalUserFinalQuantity / totalUserQuantity;
                lblPercentUserQuantity.Text = String.Format("{0:P1}", percentUserQuantity);

                double percentProviderQuantity = totalProvFinalQuantity / totalProvQuantity;
                lblPercentProviderQuantity.Text = String.Format("{0:P1}", percentProviderQuantity);
            }
        }

        private void ProcessData(List<Participant> data, DataGrid dataGrid)
        {
            var listParticpants = new ObservableCollection<Participant>();

            data = data.OrderBy(x => x.ID.Substring(3)).ToList();

            foreach (var participant in data)
            {
                listParticpants.Add(participant);
            }

            dataGrid.ItemsSource = listParticpants;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainMenu = new MainWindow();
            mainMenu.Show();
            this.Close();
        }
    }
}
