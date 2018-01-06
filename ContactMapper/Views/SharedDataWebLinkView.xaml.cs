using ContactMapper.ViewModels;

namespace ContactMapper.Views
{
    public sealed partial class SharedDataWebLinkView
    {
        public SharedDataWebLinkView()
        {
            InitializeComponent();
        }

        public SharedDataWebLinkViewModel ViewModel => DataContext as SharedDataWebLinkViewModel;
    }
}
