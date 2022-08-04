using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServices.Application
{
    public class Status_Indication
    {
        public enum ChargerStatus
        {
            available = 1,
            connected = 2,
            offline = 3,
            active = 4,
            abort = 5
        }

        public enum LocationStatus
        {
            Commisioned = 1,
            Uncommisioned = 2
        }
    }
}
