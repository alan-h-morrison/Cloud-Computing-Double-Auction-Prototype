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
            try
            {
                WinnerDetermination();

                if(winUserBids.Count == 0 || winProviderBids.Count == 0)
                {
                    Console.WriteLine("MISTAKE");
                }

                if(!(winUserBids.Count == 1 || winProviderBids.Count == 1))
                {
                    winUserBids.RemoveAt(winUserBids.Count - 1);
                    winProviderBids.RemoveAt(winProviderBids.Count - 1);
                }

                Console.WriteLine($"\n[{Name}]: Removed Least Profitable User/Provider:- \n\t\tNo. of winning users = {winUserBids.Count} \n\t\tNo. of winning providers = {winProviderBids.Count}");

                AltAdjustQuantities();
                //AdjustQuantities();

                int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
                int totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);
                int totalDiff = 0;

                totalDiff = Math.Abs(totalUserQuantity - totalProviderQuantity);
                Console.WriteLine($"\n[{Name}]: Allocation Difference = {totalDiff}");

                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Stop();
            }          
        }

        public void WinnerDetermination()
        {
            winUserBids = new List<Bid>();
            winProviderBids = new List<Bid>();

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
        

        private void AltAdjustQuantities()
        {
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
            int totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);
            int totalDiff = Math.Abs(totalUserQuantity - totalProviderQuantity);
            int counter = 0;
            int infiniteCounter = 0;

            Random rand = new Random();
            List<int> listNumbers = new List<int>();

            Console.WriteLine($"\n[{Name}]: Allocation Difference = {totalDiff}");

            if (totalUserQuantity > totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Overdemand Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
                totalDiff = totalUserQuantity - totalProviderQuantity;

                List<int> possibleUsers = Enumerable.Range(0, winUserBids.Count).ToList();

                while (counter != totalDiff)
                {
                    infiniteCounter++;

                    int randomUser = rand.Next(0, possibleUsers.Count);
                    listNumbers.Add(possibleUsers[randomUser]);
                    possibleUsers.RemoveAt(randomUser);

                    if (winUserBids[randomUser].BidQuantity > 1)
                    {
                        winUserBids[randomUser].BidQuantity = winUserBids[randomUser].BidQuantity - 1;
                        counter++;
                        infiniteCounter = 0;
                    }

                    if(infiniteCounter == winUserBids.Count)
                    {
                        Console.WriteLine("widepeepohappy");
                    }

                    if (listNumbers.Count == winUserBids.Count)
                    {
                        possibleUsers = Enumerable.Range(0, winUserBids.Count).ToList();
                        listNumbers.Clear();
                    }
                }
                return;
            }
            else if (totalUserQuantity < totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Oversupply Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");

                totalDiff = totalProviderQuantity - totalUserQuantity;

                List<int> possibleProviders = Enumerable.Range(0, winProviderBids.Count).ToList();

                while (counter < totalDiff)
                {
                    infiniteCounter++;

                    int randomProvider = rand.Next(0, possibleProviders.Count);
                    listNumbers.Add(possibleProviders[randomProvider]);
                    possibleProviders.RemoveAt(randomProvider);

                    if (winProviderBids[randomProvider].BidQuantity > 1)
                    {
                        winProviderBids[randomProvider].BidQuantity = winProviderBids[randomProvider].BidQuantity - 1;
                        counter++;
                    }

                    if (infiniteCounter == winUserBids.Count)
                    {
                        Console.WriteLine("widepeepohappy");
                    }

                    if (listNumbers.Count == winProviderBids.Count)
                    {
                        possibleProviders = Enumerable.Range(0, winProviderBids.Count).ToList();
                        listNumbers.Clear();
                    }
                }
                return;
            }
            Console.WriteLine($"\n[{Name}]: No Difference Between Demand and Supply:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
            Console.WriteLine($"\n[{Name}]: No Reallocation");
        }


        private bool FirstConditionBidPrice(int i, int j)
        {
            int nextUserPrice = 0;

            if (!(i + 1 == userBids.Count - 1))
            {
                nextUserPrice = userBids[i + 1].BidPrice;
            }

            if ((userBids[i].BidPrice >= providerBids[j].BidPrice) && (providerBids[j].BidPrice >= nextUserPrice))
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
            int nextProviderPrice = 0;

            if (!(j + 1 > providerBids.Count - 1))
            {
                nextProviderPrice = providerBids[j + 1].BidPrice;
            }

            if (nextProviderPrice >= userBids[i].BidPrice && userBids[i].BidPrice >= providerBids[j].BidPrice)
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

/*
        private void AdjustQuantities()
        {
            double totalProviderQuantity = Convert.ToDouble(winProviderBids.Sum(provider => provider.BidQuantity));
            double totalUserQuantity = Convert.ToDouble(winUserBids.Sum(user => user.BidQuantity));
            double totalDiff = 0;
            double originalQuantity = 0;
            double newQuantity = 0;

            double indivDiff = 0;

            totalDiff = Math.Abs(totalUserQuantity - totalProviderQuantity);
            Console.WriteLine($"\n[{Name}]: No Reallocation:- \n\t\tAllocation Difference = {totalDiff}");

            if (totalUserQuantity >= totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Overdemand Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
                totalDiff = totalUserQuantity - totalProviderQuantity;
                
                foreach (var user in winUserBids)
                {
                    Console.WriteLine($"[{user.Bidder}]: original quantity = {user.BidQuantity}");
                    if(totalDiff == 0)
                    {
                        break;
                    }

                    originalQuantity = user.BidQuantity;

                    newQuantity = Convert.ToDouble(user.BidQuantity) - ((totalUserQuantity - totalProviderQuantity) * (Convert.ToDouble(user.BidQuantity) / totalUserQuantity));
                    newQuantity = Math.Round(newQuantity, 0);

                    indivDiff = originalQuantity - newQuantity;

                    if (indivDiff > totalDiff)
                    {
                        newQuantity = newQuantity + (indivDiff - totalDiff);
                    }

                    user.BidQuantity = Convert.ToInt32(newQuantity);

                    totalDiff = totalDiff - (originalQuantity - newQuantity);

                    Console.WriteLine($"[{user.Bidder}]: adjusted quantity = {user.BidQuantity}");
                }
                Console.WriteLine($"\n[{Name}]: Reallocated Quantities:- \n\t\tAllocation Difference = {totalDiff}");
                return;
            }
            else if (totalUserQuantity <= totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Oversupply Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
                totalDiff = totalProviderQuantity - totalUserQuantity;

                foreach (var provider in winProviderBids)
                {
                    Console.WriteLine($"[{provider.Bidder}]: original quantity = {provider.BidQuantity}");

                    if (totalDiff == 0)
                    {
                        break;
                    }
                    originalQuantity = provider.BidQuantity;

                    newQuantity = originalQuantity - ((totalProviderQuantity - totalUserQuantity) * (originalQuantity / totalProviderQuantity));
                    newQuantity = Math.Round(newQuantity, 0);

                    indivDiff = originalQuantity - newQuantity;

                    if (indivDiff > totalDiff)
                    {
                        newQuantity = newQuantity + (indivDiff - totalDiff);
                        indivDiff = totalDiff;
                    }

                    provider.BidQuantity = Convert.ToInt32(newQuantity);

                    totalDiff = totalDiff - (indivDiff);

                    Console.WriteLine($"[{provider.Bidder}]: adjusted quantity = {provider.BidQuantity}");

                }
                Console.WriteLine($"\n[{Name}]: Reallocated Quantities:- \n\t\tAllocation Difference = {totalDiff}");

                if(totalDiff > 0)
                {
                    return;
                }

                return;
            }
            Console.WriteLine($"\n[{Name}]: No Difference Between Demand and Supply:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");
            Console.WriteLine($"\n[{Name}]: No Reallocation:- \n\t\tAllocation Difference = {totalDiff}");
        }

*/