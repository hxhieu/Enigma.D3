using Enigma.D3.Mitmeo.Extensions.Models;
using PropertyChanged;

namespace Mitmeo.D3.App.ViewModels.Base
{
    [ImplementPropertyChanged]
    public abstract class ViewModelBase
    {
        public bool Enabled { get; set; }

        public abstract void AfterDisabled();
        public abstract void AfterEnabled();

        protected bool IsReady
        {
            get
            {
                return Avatar.Current.IsValid;
            }
        }
    }
}
