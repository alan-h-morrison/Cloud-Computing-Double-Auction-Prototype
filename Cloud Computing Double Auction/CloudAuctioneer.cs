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

            Console.WriteLine($"\n[{Name}]: Removed Least Profitable User/Provider:- \n\t\tNo. of winning users = {winUserBids.Count} \n\t\tNo. of winning providers = {winProviderBids.Count}");

            AdjustQuantity();

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

                            int userQuantity = userBids[i].BidQuantity;
                            int providerQuantity = providerBids[j].BidQuantity;

                            if (FirstConditionBidPrice(i, j) && FirstConditionQuantity(providerQuantity))
                            {
                                Console.WriteLine($"\n[{Name}]: First Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
                                return;
                            }
                            else if (SecondConditionBidPrice(i, j) && SecondConditionQuantity(userQuantity))
                            {
                                Console.WriteLine($"\n[{Name}]: Second Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
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

        private void AdjustQuantity()
        {
            double totalProviderQuantity = Convert.ToDouble(winProviderBids.Sum(provider => provider.BidQuantity));
            double totalUserQuantity = Convert.ToDouble(winUserBids.Sum(user => user.BidQuantity));
            double totalDiff = 0;
            double originalQuantity = 0;
            double newQuantity = 0;

            int indivDiff = 0;

            if (totalUserQuantity > totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Overdemand Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
                totalDiff = totalUserQuantity - totalProviderQuantity;
                
                foreach (var user in winUserBids)
                {
                    if(totalDiff == 0)
                    {
                        break;
                    }

                    originalQuantity = user.BidQuantity;

                    newQuantity = Convert.ToDouble(user.BidQuantity) - ((totalUserQuantity - totalProviderQuantity) * (Convert.ToDouble(user.BidQuantity) / totalUserQuantity));
                    newQuantity = Math.Round(newQuantity, 0);

                    indivDiff = Convert.ToInt32(originalQuantity - newQuantity);

                    if (indivDiff > totalDiff)
                    {
                        newQuantity = newQuantity + (indivDiff - totalDiff);
                    }

                    user.BidQuantity = Convert.ToInt32(newQuantity);

                    totalDiff = totalDiff - (originalQuantity - newQuantity);
                }
                Console.WriteLine($"\n[{Name}]: Reallocated Quantities:- \n\t\tAllocation Difference = {totalDiff}");
                return;
            }
            else if (totalUserQuantity < totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Oversupply Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
                totalDiff = totalProviderQuantity - totalUserQuantity;

                foreach (var provider in winProviderBids)
                {
                    if (totalDiff == 0)
                    {
                        break;
                    }
                    originalQuantity = provider.BidQuantity;

                    newQuantity = Convert.ToDouble(provider.BidQuantity) - ((totalProviderQuantity - totalUserQuantity) * (Convert.ToDouble(provider.BidQuantity) / totalProviderQuantity));
                    newQuantity = Math.Round(newQuantity, 0);

                    indivDiff = Convert.ToInt32(originalQuantity - newQuantity);

                    if (indivDiff > totalDiff)
                    {
                        newQuantity = newQuantity + (indivDiff - totalDiff);
                    }

                    provider.BidQuantity = Convert.ToInt32(newQuantity);

                    totalDiff = totalDiff - (indivDiff);
                }
                Console.WriteLine($"\n[{Name}]: Reallocated Quantities:- \n\t\tAllocation Difference = {totalDiff}");
                return;
            }
            Console.WriteLine($"\n[{Name}]: No Difference Between Demand and Supply:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
            Console.WriteLine($"\n[{Name}]: No Reallocation:- \n\t\tAllocation Difference = {totalDiff}");
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
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
            int totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);

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
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
            int totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);

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