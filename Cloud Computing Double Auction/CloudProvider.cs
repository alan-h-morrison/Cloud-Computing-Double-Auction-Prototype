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
        private int totalUtility;
        private string won;

        public CloudProvider()
        {
            supply = 0;
            bidPrice = 0;
            finalQuantity = 0;
            totalPriceRecieved = 0;
            totalUtility = 0;
            won = "false";
        }

        // A contructor for when supply quantity and price per unit are entered manually
        public CloudProvider(int supplyQuantity, int pricePerUnit)
        {
            supply = supplyQuantity;
            bidPrice = pricePerUnit;
            finalQuantity = 0;
            totalPriceRecieved = 0;
            totalUtility = 0;
            won = "false";
        }

        // At the start of the MAS, providers message the environment to inform them of demand quantity and price per unit
        public override void Setup()
        {
            Send("environment", $"provider {supply} {bidPrice}");
        }

        // Act() Method defines the actions a Cloud User agent takes in response to messages received from other agents
        public override void Act(Message message)
        {
            // Prints message received to console
            Console.WriteLine($"\t{message.Format()}");

            // Parses the message into the action and any parameters received with the action
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                case "allocate":
                    HandleAllocateRequest(parameters);
                    break;

                // Calls HandleEnd() method when informed by the auctioneer that the auction has concluded
                case "end":
                    HandleEnd(parameters);
                    break;

                // Calls HandleInform() method when the environment informs them of their statistics used to form the bid submitted to the auctioneer
                case "inform":
                    HandleInform(parameters);
                    break;


                // Calls HandlePaid() method when they have been paid by the auctioneer for VMs which were given to a user
                case "paid":
                    HandlePaid(parameters);
                    break;

                default:
                    break;
            }
        }

        // Method is used to handle a request from the auctioneer to allocate VMs to a user
        private void HandleAllocateRequest(string info)
        {
            string[] values = info.Split(' ');

            string user = values[0];
            int amount = Convert.ToInt32(values[1]);

            finalQuantity = finalQuantity + amount;

            Send("auctioneer", $"give {user} {amount}");
        }

        // Method is used to message environment of a provider's statistics at the end of the auction
        private void HandleEnd(string info)
        {
            totalUtility = finalQuantity * utilityGained;

            Send("environment", $"statistics {won} {supply} {bidPrice} {finalQuantity} {totalPriceRecieved} {utilityGained} {totalUtility}");
            Stop();
        }

        // Method is called to formulate a provider's bid and submit it to the cloud auctioneer
        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            supply = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid provider {supply} {bidPrice}");
        }

        // Method is called to handle payment received from the auctioneer for VMs allocated to a user
        private void HandlePaid(string info)
        {
            string[] values = info.Split(' ');

            totalPriceRecieved = totalPriceRecieved + Convert.ToInt32(values[0]);
            utilityGained = Convert.ToInt32(values[1]) - bidPrice;
            won = "true";
        }
    }
}
