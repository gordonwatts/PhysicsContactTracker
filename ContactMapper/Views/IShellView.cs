using System;

using Caliburn.Micro;

namespace ContactMapper.Views
{
    public interface IShellView
    {
        INavigationService CreateNavigationService(WinRTContainer container);
    }
}
