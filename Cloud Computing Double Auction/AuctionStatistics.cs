using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{

    public class AuctionStatistics
    {

        public string AdjustmentCondition { get; set; }
        public string AdjustmentReason { get; set; }
        public string WinningCondition { get; set; }
        public string WinningReason { get; set; }
        public int ProviderPricePerUnit { get; set; }
        public int TotalTradeSurplus { get; set; }
        public int UserPricePerUnit { get; set; }

        public AuctionStatistics(int userPrice, int providerPrice, int surplus, string winCondition, string winReason, string adjustCondition, string adjustReason)
        {
            UserPricePerUnit = userPrice;
            ProviderPricePerUnit = providerPrice;
            TotalTradeSurplus = surplus;
            WinningCondition = winCondition;
            WinningReason = winReason;
            AdjustmentCondition = adjustCondition;
            AdjustmentReason = adjustReason;
        }
    }
}
