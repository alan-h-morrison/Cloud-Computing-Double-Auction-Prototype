using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class Participant
    {
        public string ID { get; set; }
        public int Quantity { get; set; }
        public int BidPrice { get; set; }
        public int FinalQuantity { get; set; }
        public int TotalPrice { get; set; }

        public Participant(string id, int demand, int bidPrice)
        {
            ID = id;
            Quantity = demand;
            BidPrice = bidPrice;
        }

        public Participant(string id, int demand, int bidPrice, int finalQuant, int totalPaid)
        {
            ID = id;
            Quantity = demand;
            BidPrice = bidPrice;
            FinalQuantity = finalQuant;
            TotalPrice = totalPaid;
        }
    }
}
