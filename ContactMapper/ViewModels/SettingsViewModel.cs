using System;
using System.Windows.Input;

using Caliburn.Micro;

using ContactMapper.Helpers;
using ContactMapper.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace ContactMapper.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public Visibility FeedbackLinkVisibility => Visibility.Collapsed; //Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported() ? Visibility.Visible : Visibility.Collapsed;

        public async void LaunchFeedbackHub()
        {
            //TODO: figure out why this isn't working correctly.
            // This launcher is part of the Store Services SDK https://docs.microsoft.com/en-us/windows/uwp/monetize/microsoft-store-services-sdk
            //var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            //await launcher.LaunchAsync();
        }

        // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public async void SwitchTheme(ElementTheme theme)
        {
            await ThemeSelectorService.SetThemeAsync(theme);
        }

        public SettingsViewModel()
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
