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
                    case "end":
                        HandleEnd(parameters);
                            break;
                    case "inform":
                        HandleInform(parameters);
                        break;

                    case "won":
                        HandleWin(parameters);
                        break;

                    case "paid":
                        HandlePayment(parameters);
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

        private void HandleEnd(string info)
        {
            Send("environment", $"statistics {demand} {bidPrice}");
            Stop();
        }

        private void HandleInform(string info)
        {
            string[] values = info.Split(' ');

            demand = Int32.Parse(values[0]);
            bidPrice = Int32.Parse(values[1]);

            Send("auctioneer", $"bid user {demand} {bidPrice}");
        }

        private void HandleWin(string info)
        {
            string[] values = info.Split(' ');

            string provider = values[0];
            int amount = Convert.ToInt32(values[1]);
            int pricePerUnit = Convert.ToInt32(values[2]);

            int totalPrice = amount * pricePerUnit;

            Send("auctioneer", $"pay {provider} {totalPrice} {amount}");
        }

        private void HandlePayment(string str)
        {
            int payment = Convert.ToInt32(str);
        }

    }
}
