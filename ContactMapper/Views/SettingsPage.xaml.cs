using System;

using ContactMapper.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ContactMapper.Views
{
    public sealed partial class SettingsPage : Page
    {
        //// TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere

        public SettingsPage()
        {
            InitializeComponent();
        }

        private SettingsViewModel ViewModel
        {
            get { return DataContext as SettingsViewModel; }
        }
    }
}
