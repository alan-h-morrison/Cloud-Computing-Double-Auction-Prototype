using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class ProviderStatistic : Participant
    {
        public ProviderStatistic(string id, int supply, int bidPrice)
        {
            this.ID = id;
            this.Quantity = supply;
            this.BidPrice = bidPrice;
        }
    }
}
