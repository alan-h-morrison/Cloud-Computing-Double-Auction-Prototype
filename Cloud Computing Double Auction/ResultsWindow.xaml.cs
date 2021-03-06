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
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(double time)
        {
            InitializeComponent();

            // Display the time taken to run the auction in milliseconds
            lblTime.Text = $"{time}s";

            // Statistics from double auction are displayed in appropriate sections of the results window
            DisplayStatistics();

        }

        // The button "<<<" returns a user back to the main window
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainMenu = new MainWindow();
            mainMenu.Show();
            this.Close();
        }

        private void DisplayStatistics()    
        {
            // From the cloud environment, store list for all of the user/provider bids and data about the winning users/providers
            var userData = CloudEnvironment.ListUserDetails;
            var provData = CloudEnvironment.ListProvDetails;
            var winningUserData = CloudEnvironment.ListWinningUsers;
            var winningProviderData = CloudEnvironment.ListWinningProviders;

            // Displays the initial bids of all Users/Providers in appropriate data grids
            ProcessData(userData, dgInitUserData);
            ProcessData(provData, dgInitProvData);

            // When the double auction is succesful, display additional insights about it and the participants within it
            if (winningUserData.Count != 0 || winningProviderData.Count != 0)
            {
                double totalUsers = Properties.Settings.Default.NumUsers;
                double totalProviders = Properties.Settings.Default.NumProviders;

                // Displays the winning users/providers in appropriate data grids
                ProcessData(winningUserData, dgWinningUsers);
                ProcessData(winningProviderData, dgWinningProv);

                // The number of winning of users/providers is displayed in textboxes
                lblWinningUsers.Text = winningUserData.Count().ToString();
                lblWinningProviders.Text = winningProviderData.Count().ToString();

                // The percentage of winning participants compared to all participants is displayed in a textbox
                double percentWinning = (winningProviderData.Count() + winningUserData.Count) / (totalUsers + totalProviders);
                lblPercentWinning.Text = String.Format("{0:P1}", percentWinning);

                // Total amount of participants, users and providers who participated is displayed in appropriate textboxes
                lblNumUsers.Text = totalUsers.ToString();
                lblNumProviders.Text = totalProviders.ToString();
                lblTotal.Text = (totalUsers + totalProviders).ToString();

                // Changes textbox to show that the auction has been successful
                lblStatus.Text = "Success";
                lblStatus.Foreground = Brushes.Green;
                lblStatus.TextDecorations = TextDecorations.Underline;

                // Displays the total trade surplus
                lblSurplus.Text = CloudEnvironment.AuctionStats.TotalTradeSurplus.ToString();

                // Displays the price per unit for winning providers and users
                lblUserUnitPrice.Text = CloudEnvironment.AuctionStats.UserPricePerUnit.ToString();
                lblProvUnitPrice.Text = CloudEnvironment.AuctionStats.ProviderPricePerUnit.ToString();

                test1.Text = CloudEnvironment.AuctionStats.WinningCondition;
                test2.Text = CloudEnvironment.AuctionStats.WinningReason;
                test3.Text = CloudEnvironment.AuctionStats.AdjustmentCondition;
                test4.Text = CloudEnvironment.AuctionStats.AdjustmentReason;


                double totalUserUtility = 0;
                double totalUserQuantity = 0;
                double totalUserFinalQuantity = 0;
                int totalRequested= 0;
                int totalReceived = 0;

                // Calculates for winning users, the total quantity of VMs requested, total quantity of VMs received, the total utility generated, the total quantity demanded and total final providers received
                foreach (var user in winningUserData)
                {
                    totalRequested = totalRequested + user.Quantity;
                    totalReceived = totalReceived + user.FinalQuantity;
                    totalUserUtility = totalUserUtility + user.TotalUtility;
                    totalUserQuantity = totalUserQuantity + user.Quantity;
                    totalUserFinalQuantity = totalUserFinalQuantity + user.FinalQuantity;
                }
                // Displays the total quantity of VMs requested
                lblTotalRequested.Text = totalRequested.ToString();

                // Displays the total quantity of VMs received
                lblTotalReceived.Text = totalReceived.ToString();

                // Displays the total utilty generated by winning users
                lblTotalUserUtility.Text = totalUserUtility.ToString();

                // Displays the average utility gained by winning users
                double averageUserUtility = Math.Round(totalUserUtility / winningUserData.Count(), 1);
                lblAverageUserUtilty.Text = averageUserUtility.ToString();

                // Displays the percentage of users who won in the auction compared to total number of user who participated
                double percentWinningUser = winningUserData.Count() / totalUsers;
                lblPercentUsers.Text = String.Format("{0:P1}", percentWinningUser);

                // Displays the percentage of total quantity received compared to total quantitied requested
                double percentUserQuantity = totalUserFinalQuantity / totalUserQuantity;
                lblPercentUserQuantity.Text = String.Format("{0:P1}", percentUserQuantity);

                // Calculates for winning providers, the total quantity of VMs offered, total quantity of VMs given, the total utility generated, the total quantity offered and total final providers given
                double totalProvUtilit = 0;
                double totalProvQuantity = 0;
                double totalProvFinalQuantity = 0;
                int totalOffered = 0;
                int totalGiven = 0;

                foreach (var provider in winningProviderData)
                {
                    totalOffered = totalOffered + provider.Quantity;
                    totalGiven = totalGiven + provider.FinalQuantity;
                    totalProvUtilit = totalProvUtilit + provider.TotalUtility;
                    totalProvQuantity = totalProvQuantity + provider.Quantity;
                    totalProvFinalQuantity = totalProvFinalQuantity + provider.FinalQuantity;                
                }

                // Displays the total quantity of VMs offered
                lblTotalOffered.Text = totalOffered.ToString();

                // Displays the total quantity of VMs given
                lblTotalGiven.Text = totalGiven.ToString();

                // Displays the total utilty generated by winning providers
                lblTotalProvUtilty.Text = totalProvUtilit.ToString();
           
                // Displays the average utility gained by winning providers
                double averageProvUtility = Math.Round(totalProvUtilit / winningProviderData.Count(), 1);
                lblAverageProvUtilty.Text = averageProvUtility.ToString();

                // Displays the percentage of providers who won in the auction compared to total number of providers who participated
                double percentWinningProvider = winningProviderData.Count() / totalProviders;
                lblPercentProviders.Text = String.Format("{0:P1}", percentWinningProvider);

                // Displays the percentage of total quantity offered compared to total quantitied given
                double percentProviderQuantity = totalProvFinalQuantity / totalProvQuantity;
                lblPercentProviderQuantity.Text = String.Format("{0:P1}", percentProviderQuantity);

                
            }
        }

        // Processes a list of participants to display data in a data grid 
        private void ProcessData(List<Participant> data, DataGrid dataGrid)
        {
            var listParticpants = new ObservableCollection<Participant>();

            // Data is stored numerically based on the number of a participants id 
            data = data.OrderBy(x => x.ID.Substring(3)).ToList();

            // Each participant in a list of participants is stored in a observable collection
            foreach (var participant in data)
            {
                listParticpants.Add(participant);
            }

            // The data grid source is set to this observable collection containing the list of participants
            dataGrid.ItemsSource = listParticpants;
        } 
    }
}
