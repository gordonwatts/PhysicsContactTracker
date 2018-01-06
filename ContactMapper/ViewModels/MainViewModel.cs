using System;
using System.Linq;

using Caliburn.Micro;

using ContactMapper.Helpers;
using InSpireHEPAccess;

namespace ContactMapper.ViewModels
{
    public class MainViewModel : Screen
    {
        public MainViewModel()
        {
            People = new BindableCollection<ContactViewModel>();
            LoadDummyData();
        }

        private async void LoadDummyData()
        {
            // Add a simple contact (this will be "improved" when we have somethign real).
            var finder = new InSpireContactFinder();
            var me = await finder.FindContactAsync(new Uri("http://inspirehep.net/record/983968?ln=en"));
            People.Add(new ContactViewModel(me.First()));
        }

        /// <summary>
        /// List of all contacts we are tracking.
        /// </summary>
        public BindableCollection<ContactViewModel> People { get; private set; }
    }
}
