using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class Bid
    {
        public string Bidder { get; set; }
        public int BidQuantity { get; set; }
        public int BidPrice { get; set; }

        public Bid(string bidder, int quantity, int price)
        {
            Bidder = bidder;
            BidQuantity = quantity;
            BidPrice = price;
        }
    }
}
