using PropertyChanged;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class ViewModelBase
    {
        public bool Enabled { get; set; }

        public abstract void AfterDisabled();
        public abstract void AfterEnabled();
    }
}
