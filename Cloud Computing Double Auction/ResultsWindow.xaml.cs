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
        public ResultsWindow()
        {
            InitializeComponent();

            DisplayParticipants();
            
        }

        public void DisplayParticipants()
        {
            var listUsers = new ObservableCollection<Participant>();
            var listProviders = new ObservableCollection<Participant>();
            var listWinningUsers = new ObservableCollection<Participant>();
            var listWinningProviders = new ObservableCollection<Participant>();

            var userData = CloudEnvironment.listUserDetails;
            var provData = CloudEnvironment.listProvDetails;
            var winningUserData = CloudEnvironment.listWinningUsers;
            var winningProviderData = CloudEnvironment.listWinningProviders;

            userData = userData.OrderBy(x => x.ID.Substring(3)).ToList();
            provData = provData.OrderBy(x => x.ID.Substring(3)).ToList();
            winningUserData = winningUserData.OrderBy(x => x.ID.Substring(3)).ToList();
            winningProviderData = winningProviderData.OrderBy(x => x.ID.Substring(3)).ToList();


            foreach (var provider in provData)
            {
                listProviders.Add(provider);
            }

            foreach (var user in userData)
            {
                listUsers.Add(user);
            }

            foreach(var winningUser in winningUserData)
            {
                listWinningUsers.Add(winningUser);
            }

            foreach(var winningProvider in winningProviderData)
            {
                listWinningProviders.Add(winningProvider);
            }

            dgInitUserData.ItemsSource = listUsers;
            dgInitProvData.ItemsSource = listProviders;
            dgWinningUsers.ItemsSource = listWinningUsers;
            dgWinningProv.ItemsSource = listWinningProviders;
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainMenu = new MainWindow();
            mainMenu.Show();
            this.Close();
        }
    }
}
