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

        private List<Bid> winUserBids;
        private List<Bid> winProviderBids;

        private int numUsers;
        private int numProviders;

        int turnsToWait;

        public CloudAuctioneer()
        {
            userBids = new List<Bid>();
            providerBids = new List<Bid>();
            turnsToWait = 5;
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
            winUserBids = new List<Bid>();
            winProviderBids = new List<Bid>();

            try
            {
                // sort provider bid list by ascending value and the reversing the list to sort by descending value
                providerBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));

                // sort buyer bidding list by descending value and the reversing the list to sort by descending value
                userBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));
                userBids.Reverse();

                numUsers = userBids.Count;
                numProviders = providerBids.Count;

                if (numUsers > 0 && numProviders > 0)
                {
                    for (int i = 0; i < numUsers; i++)
                    {
                        winUserBids.Add(userBids[i]);
                        for (int j = 0; j < numProviders - 1; j++)
                        {
                            winProviderBids.Add(providerBids[i]);

                            

                            if(FirstConditionBidPrice(i,j))
                            {
                                Console.WriteLine($"First Condition:- \n\tNo. of winning users = {i + 1} \n\tNo. of winning providers = {j + 1}");
                            }
                        }
                        winProviderBids.Clear();
                    }
                }

                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        private bool FirstConditionBidPrice(int i, int j)
        {
            if ((userBids[i].BidPrice >= providerBids[j].BidPrice) && (providerBids[j].BidPrice >= userBids[i + 1].BidPrice))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
        private bool FirstConditionQuantity(int i, int j)
        {
            if()
            {

            }
            else
            {

            }
        }
        */

        public void PricingDetermination()
        {

        }
    }
}