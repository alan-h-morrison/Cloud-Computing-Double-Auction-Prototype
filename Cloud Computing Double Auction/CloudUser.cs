﻿using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class CloudUser : Agent
    {
        public CloudUser()
        {

        }

        public override void Setup()
        {
            Console.WriteLine($"{Name} says hello");
            Stop();
        }
    }
}
