﻿using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudAuctioneer : Agent
    {
        public CloudAuctioneer()
        {

        }

        public override void Setup()
        {
            Console.WriteLine($"auctioneer says hello");
            Stop();
        }
    }
}
        
    

