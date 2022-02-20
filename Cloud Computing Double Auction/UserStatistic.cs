﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class UserStatistic : Participant
    {
        public UserStatistic(string id, int demand, int bidPrice)
        {
            this.ID = id;
            this.Quantity = demand;
            this.BidPrice = bidPrice;
        }
    }
}
