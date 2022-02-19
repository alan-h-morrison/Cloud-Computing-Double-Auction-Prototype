using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class UserStatistic
    {
        public String UserID { get; set; }
        public int Demand { get; set; }
        public int BidPrice { get; set; }

        public UserStatistic(string id, int quantity, int price)
        {
            UserID = id;
            Demand = quantity;
            BidPrice = price;
        }
    }
}
