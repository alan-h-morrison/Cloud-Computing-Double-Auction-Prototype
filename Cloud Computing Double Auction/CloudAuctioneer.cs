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

        private int numUsers;
        private int numProviders;

        int turnsToWait;

        public CloudAuctioneer()
        {
            userBids = new List<Bid>();
            providerBids = new List<Bid>();
            turnsToWait = 10;
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            message.Parse(out string action, out List<string> parameters);

            switch (action)
            {
                case "bid":
                    HandleBid(message.Sender, parameters);
                    break;

                default:
                    break;
            }
        }

        public override void ActDefault()
        {
            if (--turnsToWait <= 0)
            {
                HandleAllocation();
            }
        }

        public void HandleBid(string sender, List<string> info)
        {
            var bid = new Bid(sender, Convert.ToInt32(info[1]), Convert.ToInt32(info[2]));

            if (info[0] == "user")
            {
                userBids.Add(bid);
            }

            if (info[0] == "provider")
            {
                providerBids.Add(bid);
            }
        }

        public void HandleAllocation()
        {
            WinnerDetermination();
        }

        public void WinnerDetermination()
        {
            // sort seller bidding list by asceding value and the reversing the list to sort by descending value
            providerBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));

            // sort buyer bidding list by descending value and the reversing the list to sort by descending value
            userBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));
            userBids.Reverse();

            numUsers = userBids.Count;
            numProviders = providerBids.Count;

            if (numUsers > 0 && numProviders > 0)
            {
                for (int i = numUsers - 1; i >= 0; i--)
                {
                    for (int j = numProviders - 1; j >= 0; j--)
                    {

                    }
                }
            }
        }

        public void PricingDetermination()
        {

        }
    }
}
        
    

