using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public enum ProviderPosition { Positive, Neutral, Negative }
    public class CloudProvider : Agent
    {
        private Random rand = new Random();

        private int supply;
        private int bidPrice;

        private ProviderPosition position;
        private string stringPosition;

        public CloudProvider(ProviderPosition providerPosition)
        {
            position = providerPosition;
            stringPosition = position.ToString();
        }

        public override void Setup()
        {
            Send("environment", $"provider {stringPosition}");
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                case "inform":
                    HandleInform(parameters);
                    break;

                default:
                    break;
            }
        }

        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            supply = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid seller {supply} {bidPrice}");
            Stop();

        }
    }
}
