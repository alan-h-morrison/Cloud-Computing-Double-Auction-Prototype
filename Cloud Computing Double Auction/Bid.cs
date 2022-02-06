using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    class Bid
    {
        public string Bidder { get; set; }
        public int BidAmount { get; set; }
        public int BidPrice { get; set; }

        public Bid(string bidder, int amount, int price)
        {
            Bidder = bidder;
            BidAmount = amount;
            BidPrice = price;
        }
    }
}
