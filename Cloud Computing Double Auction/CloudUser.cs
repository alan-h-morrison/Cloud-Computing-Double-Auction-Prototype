using ActressMas;
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

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            message.Parse(out string action, out string parameters);

            switch (action)
            {
                case "start":
                    HandleStart(message.Sender);
                    break;

                default:
                    break;
            }

        }

        private void HandleStart(string sender)
        {
            Console.WriteLine($"{Name} begin bid formation");
        }
    }
}
