using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static int numProviders = 100;
        public static int numUsers = 100;

        public int[] userQuantites;
        public int[] userPrices;

        public int[] providerQuantities;
        public int[] providerPrices;

        public Settings()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(chkUser.IsChecked == true)
                {
                    ValidateUserFields();
                }

                if (chkProvider.IsChecked == true)
                {
                    ValidateProviderFields();
                }

                MainWindow mainMenu = new MainWindow();
                mainMenu.Show();
                this.Close();
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }

        public void ValidateUserFields()
        {
            if (!(String.IsNullOrWhiteSpace(txtUserDemands.Text)) && !((String.IsNullOrWhiteSpace(txtUserPrices.Text))))
            {
                string usrDemandRegex = "^[1-5][0-9]( [1-9][0-9])*$";

                if ((Regex.IsMatch(txtUserDemands.Text, usrDemandRegex)))
                {
                    string[] strUserQuantites = txtUserDemands.Text.Split(' ');
                    userQuantites = Array.ConvertAll(strUserQuantites, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("User demands  is not valid:- Can only be numbers and spaces");
                }

                string usrQuantityRegex = "^[1-5][0-9]( [1-9][0-9])*$";

                if ((Regex.IsMatch(txtUserPrices.Text, usrQuantityRegex)))
                {
                    string[] strUserPrices = txtUserPrices.Text.Split(' ');
                    userPrices = Array.ConvertAll(strUserPrices, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("User prices field is not valid:- Can only be numbers and spaces");
                }
            }
            else
            {
                throw new ArgumentException("Users demand quanties or price field is empty, please enter the approriate values");
            }
        }

        public void ValidateProviderFields()
        {
            if (!(String.IsNullOrWhiteSpace(txtProviderSupply.Text)) && !((String.IsNullOrWhiteSpace(txtProviderPrices.Text))))
            {
                string providerSupplyRegex = "^[1-5][0-9]( [1-9][0-9])*$";

                if ((Regex.IsMatch(txtProviderSupply.Text, providerSupplyRegex)))
                {
                    string[] strProviderQuantities = txtProviderSupply.Text.Split(' ');
                    providerQuantities = Array.ConvertAll(strProviderQuantities, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider supplies field is not valid:- Can only be numbers and spaces");
                }

                string prvPricesRegex = "^[1-5][0-9]( [1-9][0-9])*$";

                if ((Regex.IsMatch(txtProviderPrices.Text, prvPricesRegex)))
                {
                    string[] strProviderPrices = txtProviderPrices.Text.Split(' ');
                    providerPrices = Array.ConvertAll(strProviderPrices, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider prices field is not valid:- Can only be numbers and spaces");
                }
            }
            else
            {
                throw new ArgumentException("Provider supply field or price field is empty, please enter the approriate values");
            }
        }

        private static int counter = 0;
        private static readonly object lockObject = new object();
        public static void Increment()
        {
            lock (lockObject)
            {
                counter++;
            }
        }

        public static int messageCounter
        {
            get
            {
                lock (lockObject)
                {
                    return counter;
                }
            }
        }
    }
}
