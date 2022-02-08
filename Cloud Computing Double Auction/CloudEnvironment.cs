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
        private Random rand = new Random();
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
                    string userID = message.Sender;
                    int demand = rand.Next(5, 5);
                    int userBid = rand.Next(10, 50);

                    string userContent = $"inform {demand} {userBid}";

                    Send(userID, userContent);

                    break;

                case "provider":
                    string providerID = message.Sender;
                    int supply = rand.Next(5, 5);
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

    }
}
