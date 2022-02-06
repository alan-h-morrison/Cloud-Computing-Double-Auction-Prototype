using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudAuctioneer : Agent
    {
        private List<Bid> userBids;
        private List<Bid> providerBids;

        public CloudAuctioneer()
        {
            userBids = new List<Bid>();
            providerBids = new List<Bid>();
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                case "bid":
                    HandleBid(parameters);
                    break;

                default:
                    break;
            }
        }

        public void HandleBid(string info)
        {
            
        }
    }
}
        
    

