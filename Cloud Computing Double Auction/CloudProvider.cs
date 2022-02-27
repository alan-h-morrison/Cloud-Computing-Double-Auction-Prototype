using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudProvider : Agent
    {
        private Random rand = new Random();

        private int supply;
        private int bidPrice;
        private int finalQuantity;
        private int totalPriceRecieved;
        private int utilityGained;
        private string won;

        public CloudProvider()
        {
            supply = 0;
            bidPrice = 0;
            finalQuantity = 0;
            totalPriceRecieved = 0;
            won = "false";
        }

        public CloudProvider(int supplyQuantity, int pricePerUnit)
        {
            supply = supplyQuantity;
            bidPrice = pricePerUnit;
            finalQuantity = 0;
            totalPriceRecieved = 0;
            won = "false";
        }

        public override void Setup()
        {
            Send("environment", $"provider {supply} {bidPrice}");
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

                case "paid":
                    HandlePaid(parameters);
                    break;

                default:
                    break;
            }
        }

        private void HandlePaid(string info)
        {
            string[] values = info.Split(' ');

            totalPriceRecieved = totalPriceRecieved + Convert.ToInt32(values[0]);
            utilityGained = Convert.ToInt32(values[1]) - bidPrice;
            won = "true";
        }

        private void HandleEnd(string info)
        {
            Send("environment", $"statistics {won} {supply} {bidPrice} {finalQuantity} {totalPriceRecieved} {utilityGained}");
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

            finalQuantity = finalQuantity + amount;

            Send("auctioneer", $"give {bidder} {amount}");
        }
    }
}
