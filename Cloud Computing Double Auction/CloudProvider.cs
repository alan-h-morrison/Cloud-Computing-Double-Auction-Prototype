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
                case "end":
                    HandleEnd(parameters);
                    break;

                case "inform":
                    HandleInform(parameters);
                    break;

                case "allocate":
                    HandleAllocateRequest(parameters);
                    break;

                default:
                    break;
            }
        }
        
        private void HandleEnd(string info)
        {
            Send("environment", $"statistics {supply} {bidPrice}");
            Stop();
        }

        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            supply = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid provider {supply} {bidPrice}");
        }

        private void HandleAllocateRequest(string info)
        {
            string[] values = info.Split(' ');

            string bidder = values[0];
            int amount = Convert.ToInt32(values[1]);

            Send("auctioneer", $"give {bidder} {amount}");
        }
    }
}
