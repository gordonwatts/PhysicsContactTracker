using Caliburn.Micro;
using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactMapper.ViewModels
{
    /// <summary>
    /// All information for a contact
    /// </summary>
    public class ContactViewModel : Screen
    {
        /// <summary>
        /// Contact we will be showing.
        /// </summary>
        private readonly IContact _contact;

        public ContactViewModel(IContact contact)
        {
            _contact = contact;
        }

        /// <summary>
        /// Return the name for easy display in a view.
        /// </summary>
        public string ContactName => $"{_contact.LastName}, {_contact.FirstName}";
    }
}
