using Enigma.D3;
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
                try
                {
                    var engine = Engine.Current;
                    var player = ActorCommonData.Local;
                    return engine == null || player == null ? false : true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
