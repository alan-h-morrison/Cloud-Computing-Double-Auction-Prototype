using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{

    public class AuctionStatistics
    {
        public int UserPricePerUnit { get; set; }
        public int ProviderPricePerUnit { get; set; }
        public int TotalTradeSurplus { get; set; }

        public AuctionStatistics(int userPrice, int providerPrice, int surplus)
        {
            UserPricePerUnit = userPrice;
            ProviderPricePerUnit = providerPrice;
            TotalTradeSurplus = surplus;
        }

    }
}
