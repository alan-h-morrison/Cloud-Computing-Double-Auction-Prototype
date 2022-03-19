using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudEnvironment : Agent
    {
        public static AuctionStatistics AuctionStats { get; set; }
        public static List<Participant> ListProvDetails { get; set; }
        public static List<Participant> ListUserDetails { get; set; }
        public static List<Participant> ListWinningProviders { get; set; }
        public static List<Participant> ListWinningUsers { get; set; }

        private Random rand = new Random();
        int turnsToWait;
        bool statReceived;

        public CloudEnvironment()
        {
            turnsToWait = 10;
            ListUserDetails = new List<Participant>();
            ListProvDetails = new List<Participant>();
            ListWinningUsers = new List<Participant>();
            ListWinningProviders = new List<Participant>();
        }

        // Act() Method defines the actions the Cloud Auctioneer agent takes in response to messages received from other agents
        public override void Act(Message message)
        {
            // Prints message received to console
            Console.WriteLine($"\t{message.Format()}");

            // Parses the message into the action and any parameters received with the action
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                // Calls HandleProvider() method when a cloud provider messages to register their personal details
                case "provider":
                    HandleProvider(message.Sender, parameters);
                    break;

                // Calls HandleStatistics() method when a user/provider submits their statistics after the auction ends
                case "statistics":
                    HandleStatistics(message.Sender, parameters);
                    break;

                // Calls HandleUser() method when a cloud user messages to receive their personal details
                case "user":
                    HandleUser(message.Sender, parameters);
                    break;

                default:
                    break;
            }
        }

        // The action the agent takes if no message is received during a turn
        public override void ActDefault()
        {
            // If after a certain amount of turns takes place with no messages received and statisitcs have not been recieved
            if (--turnsToWait <= 0 && statReceived == true)
            {
                Stop();
            }
        }

        // Method is called when a provider messages to receive their personal details
        private void HandleProvider(string sender, string info)
        {
            string[] values = info.Split(' ');

            string providerID = sender;

            int supply = Convert.ToInt32(values[0]);
            int minProvQuantity = Properties.Settings.Default.MinProvQuantity;
            int maxProvQuantity = Properties.Settings.Default.MaxProvQuantity + 1;

            int providerPrice = Convert.ToInt32(values[1]);
            int minProviderPrice = Properties.Settings.Default.MinProvPrice;
            int maxProviderPrice = Properties.Settings.Default.MaxProvPrice + 1;
           
            // If their supply/price per unit has not been set already, randomly generate the values based on min/max values set for provider quantity and bid price
            if (supply == 0 && providerPrice == 0)
            {
                supply = rand.Next(minProvQuantity, maxProvQuantity);
                providerPrice = rand.Next(minProviderPrice, maxProviderPrice);
            }

            // Messages provider to inform them of their personal details
            string providerContent = $"inform {supply} {providerPrice}";
            Send(providerID, providerContent);
        }

        // Method is called when the user, provider or the auctioneer messages their statisitcs after the end of the auction
        private void HandleStatistics(string sender, string info)
        {
            string[] values = info.Split(' ');

            if (sender.Contains("user"))
            {
                // Reset turn to wait counter and sets the statistics received variable to true
                turnsToWait = 5;
                statReceived = true;

                string won = values[0];
                int demand = Convert.ToInt32(values[1]);
                int bid = Convert.ToInt32(values[2]);
                int quantityReceived = Convert.ToInt32(values[3]);
                int totalPaid = Convert.ToInt32(values[4]);
                int utilityGained = Convert.ToInt32(values[5]);
                int totalUtility = Convert.ToInt32(values[6]);

                // Adds all users into a list of participants
                Participant user = new Participant(sender, demand, bid);
                ListUserDetails.Add(user);

                // Winning users are added to a list of participants
                if (won == "true")
                {
                    Participant winningUser = new Participant(sender, demand, bid, quantityReceived, totalPaid, utilityGained, totalUtility);
                    ListWinningUsers.Add(winningUser);
                }
            }
            else if (sender.Contains("provider"))
            {
                // Reset turn to wait counter and sets the statistics received variable to true
                turnsToWait = 5;
                statReceived = true;

                string won = values[0];
                int supply = Convert.ToInt32(values[1]);
                int bid = Convert.ToInt32(values[2]);
                int quantityAllocated = Convert.ToInt32(values[3]);
                int totalReceived = Convert.ToInt32(values[4]);
                int utilityGained = Convert.ToInt32(values[5]);
                int totalUtility = Convert.ToInt32(values[6]);

                // Adds all providers into a list of participants
                Participant provider = new Participant(sender, supply, bid);
                ListProvDetails.Add(provider);

                // Winning providers are added to a list of participants
                if (won == "true")
                {
                    Participant winningProvider = new Participant(sender, supply, bid, quantityAllocated, totalReceived, utilityGained, totalUtility);
                    ListWinningProviders.Add(winningProvider);
                }
            }
            else if (sender.Contains("auctioneer"))
            {
                int userPricePerUnit = Convert.ToInt32(values[0]);
                int providerPricePerUnit = Convert.ToInt32(values[1]);
                int tradeSurplus = Convert.ToInt32(values[2]);

                AuctionStats = new AuctionStatistics(userPricePerUnit, providerPricePerUnit, tradeSurplus);
            }
        }

        // Method is called when a user messages to receive their personal details
        private void HandleUser(string sender, string info)
        {
            string[] values = info.Split(' ');

            string userID = sender;

            int demand = Convert.ToInt32(values[0]);
            int minUserQuantity = Properties.Settings.Default.MinUserQuantity;
            int maxUserQuantity = Properties.Settings.Default.MaxUserQuantity + 1;

            int userPrice = Convert.ToInt32(values[1]);
            int minUserPrice = Properties.Settings.Default.MinUserPrice;
            int maxUserPrice = Properties.Settings.Default.MaxUserPrice + 1;

            // If their supply/price per unit has not been set already, randomly generate the values based on min/max values set for user quantity and bid price
            if (demand == 0 && userPrice == 0)
            {
                demand = rand.Next(minUserQuantity, maxUserQuantity);
                userPrice = rand.Next(minUserPrice, maxUserPrice);
            }

            // Messages user to inform them of their personal details
            string userContent = $"inform {demand} {userPrice}";      
            Send(userID, userContent);
        }
    } 
}
