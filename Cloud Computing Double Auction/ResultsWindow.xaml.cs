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

            DisplayParticipants();

            lblStatus.Text = "Success";
            lblStatus.Foreground = Brushes.Green;
            lblStatus.TextDecorations = TextDecorations.Underline;

            lblTime.Text = $"{time}s";
        }

        public void DisplayParticipants()    
        {
            var userData = CloudEnvironment.listUserDetails;
            var provData = CloudEnvironment.listProvDetails;
            var winningUserData = CloudEnvironment.listWinningUsers;
            var winningProviderData = CloudEnvironment.listWinningProviders;

            DisplayData(userData, dgInitUserData);
            DisplayData(provData, dgInitProvData);

            if (winningUserData.Count != 0 || winningProviderData.Count != 0)
            {
                int totalUsers = Properties.Settings.Default.NumUsers;
                int totalProviders = Properties.Settings.Default.NumProviders;

                DisplayData(winningUserData, dgWinningUsers);
                DisplayData(winningProviderData, dgWinningProv);

                lblWinningUsers.Text = winningUserData.Count().ToString();
                lblWinningProviders.Text = winningProviderData.Count().ToString();

                lblNumUsers.Text = totalUsers.ToString();
                lblNumProviders.Text = totalProviders.ToString();
                lblTotal.Text = (totalUsers + totalProviders).ToString();

            }
        }

        private void DisplayData(List<Participant> data, DataGrid dataGrid)
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
