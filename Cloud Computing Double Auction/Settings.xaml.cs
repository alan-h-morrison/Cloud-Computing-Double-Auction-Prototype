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
        public static int minDemand = 5;
        public static int maxDemand = 15;

        public static int minSupply = 5;
        public static int maxSupply = 15;

        public static int minUserPrice = 10;
        public static int maxUserPrice = 50;

        public static int minProviderPrice = 10;
        public static int maxProviderPrice = 50;

        public int[] userQuantites;
        public int[] userPrices;

        public int[] providerQuantities;
        public int[] providerPrices;

        public Settings()
        {
            InitializeComponent();
            GetSettings();
        }

        private void GetSettings()
        {
            txtUserQuantity.Text = Properties.Settings.Default.UserQuantities;
            txtUserPrices.Text = Properties.Settings.Default.UserPrices;
            txtProviderQuantity.Text = Properties.Settings.Default.ProviderQuantites;
            txtProviderPrices.Text = Properties.Settings.Default.ProviderPrices;

            if(Properties.Settings.Default.ManualUser == true)
            {
                chkUser.IsChecked = true;
            }

            if(Properties.Settings.Default.ManualProvider == true)
            {
                chkProvider.IsChecked = true;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.UserQuantities = txtUserQuantity.Text;
            Properties.Settings.Default.UserPrices = txtUserPrices.Text;
            Properties.Settings.Default.ProviderQuantites = txtProviderQuantity.Text;
            Properties.Settings.Default.ProviderPrices = txtProviderPrices.Text;

            if (chkUser.IsChecked == true)
            {
                Properties.Settings.Default.ManualUser = true;
            }
            else if (chkProvider.IsChecked == false)
            {
                Properties.Settings.Default.ManualUser = false;
            }

            if (chkProvider.IsChecked == true)
            {
                Properties.Settings.Default.ManualProvider = true;
            }
            else if(chkProvider.IsChecked == false)
            {
                Properties.Settings.Default.ManualProvider = false;
            }
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

                SaveSettings();

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
            txtUserQuantity.Text = txtUserQuantity.Text.Trim();
            txtUserPrices.Text = txtUserPrices.Text.Trim();

            if (!(String.IsNullOrWhiteSpace(txtUserQuantity.Text)) && !((String.IsNullOrWhiteSpace(txtUserPrices.Text))))
            {
                string usrQuantityRegexSpaces = "[0-9 ]+";
                string usrQuantityRegexNum = "^[1-9][0-9]?$|^100";

                if (Regex.IsMatch(txtUserQuantity.Text, usrQuantityRegexSpaces))
                {
  
                    string[] strUserQuantites = txtUserQuantity.Text.Split(' ');

                    foreach(string item in strUserQuantites)
                    {
                        string entry = item.Trim();

                        if(!(Regex.IsMatch(entry, usrQuantityRegexNum)))
                        {
                            throw new ArgumentException("User Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                        }
                    }

                    userQuantites = Array.ConvertAll(strUserQuantites, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("User Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                }

                string usrPricesRegexSpaces = "[0-9 ]+";
                string usrPricesRegexNum = "^[1-9][0-9]?$|^100";

                if ((Regex.IsMatch(txtUserPrices.Text, usrPricesRegexSpaces)))
                {
                    string[] strUserPrices = txtUserPrices.Text.Split(' ');

                    foreach (string item in strUserPrices)
                    {
                        string entry = item.Trim();

                        if (!(Regex.IsMatch(entry, usrPricesRegexNum)))
                        {
                            throw new ArgumentException("User Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                        }
                    }

                    userPrices = Array.ConvertAll(strUserPrices, s => int.Parse(s));

                }
                else
                {
                    throw new ArgumentException("User Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                }


                if (userQuantites.Count() == userPrices.Count())
                {
                    Properties.Settings.Default.NumUsers = userPrices.Count();
                }
                else
                {
                    throw new ArgumentException($"There are an uneven amount of user quantity and price entries: \n\tNumber of User Quantites: {userQuantites.Count()}\n\tNumber of User Prices: {userPrices.Count()}");
                }
            }
            else
            {
                throw new ArgumentException("Users Quantity/Prices field is empty, please enter the approriate values");
            }
        }

        public void ValidateProviderFields()
        {
            if (!(String.IsNullOrWhiteSpace(txtProviderQuantity.Text)) && !((String.IsNullOrWhiteSpace(txtProviderPrices.Text))))
            {
                string proQuantityRegexSpaces = "[0-9 ]+";
                string proQuantityRegexNum = "^[1-9][0-9]?$|^100";

                if ((Regex.IsMatch(txtProviderQuantity.Text, proQuantityRegexSpaces)))
                {
                    string[] strProviderQuantities = txtProviderQuantity.Text.Split(' ');

                    foreach (string item in strProviderQuantities)
                    {
                        string entry = item.Trim();

                        if (!(Regex.IsMatch(entry, proQuantityRegexNum)))
                        {
                            throw new ArgumentException("Provider Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                        }
                    }
                    providerQuantities = Array.ConvertAll(strProviderQuantities, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                }

                string proPricesRegexSpaces = "[0-9 ]+";
                string proPricesRegexNum = "^[1-9][0-9]?$|^100";

                if ((Regex.IsMatch(txtProviderPrices.Text, proPricesRegexSpaces)))
                {
                    string[] strProviderPrices = txtProviderPrices.Text.Split(' ');

                    foreach (string item in strProviderPrices)
                    {
                        string entry = item.Trim();

                        if (!(Regex.IsMatch(entry, proPricesRegexNum)))
                        {
                            throw new ArgumentException("Provider Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                        }
                    }

                    providerPrices = Array.ConvertAll(strProviderPrices, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                }

                if (providerQuantities.Count() == providerPrices.Count())
                {
                    Properties.Settings.Default.NumProviders = providerPrices.Count();
                }
                else
                {
                    throw new ArgumentException($"There are an uneven amount of provider quantity and price entries: \n\tNumber of User Quantites: {providerQuantities.Count()}\n\tNumber of User Prices: {providerPrices.Count()}");
                }
            }
            else
            {
                throw new ArgumentException("Provider Quantity field or price field is empty, please enter the approriate values");
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
