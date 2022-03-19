using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudUser : Agent
    {
        private Random rand = new Random();

        private int demand;
        private int bidPrice;
        private int finalQuantity;
        private int totalPricePaid;
        private int utilityGained;
        private int totalUtility;
        private string won;

        public CloudUser()
        {
            demand = 0;
            bidPrice = 0;
            finalQuantity = 0;
            totalPricePaid = 0;
            totalUtility = 0;
            won = "false";
    }

        // A contructor for when demand quantity and price per unit are entered manually
        public CloudUser(int demandQuantity, int pricePerUnit)
        {
            demand = demandQuantity;
            bidPrice = pricePerUnit;
            finalQuantity = 0;
            totalPricePaid = 0;
            totalUtility = 0;
            won = "false";
        }

        // At the start of the MAS, users message the environment to inform them of demand quantity and price per unit
        public override void Setup()
        {
            Send("environment", $"user {demand} {bidPrice}");
        }

        // Act() Method defines the actions a Cloud User agent takes in response to messages received from other agents
        public override void Act(Message message)
        {
            try
            {
                // Prints message received to console
                Console.WriteLine($"\t{message.Format()}");

                // Parses the message into the action and any parameters received with the action
                message.Parse(out string action, out string parameters);

                switch (action)
                {
                    // Calls HandleEnd() method when informed by the auctioneer that the auction has concluded
                    case "end":
                        HandleEnd(parameters);
                        break;

                    // Calls HandleInform() method when the environment informs them of their statistics used to form the bid submitted to the auctioneer
                    case "inform":
                        HandleInform(parameters);
                        break;

                    // Calls HandleWin() method when the user has won some quantity of VMs
                    case "won":
                        HandleWin(parameters);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Method is used to message environment of a user's statistics at the end of the auction
        private void HandleEnd(string info)
        {
            Send("environment", $"statistics {won} {demand} {bidPrice} {finalQuantity} {totalPricePaid} {utilityGained} {totalUtility}");
            Stop();
        }

        // Method is called to formulate a user's bid and submit it to the cloud auctioneer
        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            demand = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid user {demand} {bidPrice}");
        }

        // Method is called when a user has won some amount of VMs from participating in the auction and sends the appropriate amount to pay for the amount received
        private void HandleWin(string info)
        {
            string[] values = info.Split(' ');

            string provider = values[0];
            int amount = Convert.ToInt32(values[1]);
            int totalPrice = amount * Convert.ToInt32(values[2]);

            utilityGained = bidPrice - Convert.ToInt32(values[2]);
            won = "true";
            finalQuantity = finalQuantity + amount;
            totalPricePaid = totalPricePaid + totalPrice;
            totalUtility = totalUtility + (utilityGained * amount);

            Send("auctioneer", $"pay {provider} {totalPricePaid} {amount}");
        }
    }
}
