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
        public ObservableCollection<UserStatistic> ListUsers { get; set; }

        public ResultsWindow()
        {
            InitializeComponent();

            ListUsers = new ObservableCollection<UserStatistic>();

            var userData = CloudEnvironment.listUserDetails;
            userData = userData.OrderBy(x => x.UserID).ToList();

            foreach (var user in userData)
            {
                ListUsers.Add(user);
            }

            dgInitUserData.ItemsSource = ListUsers;
            
            foreach(DataGridColumn column in dgInitUserData.Columns)
            {
                //if you want to size your column as per the cell content
                //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                //if you want to size your column as per the column header
                //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);
                //if you want to size your column as per both header and cell content
                 column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
        }



        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainMenu = new MainWindow();
            mainMenu.Show();
            this.Close();
        }
    }
}
