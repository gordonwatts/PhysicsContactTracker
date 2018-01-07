using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InSpireHEPAccess
{
    internal class InSpireContact : IContact
    {
        public string FirstName { get; internal set; }

        public string LastName { get; internal set; }

        public string UniqueID { get; set; }

        // So we can track where in the HEP Names database we are if need be.
        public long InspireRecordID { get; internal set; }
    }
}
