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
using System.Windows.Shapes;

namespace Cloud_Computing_Double_Auction
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static int numProviders = 10;
        public static int numUsers = 10;

        public int[] userQuantites;

        private static int counter = 0;
        private static readonly object lockObject = new object();

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
                    if (!(String.IsNullOrWhiteSpace(txtUserDemands.Text)))
                    {
                        string[] strUserQuantites = txtUserDemands.Text.Split(',');
                        userQuantites = Array.ConvertAll(strUserQuantites, s => int.Parse(s));
                    }
                    else
                    {
                        throw new ArgumentException("User demands field is empty, please enter approriate values");
                    }
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
