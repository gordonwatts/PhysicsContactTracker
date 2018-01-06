using ContactMapper.ViewModels;

namespace ContactMapper.Views
{
    public sealed partial class SharedDataStorageItemsView
    {
        public SharedDataStorageItemsView()
        {
            InitializeComponent();
        }

        public SharedDataStorageItemsViewModel ViewModel => DataContext as SharedDataStorageItemsViewModel;
    }
}
