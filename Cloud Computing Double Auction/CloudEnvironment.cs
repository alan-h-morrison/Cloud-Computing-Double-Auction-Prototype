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
        public static List<Participant> ListUserDetails { get; set; }
        public static List<Participant> ListProvDetails { get; set; }
        public static List<Participant> ListWinningUsers { get; set; }
        public static List<Participant> ListWinningProviders { get; set; }

        private int minUserQuantity = Properties.Settings.Default.MinUserQuantity;
        private int maxUserQuantity = Properties.Settings.Default.MaxUserQuantity + 1;

        private int minUserPrice = Properties.Settings.Default.MinUserPrice;
        private int maxUserPrice = Properties.Settings.Default.MaxUserPrice + 1;

        private int minProvQuantity = Properties.Settings.Default.MinProvQuantity;
        private int maxProvQuantity = Properties.Settings.Default.MaxProvQuantity + 1;

        private int minProviderPrice = Properties.Settings.Default.MinProvPrice;
        private int maxProviderPrice = Properties.Settings.Default.MaxProvPrice + 1;

        public Random rand = new Random();
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

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");

            message.Parse(out string action, out string parameters);

            switch (action)
            {
                case "user":
                    HandleUser(message.Sender, parameters);
                    break;

                case "provider":
                    HandleProvider(message.Sender, parameters);
                    break;

                case "statistics":
                    HandleStatistics(message.Sender, parameters);
                    break;

                default:
                    break;
            }
        }

        public override void ActDefault()
        {
            if (--turnsToWait <= 0 && statReceived == true)
            {
                Stop();
            }
        }

        private void HandleStatistics(string sender, string info)
        {
            string[] values = info.Split(' ');

            if (sender.Contains("user"))
            {
                turnsToWait = 5;
                statReceived = true;

                string won = values[0];
                int demand = Convert.ToInt32(values[1]);
                int bid = Convert.ToInt32(values[2]);
                int quantityReceived = Convert.ToInt32(values[3]);
                int totalPaid = Convert.ToInt32(values[4]);
                int utilityGained = Convert.ToInt32(values[5]);

                Participant user = new Participant(sender, demand, bid);
                ListUserDetails.Add(user);

                if (won == "true")
                {
                    Participant winningUser = new Participant(sender, demand, bid, quantityReceived, totalPaid, utilityGained);
                    ListWinningUsers.Add(winningUser);
                }
            }
            else if (sender.Contains("provider"))
            {
                turnsToWait = 5;
                statReceived = true;

                string won = values[0];
                int supply = Convert.ToInt32(values[1]);
                int bid = Convert.ToInt32(values[2]);
                int quantityAllocated = Convert.ToInt32(values[3]);
                int totalReceived = Convert.ToInt32(values[4]);
                int utilityGained = Convert.ToInt32(values[5]);

                Participant provider = new Participant(sender, supply, bid);
                ListProvDetails.Add(provider);

                if(won == "true")
                {
                    Participant winningProvider = new Participant(sender, supply, bid, quantityAllocated, totalReceived, utilityGained);
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

        private void HandleUser(string user, string info)
        {
            string[] values = info.Split(' ');

            string userID = user;

            int demand = Convert.ToInt32(values[1]);
            int userPrice = Convert.ToInt32(values[2]);

            if(demand == 0 && userPrice == 0)
            {
                demand = rand.Next(minUserQuantity, maxUserQuantity);
                userPrice = rand.Next(minUserPrice, maxUserPrice);
            }

            string userContent = $"inform {demand} {userPrice}";

            Send(userID, userContent);
        }

        private void HandleProvider(string provider, string info)
        {
            string[] values = info.Split(' ');

            string providerID = provider;

            int supply = Convert.ToInt32(values[1]);
            int userPrice = Convert.ToInt32(values[2]);

            if (supply == 0 && userPrice == 0)
            {
                supply = rand.Next(minProvQuantity, maxProvQuantity);
                userPrice = rand.Next(minProviderPrice, maxProviderPrice);
            }

            string providerContent = $"inform {supply} {userPrice}";

            Send(providerID, providerContent);
        }
    } 
}
