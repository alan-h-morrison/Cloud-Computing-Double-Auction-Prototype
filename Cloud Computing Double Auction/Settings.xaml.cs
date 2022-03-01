using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int[] userQuantites;
        private int[] userPrices;

        private int[] providerQuantities;
        private int[] providerPrices;

        public Settings()
        {
            InitializeComponent();

            // Displays settings in the settings window
            GetSettings();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Try-Catch loop to catch any argument exception
            try
            {
                // When user manual entry mode is selected, the entries for quantities and price per unit is validated
                // Else the min-max ranges for a user bid quantity and price is validated
                if (chkUser.IsChecked == true)
                {
                    ValidateUserFields();
                }
                else
                {
                    ValidateUserRanges();
                }

                // When provider manual entry mode is selected, the entries for quantities and price per unit is validated
                // Else the min-max ranges for a provider bid quantity and price is validated
                if (chkProvider.IsChecked == true)
                {
                    ValidateProviderFields();
                }
                else
                {
                    ValidateProviderRanges();
                }

                // Settings.settings is set to values derived from the textboxes/combo boxes in the settings window
                SaveSettings();

                MainWindow mainMenu = new MainWindow();
                mainMenu.Show();
                this.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Settings retrieved from the Settings.settings class are displayed in the settings menu
        private void GetSettings()
        {
            // Manual entry fields for user/provider quantities and prices is displayed
            txtUserQuantity.Text = Properties.Settings.Default.UserQuantities;
            txtUserPrices.Text = Properties.Settings.Default.UserPrices;
            txtProviderQuantity.Text = Properties.Settings.Default.ProviderQuantites;
            txtProviderPrices.Text = Properties.Settings.Default.ProviderPrices;

            // If manual entry mode for user is set to true, user manual entry checkbox is checked
            if(Properties.Settings.Default.ManualUser == true)
            {
                chkUser.IsChecked = true;
            }

            // If manual entry mode for provider is set to true, user manual entry checkbox is checked
            if (Properties.Settings.Default.ManualProvider == true)
            {
                chkProvider.IsChecked = true;
            }

            // Populates the number of user and provider comboboxes with numbers ranging from 5 to 100
            ObservableCollection<string> participantNum = new ObservableCollection<string>();
            for (int i = 5; i < 101; i++)
            {
                participantNum.Add($"{i}");
            }
            cmbUsers.ItemsSource = participantNum;
            cmbProviders.ItemsSource = participantNum;

            // Using Setting.settings, the number of users and providers is displayed
            cmbUsers.SelectedItem = $"{Properties.Settings.Default.NumUsers}";
            cmbProviders.SelectedItem = $"{Properties.Settings.Default.NumProviders}";

            // Populates the min/max quantity range for user and provider comboboxes with numbers ranging from 5 to 50
            ObservableCollection<string> quantityRange = new ObservableCollection<string>();
            for (int i = 5; i < 51; i++)
            {
                quantityRange.Add($"{i}");
            }    
            cmbMinUserQuan.ItemsSource = quantityRange;
            cmbMaxUserQuan.ItemsSource = quantityRange;
            cmbMinProvQuan.ItemsSource = quantityRange;
            cmbMaxProvQuan.ItemsSource = quantityRange;

            // Using Setting.settings, the min/max range for quantitiy for provider/user is displayed
            cmbMinUserQuan.SelectedItem = $"{Properties.Settings.Default.MinUserQuantity}";
            cmbMaxUserQuan.SelectedItem = $"{Properties.Settings.Default.MaxUserQuantity}";
            cmbMinProvQuan.SelectedItem = $"{Properties.Settings.Default.MinProvQuantity}";
            cmbMaxProvQuan.SelectedItem = $"{Properties.Settings.Default.MaxProvQuantity}";

            // Populates the min/max bid price range for user and provider comboboxes with numbers ranging from 10 to 150
            ObservableCollection<string> priceRange = new ObservableCollection<string>();
            for (int i = 10; i < 150; i++)
            {
                priceRange.Add($"{i}");
            }       
            cmbMinUserPrice.ItemsSource = priceRange;
            cmbMaxUserPrice.ItemsSource = priceRange;
            cmbMinProvPrice.ItemsSource = priceRange;
            cmbMaxProvPrice.ItemsSource = priceRange;

            // Using Setting.settings, the min/max range for bid price for provider/user is displayed
            cmbMinUserPrice.SelectedItem = $"{Properties.Settings.Default.MinUserPrice}";
            cmbMaxUserPrice.SelectedItem = $"{Properties.Settings.Default.MaxUserPrice}";
            cmbMinProvPrice.SelectedItem = $"{Properties.Settings.Default.MinProvPrice}";
            cmbMaxProvPrice.SelectedItem = $"{Properties.Settings.Default.MaxProvPrice}";
        }

        // Settings.settings is updated to the settings set in the settings window
        private void SaveSettings()
        {
            // User quantities/bid prices manual entry settings set to values within textboxes
            Properties.Settings.Default.UserQuantities = txtUserQuantity.Text;
            Properties.Settings.Default.UserPrices = txtUserPrices.Text;

            // Provider quantities/bid prices manual entry settings set to values within textboxes
            Properties.Settings.Default.ProviderQuantites = txtProviderQuantity.Text;
            Properties.Settings.Default.ProviderPrices = txtProviderPrices.Text;

            // If the user manual entry mode checkbox is checked then user manual entry mode is set to true
            // Else the number of users is set the number of users textbox and the user manual entry is set to false
            if (chkUser.IsChecked == true)
            {
                Properties.Settings.Default.ManualUser = true;
            }
            else if (chkUser.IsChecked == false)
            {
                Properties.Settings.Default.NumUsers = Convert.ToInt32(cmbUsers.SelectedItem);
                Properties.Settings.Default.ManualUser = false;
            }

            // If the provider manual entry mode checkbox is checked then provider manual entry mode is set to true
            // Else the number of providers is set the number of providers textbox and the provider manual entry is set to false
            if (chkProvider.IsChecked == true)
            {
                Properties.Settings.Default.ManualProvider = true;
            }
            else if(chkProvider.IsChecked == false)
            {
                Properties.Settings.Default.NumProviders = Convert.ToInt32(cmbProviders.SelectedItem);
                Properties.Settings.Default.ManualProvider = false;
            }

            // Min-Max ranges for user/provider bid quantity is set to the selected items witin the comboboxs
            Properties.Settings.Default.MinUserQuantity = Convert.ToInt32(cmbMinUserQuan.SelectedItem);
            Properties.Settings.Default.MaxUserQuantity = Convert.ToInt32(cmbMaxUserQuan.SelectedItem);
            Properties.Settings.Default.MinProvQuantity = Convert.ToInt32(cmbMinProvQuan.SelectedItem);
            Properties.Settings.Default.MaxProvQuantity = Convert.ToInt32(cmbMaxProvQuan.SelectedItem);

            // Min-Max ranges for user/provider bid price per unit is set to the selected items witin the comboboxs
            Properties.Settings.Default.MinUserPrice = Convert.ToInt32(cmbMinUserPrice.SelectedItem);
            Properties.Settings.Default.MaxUserPrice = Convert.ToInt32(cmbMaxUserPrice.SelectedItem);
            Properties.Settings.Default.MinProvPrice = Convert.ToInt32(cmbMinProvPrice.SelectedItem);
            Properties.Settings.Default.MaxProvPrice = Convert.ToInt32(cmbMaxProvQuan.SelectedItem);
        }

        // Validates the min-max range for a user bid quantities/prices
        private void ValidateUserRanges()
        {
            // Throws an argument exception if the minimum bid quantity is larger than the maximum bid 
            if (!(Convert.ToInt32(cmbMinUserQuan.SelectedItem) < Convert.ToInt32(cmbMaxUserQuan.SelectedItem)))
            {
                throw new ArgumentException("The minimum amount of quantity a given user can have must be lower than the maximum amount of quantity a user can have");
            }

            // Throws an argument exception if the minimum bid price is larger than the maximum bid price
            if (!(Convert.ToInt32(cmbMinUserPrice.SelectedItem) < Convert.ToInt32(cmbMaxUserPrice.SelectedItem)))
            {
                throw new ArgumentException("The minimum bid price for any given user can have must be lower than the maximum bid");
            }
        }

        // Validates the min-max range for a provider bid quantities/prices
        private void ValidateProviderRanges()
        {
            // Throws an argument exception if the minimum bid quantity is larger than the maximum bid quantity
            if (Convert.ToInt32(cmbMinProvQuan.SelectedItem) > Convert.ToInt32(cmbMaxProvQuan.SelectedItem))
            {
                throw new ArgumentException("The minimum amount of quantity a given provider can have must be lower than the maximum amount of quantity a provider can have");
            }

            // Throws an argument exception if the minimum bid price is larger than the maximum bid price
            if (Convert.ToInt32(cmbMinProvPrice.SelectedItem) > Convert.ToInt32(cmbMaxProvPrice.SelectedItem))
            {
                throw new ArgumentException("The minimum bid price for any given provider can have must be lower than the maximum bid");
            }
        }

        // Validates the manual entry of bid quantites/prices for users
        private void ValidateUserFields()
        {
            txtUserQuantity.Text = txtUserQuantity.Text.Trim();
            txtUserPrices.Text = txtUserPrices.Text.Trim();

            // If the either field are empty, throw an arugment exception
            if (!(String.IsNullOrWhiteSpace(txtUserQuantity.Text)) && !((String.IsNullOrWhiteSpace(txtUserPrices.Text))))
            {
                // Regex to identify that user quantities are only numbers/spaces
                string usrQuantityRegexSpaces = "[0-9 ]+";

                // Regex to idenitfy that user quantities are numbers from 1 to 100
                string usrQuantityRegexNum = "^[1-9][0-9]?$|^100";

                // Checks all of the user quantities entered manually are only numbers from 1 to 100 and white spaces otherwise, throws an argument exception
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
                    // Converts the user quantities text box into an int array
                    userQuantites = Array.ConvertAll(strUserQuantites, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("User Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                }

                // Regex to identify that user prices are only numbers/spaces
                string usrPricesRegexSpaces = "[0-9 ]+";

                // Regex to idenitfy that user prices are numbers from 1 to 100
                string usrPricesRegexNum = "^[1-9][0-9]?$|^100";

                // Checks all of the user prices entered manually are only numbers from 1 to 100 and white spaces otherwise, throws an argument exception
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

                    // Converts the user prices text box into an int array
                    userPrices = Array.ConvertAll(strUserPrices, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("User Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                }


                // The numbers of entries in user quantities field must be equal to the number of entries in user prices field
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

        private void ValidateProviderFields()
        {
            // If the either field are empty, throw an arugment exception
            if (!(String.IsNullOrWhiteSpace(txtProviderQuantity.Text)) && !((String.IsNullOrWhiteSpace(txtProviderPrices.Text))))
            {
                // Regex to identify that provider quantities are only numbers/spaces
                string proQuantityRegexSpaces = "[0-9 ]+";

                // Regex to idenitfy that provider quantities are numbers from 1 to 100
                string proQuantityRegexNum = "^[1-9][0-9]?$|^100";

                // Checks all of the provider quantities entered manually are only numbers from 1 to 100 and white spaces otherwise, throws an argument exception
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
                    // Converts the provider quantities text box into an int array
                    providerQuantities = Array.ConvertAll(strProviderQuantities, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider Quantities is not valid:- This field can only contain numbers and spaces and quantities must range from 1 to 100");
                }

                // Regex to identify that provider prices are only numbers/spaces
                string proPricesRegexSpaces = "[0-9 ]+";

                // Regex to idenitfy that provider prices are numbers from 1 to 100
                string proPricesRegexNum = "^[1-9][0-9]?$|^100";

                // Checks all of the provider prices entered manually are only numbers from 1 to 100 and white spaces otherwise, throws an argument exception
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
                    // Converts the provider prices text box into an int array
                    providerPrices = Array.ConvertAll(strProviderPrices, s => int.Parse(s));
                }
                else
                {
                    throw new ArgumentException("Provider Prices is not valid:- This field can only contain numbers and spaces and prices must range from 1 to 100");
                }

                // The numbers of entries in provider quantities field must be equal to the number of entries in provider prices field
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
    }
}
