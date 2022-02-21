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
        public static List<Participant> listUserDetails { get; set; }
        public static List<Participant> listProvDetails { get; set; }
        public static List<Participant> listWinningUsers { get; set; }
        public static List<Participant> listWinningProviders { get; set; }

        public Random rand = new Random();
        int turnsToWait;
        bool statReceived;

        public CloudEnvironment()
        {
            turnsToWait = 10;
            listUserDetails = new List<Participant>();
            listProvDetails = new List<Participant>();
            listWinningUsers = new List<Participant>();
            listWinningProviders = new List<Participant>();
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

                Participant user = new Participant(sender, demand, bid);
                listUserDetails.Add(user);

                if (won == "true")
                {
                    Participant winningUser = new Participant(sender, demand, bid, quantityReceived, totalPaid);
                    listWinningUsers.Add(winningUser);
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


                Participant provider = new Participant(sender, supply, bid);
                listProvDetails.Add(provider);

                if(won == "true")
                {
                    Participant winningProvider = new Participant(sender, supply, bid, quantityAllocated, totalReceived);
                    listWinningProviders.Add(winningProvider);
                }
            }
            else if (sender.Contains("auctioneer"))
            {

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
                demand = rand.Next(Settings.minDemand, Settings.maxDemand);
                userPrice = rand.Next(Settings.minUserPrice, Settings.maxUserPrice);
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
                supply = rand.Next(Settings.minSupply, Settings.maxSupply);
                userPrice = rand.Next(Settings.minProviderPrice, Settings.maxProviderPrice);
            }

            string providerContent = $"inform {supply} {userPrice}";

            Send(providerID, providerContent);
        }
    } 
}
