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

        private int userCounter;
        private int providerCounter;

        private int winningUsers;
        private int winningProviders;

        private int turnsToWait;
        private bool allocation;

        private int totalProviderQuantity;
        private int totalUserQuantity;

        private int userQuantityCounter;
        private int providerQuanityCounter;

        public CloudAuctioneer()
        {
            userBids = new List<Bid>();
            providerBids = new List<Bid>();
            turnsToWait = 50;
            allocation = false;
        }

        // Act() Method defines the actions the Cloud Auctioneer agent takes in response to messages received from other agents
        public override void Act(Message message)
        {
            // Prints message received to console
            Console.WriteLine($"\t{message.Format()}");

            // Parses the message into the action and any parameters received with the action
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                // Calls HandleBid() method when a cloud provider/user submits a bid
                case "bid":
                    HandleBid(message.Sender, parameters);
                    break;

                // Calls HandleGive() method when receives cloud resources from cloud provider to be given to a cloud user
                case "give":
                    HandleGive(message.Sender, parameters);
                    break;
                
                // Calls HandlePay() method when receives payment from a cloud user to a provider for cloud resources
                case "pay":
                    HandlePay(parameters);
                    break;

                default:
                    break;
            }
        }
     
        // The action the agent takes if no message is received during a turn
        public override void ActDefault()
        {
            // If after a certain amount of turns takes place with no messages received from other and allocation has not taken place before
            // Then the cloud auctioneer begins allocation of the bids received from cloud providers and users
            if (--turnsToWait <= 0 && allocation == false)
            {
                // Begin allocation of cloud resources
                HandleAllocation();

                // Allocation has taken place therefore the allocation is set to true
                allocation = true; 
            }

            // After allocation has taken place and cloud user/providers have exchanged all cloud resources, finish the auction
            if (userQuantityCounter == 0 && providerQuanityCounter == 0 && allocation == true)
            {
                FinishAuction();
            }
        }

        // Method is called when a user/provider submits a bid
        // Bids are added to either a list of provider bids or a list of user bids
        public void HandleBid(string sender, string info)
        {
            string[] values = info.Split(' ');

            // A bid is created with the sender, the quantity being bidded and the price per unit
            var bid = new Bid(sender, Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));

            // If the sender is from a user, bids are added to a list of user bids and the user counter is incremented
            if (values[0] == "user")
            {
                userBids.Add(bid);
                userCounter++;
            }

            // If the sender is from a provider, bids are added to a list of provider bids and the provider counter is incremented
            if (values[0] == "provider")
            {
                providerBids.Add(bid);
                providerCounter++;
            }
        }

        // Method is called when a provider sends the auctioneer VMs to be sent to a user
        private void HandleGive(string sender, string info)
        {
            string[] values = info.Split(' ');

            // From the parameters of the message, the user to be sent to and the quantity of VMs to be sent is derived
            string user = values[0];
            int quantity = Convert.ToInt32(values[1]);

            // The price per unit is the last winning users bid price
            int pricePerUnit = userBids[winningUsers].BidPrice;

            // Total provider quantity to be exchanged is updated
            providerQuanityCounter = providerQuanityCounter - quantity;

            // The user is messaged the provider's id, the bid quantity and the price per VM
            Send(user, $"won {sender} {quantity} {pricePerUnit}");
        }

        // Method is called when a user sends the auctioneer payment for the VMs received
        private void HandlePay(string info)
        {
            string[] values = info.Split(' ');

            // From the parameters of the message, the provider to be sent to, the payment for the VMs and the number of VMs to be payed for is derived
            string provider = values[0];
            int payment = Convert.ToInt32(values[1]);
            int quantity = Convert.ToInt32(values[2]);

            // Providers receive payment based on the quantity give and the bid price of the last winning provider
            int providerProfit = providerBids[winningProviders].BidPrice * quantity;

            // Total user quantity to be exchanged is updated
            userQuantityCounter = userQuantityCounter - quantity;

            // The provider is messaged the payment for VMs given and the bid price per unit for each VM sold
            Send(provider, $"paid {providerProfit} {providerBids[winningProviders].BidPrice}");
        }

        // Method called when the auction has finished allocating resources
        private void FinishAuction()
        {
            // Each user is messaged about the end of the auction
            foreach(var user in userBids)
            {
                Send(user.Bidder, $"end");
            }
            
            // Each provider is messaged about the end of the auction
            foreach (var provider in providerBids)
            { 
                Send(provider.Bidder, $"end");
            }

            // Cloud auctioneer messages the cloud environment statistics about the auction
            // This includes the bid price for users and providers and the total trade surplus received by the auctioneer from participating in the auction
            int userBidPrice = userBids[winningUsers].BidPrice;
            int providerBidPrice = providerBids[winningProviders].BidPrice;
            int tradeSurplus = Math.Min(totalUserQuantity, totalProviderQuantity) * (userBidPrice - providerBidPrice);
            Send("environment", $"statistics {userBidPrice} {providerBidPrice} {tradeSurplus}");

            // The auctioneer stops operating
            Stop();
        }

        // Method called to handle the allocation of VMs 
        // Using user/provider bids to decide the auction winners and winning users/providers should send VMs to each other
        private void HandleAllocation()
        {
            // Try-Catch statment catches any error which may occur due to an unsuccessful auction
            try
            {
                // First, a winner determination method is called to deciede the winning users/providers in the auction
                WinnerDetermination();

                // If after winner determination occurs, winning user/provider are above zero, then the auction continues
                if(winUserBids.Count > 0 && winProviderBids.Count > 0)
                {
                    // Unless the number of winning provider or user bids are equal to zero, the least profitiable bids are removed
                    if (!(winUserBids.Count == 1 || winProviderBids.Count == 1))
                    {
                        winUserBids.RemoveAt(winUserBids.Count - 1);
                        winProviderBids.RemoveAt(winProviderBids.Count - 1);
                    }

                    Console.WriteLine($"\n[{Name}]: Removed Least Profitable User/Provider:- \n\t\tNo. of winning users = {winUserBids.Count} \n\t\tNo. of winning providers = {winProviderBids.Count}");

                    // User/provider bid quantities are adjusted in the event of oversupply or overdemand
                    QuantityAdjustment();

                    totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
                    totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);

                    providerQuanityCounter = totalProviderQuantity;
                    userQuantityCounter = totalUserQuantity;

                    int totalDiff = 0;

                    totalDiff = Math.Abs(totalUserQuantity - totalProviderQuantity);
                    Console.WriteLine($"\n[{Name}]: Reallocation Difference = {totalDiff}\n");

                    // Once all trading has occurred, the PriceDeterminatin(), dictates to providers how much VMs they should supply to specific users
                    while ((winProviderBids.Sum(provider => provider.BidQuantity) > 0) && (winUserBids.Sum(user => user.BidQuantity)) > 0)
                    {
                        InformProviders();
                    }
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Stop();
            }          
        }

        // Method decides the winning users/providers based on their bids
        private void WinnerDetermination()
        {
            // List of winning user/provider bids created to store winning bids
            winUserBids = new List<Bid>();
            winProviderBids = new List<Bid>();

            // Sorts the of cloud provider bids by ascending value
            providerBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));

            // Sorts the of cloud user bids by ascending value and the reversing the list to sort by descending value
            userBids.Sort((s1, s2) => s1.BidPrice.CompareTo(s2.BidPrice));
            userBids.Reverse();

            // The number of user/providers participating in the auction is derived from the number of user/provider bids
            numUsers = userBids.Count;
            numProviders = providerBids.Count;

            // If the number of users and providers participating in the auction is above zero, start winner determination
            if (numUsers > 0 && numProviders > 0)
            {
                // A nested loop is utilised to compare all user bids against all provider bids
                for (int i = 0; i < numUsers; i++)
                {
                    // User bid is added to the winning user bid list
                    winUserBids.Add(userBids[i]);
                    for (int j = 0; j < numProviders; j++)
                    {
                        // Provider bid is added to the winning user bid list
                        winProviderBids.Add(providerBids[j]);
                    
                        // Latest user/provider bid quantity
                        int userQuantity = userBids[i].BidQuantity;
                        int providerQuantity = providerBids[j].BidQuantity;

                        // If either the first condition or second condition occurs then winner allocation ends
                        if (FirstConditionBidPrice(i, j) && FirstConditionQuantity(providerQuantity))
                        {
                            winningUsers = i;
                            winningProviders = j;

                            Console.WriteLine($"\n[{Name}]: First Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
                            return;
                        }
                        else if (SecondConditionBidPrice(i, j) && SecondConditionQuantity(userQuantity))
                        {
                            winningUsers = i;
                            winningProviders = j;

                            Console.WriteLine($"\n[{Name}]: Second Condition:- \n\t\tNo. of winning users = {i + 1} \n\t\tNo. of winning providers = {j + 1}");
                            return;
                        }
                    }
                    winProviderBids.Clear();
                }
                winUserBids.Clear();
            }
        }

        // Boolean method which determines if with the current number of winning provider and user bids if the first part of the first condition is satisfied
        private bool FirstConditionBidPrice(int i, int j)
        {
            int nextUserPrice = 0;

            if (i + 1 != userBids.Count)
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

        // Boolean method which determines if with the current number of winning provider and user bids if the second part of the first condition is satisfied
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

        // Boolean method which determines if with the current number of winning provider and user bids if the first part of the second condition is satisfied
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

        // Boolean method which determines if with the current number of winning provider and user bids if the second part of the second condition is satisfied
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

        // Method adjusts quantities for winning users or winning providers in the case of oversupply or overdemand
        private void QuantityAdjustment()
        {
            int totalProviderQuantity = winProviderBids.Sum(provider => provider.BidQuantity);
            int totalUserQuantity = winUserBids.Sum(user => user.BidQuantity);
            int totalDiff = Math.Abs(totalUserQuantity - totalProviderQuantity);
            int infiniteCounter = 0;
            int randomIndex = 0;
            int randomParticipant = 0;

            Random rand = new Random();
            List<int> listNumbers = new List<int>();

            Console.WriteLine($"\n[{Name}]: Allocation Difference = {totalDiff}");

            // If the total quantity of VMs requested by winning users is larger than the total quantity of VMs offered by winning providers
            // Overdemand has occured and the quantities requested by users must be updated
            if (totalUserQuantity > totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Overdemand Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");

                // The difference between total user quantity requested and total provider quantity offered is the overdemand which must be resolved
                totalDiff = totalUserQuantity - totalProviderQuantity;

                // Creates a list of the winning users which may be selected to adjust their quantity
                List<int> possibleUsers = Enumerable.Range(0, winUserBids.Count).ToList();

                // While total difference between total bid quantities is larger than zero, continue adjusting user bid quantities
                while (totalDiff > 0)
                {
                    // Tracks if method loops indefinetly due to all user bid quantities being only 1 whilst there is still a difference between total user bid quantity and total provider bid quantity
                    infiniteCounter++;

                    // Selects a random bid from winning user bids and removes it from a list of potential user bids which can be adjusted
                    randomIndex = rand.Next(0, possibleUsers.Count);
                    randomParticipant = possibleUsers[randomIndex];

                    listNumbers.Add(randomParticipant);
                    possibleUsers.Remove(randomParticipant);

                    // If the winning user's bid quantity is above 1, the bid quantity is decremented and therefore the total difference is decremented
                    if (winUserBids[randomParticipant].BidQuantity > 1)
                    {
                        winUserBids[randomParticipant].BidQuantity = winUserBids[randomParticipant].BidQuantity - 1;
                        totalDiff--;
                        infiniteCounter = 0;
                    }

                    // When the infnite loop counter is equal to the number of winning user, the least profitable users will be removed
                    if (infiniteCounter == winUserBids.Count)
                    {
                        winUserBids.RemoveAt(winProviderBids.Count - 1);
                        totalDiff--;
                    }

                    // If each user has had their bid quantity decremented and there is still a quantity difference, the list of users to be picked randomly to decrement their bid quantity is reset
                    if (listNumbers.Count == winUserBids.Count)
                    {
                        possibleUsers = Enumerable.Range(0, winUserBids.Count).ToList();
                        listNumbers.Clear();
                    }
                }
                return;
            }
            // If the total quantity of VMs offered by winning providers is larger than the total quantity of VMs requested by winning users,
            // Oversupply has occured and the quantities provided by providers must be updated
            else if (totalUserQuantity < totalProviderQuantity)
            {
                Console.WriteLine($"\n[{Name}]: Oversupply Has Occured:- \n\t\tTotal User Quantity = {totalUserQuantity} \n\t\tTotal Provider Quantity = {totalProviderQuantity}");

                // The difference between total provider quantity offered and total user quantity requested is the oversupply which must be resolved
                totalDiff = totalProviderQuantity - totalUserQuantity;

                List<int> possibleProviders = Enumerable.Range(0, winProviderBids.Count).ToList();

                // While total difference between total bid quantities is larger than zero, continue adjusting provider bid quantities
                while (totalDiff > 0)
                {
                    // Tracks if method loops indefinetly due to all provider bid quantities being only 1 whilst there is still a difference between total user bid quantity and total provider bid quantity
                    infiniteCounter++;

                    // Creates a list of the winning providers which may be selected to adjust their quantity
                    randomIndex = rand.Next(0, possibleProviders.Count);
                    randomParticipant = possibleProviders[randomIndex];

                    listNumbers.Add(randomParticipant);
                    possibleProviders.Remove(randomParticipant);

                    // If the winning provider's bid quantity is above 1, the bid quantity is decremented and therefore the total difference is decremented
                    if (winProviderBids[randomParticipant].BidQuantity > 1)
                    {
                        winProviderBids[randomParticipant].BidQuantity = winProviderBids[randomParticipant].BidQuantity - 1;
                        infiniteCounter = 0;
                        totalDiff--;
                    }

                    // When the infnite loop counter is equal to the number of winning user, the least profitable users will be removed
                    if (infiniteCounter == winProviderBids.Count)
                    {
                        winProviderBids.RemoveAt(winProviderBids.Count - 1);
                        totalDiff--;
                    }

                    // If each provider has had their bid quantity decremented and there is still a quantity difference, the list of providers to be picked randomly to decrement their bid quantity is reset
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

        // Method tells providers how they should allocate their VMs to users
        private void InformProviders()
        {
            // a nested for loop used to match all winning user bids to winning provider bids
            for (int i = winUserBids.Count - 1; i > -1 ; i--)
            {
                for (int j = winProviderBids.Count - 1; j > - 1; j--)
                {
                    int userQuantity = winUserBids[i].BidQuantity;
                    int providerQuantity = winProviderBids[j].BidQuantity;

                    // When the bid quantity for a user/provider is 0, then they can removed from the winning users/providers list
                    if (userQuantity == 0)
                    {
                        winUserBids.RemoveAt(i);
                        break;
                    }

                    if (providerQuantity == 0)
                    {
                        winProviderBids.RemoveAt(j);
                        break;
                    }

                    if (userQuantity > providerQuantity)
                    {
                        // Users bid quantity is updated by taking away providers bid quantity and the providers bid quantity is set to 0
                        winUserBids[i].BidQuantity = userQuantity - providerQuantity;
                        winProviderBids[j].BidQuantity = 0;

                        // Winning provider is sent the user they should allocate to and the bid quantity they should allocate
                        Send(winProviderBids[j].Bidder, $"allocate {winUserBids[i].Bidder} {providerQuantity}");
                    }
                    else if (providerQuantity > userQuantity)
                    {
                        // Providers bid quantity is updated by taking away users bid quantity and the users bid quantity is set to 0
                        winProviderBids[j].BidQuantity = providerQuantity - userQuantity;
                        winUserBids[i].BidQuantity = 0;

                        // Winning provider is sent the user they should allocate to and the bid quantity they should allocate
                        Send(winProviderBids[j].Bidder, $"allocate {winUserBids[i].Bidder} {userQuantity}");
                    }
                    else
                    {
                        // Both user and providers bid quantity is set to 0
                        winUserBids[i].BidQuantity = 0;
                        winProviderBids[j].BidQuantity = 0;

                        // Winning provider is sent the user they should allocate to and the bid quantity they should allocate
                        Send(winProviderBids[j].Bidder, $"allocate {winUserBids[i].Bidder} {userQuantity}");
                    }
                }
            }
        }
    }
}