using System;

using ContactMapper.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ContactMapper.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }
    }
}
