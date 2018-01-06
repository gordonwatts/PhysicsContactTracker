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
    }
}
