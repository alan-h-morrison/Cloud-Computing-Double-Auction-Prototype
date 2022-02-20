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
        public static List<UserStatistic> listUserDetails { get; set; }
        public static List<ProviderStatistic> listProvDetails { get; set; }

        public Random rand = new Random();
        int turnsToWait;
        bool statReceived;

        public CloudEnvironment()
        {
            turnsToWait = 10;
            listUserDetails = new List<UserStatistic>();
            listProvDetails = new List<ProviderStatistic>();
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
                Console.WriteLine($"\t[{sender}]:\n\t\t-demand quantity = {values[0]}\n\t\t-bid price (per unit): {values[1]}\n");
                turnsToWait = 5;
                statReceived = true;

                int demand = Convert.ToInt32(values[0]);
                int bid = Convert.ToInt32(values[1]);

                UserStatistic user = new UserStatistic(sender, demand, bid);

                listUserDetails.Add(user);
            }
            else if (sender.Contains("provider"))
            {
                Console.WriteLine($"\t[{sender}]:\n\t\t-supply quantity = {values[0]}\n\t\t-bid price (per unit): {values[1]}\n");
                turnsToWait = 5;
                statReceived = true;

                int supply = Convert.ToInt32(values[0]);
                int bid = Convert.ToInt32(values[1]);

                ProviderStatistic provider = new ProviderStatistic(sender, supply, bid);

                listProvDetails.Add(provider);
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
