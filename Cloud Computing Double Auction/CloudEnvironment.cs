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
        public Random rand = new Random();
        int turnsToWait;

        public CloudEnvironment()
        {
            turnsToWait = 10;
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

                default:
                    break;
            }
        }

        public override void ActDefault()
        {
            if (--turnsToWait <= 0)
            {
                Stop();
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
