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
                    string providerID = message.Sender;
                    int supply = rand.Next(5, 15);
                    int providerBid = rand.Next(10, 50);

                    string providerContent = $"inform {supply} {providerBid}";

                    Send(providerID, providerContent);

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
                demand = rand.Next(5, 15);
                userPrice = rand.Next(10, 50);
            }

            string userContent = $"inform {demand} {userPrice}";

            Send(userID, userContent);
        }
    } 
}
