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

            winUserBids.RemoveAt(winUserBids.Count - 1);
            winProviderBids.RemoveAt(winProviderBids.Count - 1);

            Console.WriteLine($"[{Name}]: Removed Least Profitable User/Provider:- \n\t\tNo. of winning users = {winUserBids.Count} \n\t\tNo. of winning providers = {winProviderBids.Count}");

            PricingDetermination();

            Stop();
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
                            winProviderBids.Add(providerBids[j]);

                            int userQuantity = userBids[i].BidAmount;
                            int providerQuantity = providerBids[j].BidAmount;



                            if (FirstConditionBidPrice(i, j) && FirstConditionQuantity(providerQuantity))
                            {
                                Console.WriteLine($"[{Name}]: First Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
                                return;
                            }
                            else if (SecondConditionBidPrice(i, j) && SecondConditionQuantity(userQuantity))
                            {
                                Console.WriteLine($"[{Name}]: Second Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
                                return;
                            }
                        }
                        winProviderBids.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void PricingDetermination()
        {
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidAmount);
            int totalUserQuantity = winUserBids.Sum(user => user.BidAmount);

            if(totalUserQuantity > totalProviderQuantity)
            {
                Console.WriteLine($"[{Name}]: Overdemand has occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");

            }
            else if (totalUserQuantity < totalProviderQuantity)
            {
                Console.WriteLine($"[{Name}]: Oversupply has occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
            }
            else
            {
                Console.WriteLine($"[{Name}]: Neither Overdemand or Oversupply has occured");
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

        
        private bool FirstConditionQuantity(int providerQuantity)
        {
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidAmount);
            int totalUserQuantity = winUserBids.Sum(user => user.BidAmount);

            int previousProviderQuantity = totalProviderQuantity - providerQuantity;

            if (previousProviderQuantity <= totalUserQuantity && totalUserQuantity <= totalProviderQuantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool SecondConditionBidPrice(int i, int j)
        {
            if (providerBids[j + 1].BidPrice >= userBids[i].BidPrice && userBids[i].BidPrice >= providerBids[j].BidPrice)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SecondConditionQuantity(int userQuantity)
        {
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidAmount);
            int totalUserQuantity = winUserBids.Sum(user => user.BidAmount);

            int previousUserQuantity = totalUserQuantity - userQuantity;

            if (previousUserQuantity <= totalProviderQuantity && totalProviderQuantity <= totalUserQuantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}