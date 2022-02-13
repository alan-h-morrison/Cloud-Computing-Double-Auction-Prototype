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

        private int tempCounter = 20;

        private ProviderPosition position;
        private string stringPosition;

        public CloudProvider(ProviderPosition providerPosition)
        {
            position = providerPosition;
            stringPosition = position.ToString();

            supply = 0;
            bidPrice = 0;
        }


        public CloudProvider(ProviderPosition providerPosition, int supplyQuantity, int pricePerUnit)
        {
            position = providerPosition;
            stringPosition = position.ToString();

            supply = supplyQuantity;
            bidPrice = pricePerUnit;
        }

        public override void Setup()
        {
            Send("environment", $"provider {stringPosition} {supply} {bidPrice}");
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

                case "allocate":
                    break;

                default:
                    break;
            }
        }

        public override void ActDefault()
        {
            if (--tempCounter <= 0)
            {
                Console.WriteLine($"{Name} stopped");
                Stop();
            }
        }

        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            supply = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid provider {supply} {bidPrice}");
        }
    }
}
