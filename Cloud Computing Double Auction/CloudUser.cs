using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public enum UserPosition { Positive, Neutral, Negative }
    public class CloudUser : Agent
    {
        private Random rand = new Random();

        private int demand;
        private int bidPrice;

        private UserPosition position;
        private string stringPosition;

        public CloudUser(UserPosition userPosition)
        {
            position = userPosition;
            stringPosition = position.ToString();

            demand = 0;
            bidPrice = 0;
        }

        public CloudUser(UserPosition userPosition, int demandQuantity, int pricePerUnit)
        {
            position = userPosition;
            stringPosition = position.ToString();

            demand = demandQuantity;
            bidPrice = pricePerUnit;
        }

        public override void Setup()
        {
            Send("environment", $"user {stringPosition} {demand} {bidPrice}");
        }

        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out string parameters);

                switch (action)
                {
                    case "inform":
                        HandleInform(parameters);
                        break;

                    case "send":   
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

        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            demand = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid user {demand} {bidPrice}");
        }
    }
}
