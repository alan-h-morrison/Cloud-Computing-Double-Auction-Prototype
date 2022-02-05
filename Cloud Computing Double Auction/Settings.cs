using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Computing_Double_Auction
{
    public class AuctionSettings
    {
        public static int providers = 10;
        public static int users = 10;


        private static int counter = 0;
        private static readonly object lockObject = new object();


        public static void Increment()
        {
            lock (lockObject)
            {
                counter++;
            }
        }

        public static int messageCounter
        {
            get
            {
                lock (lockObject)
                {
                    return counter;
                }
            }
        }
    }
}
