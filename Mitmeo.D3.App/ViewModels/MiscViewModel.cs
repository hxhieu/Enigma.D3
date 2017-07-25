using Enigma.D3.Mitmeo.Extensions.Consts;
using Enigma.D3.Mitmeo.Extensions.Models;
using Enigma.D3.Mitmeo.Extensions.Models.AvatarHeroClass;
using Mitmeo.D3.App.Commands;
using Mitmeo.D3.App.ViewModels.Base;
using PropertyChanged;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using WindowsInput.Native;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public class MiscViewModel : ViewModelBase
    {
        public bool NecroAutoMage { get; set; } = true;
        public VirtualKeyCode NecroAutoMageKey { get; set; } = VirtualKeyCode.VK_2;
        public int NecroAutoMageInterval { get; set; } = 500;
        public List<VirtualKeyCode> AvailableKeys { get { return Misc.VitualKeys; } }

        private Timer _necroAutoMageTimer;

        private ICommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            ApplyNecroAutoMage();
                        }
                    );
                }

                return _applyCommand;
            }
        }

        public MiscViewModel()
        {
            ApplyCommand.Execute(this);
        }

        private void ApplyNecroAutoMage()
        {
            if (_necroAutoMageTimer == null)
            {
                _necroAutoMageTimer = new Timer();
            }

            _necroAutoMageTimer.Stop();
            _necroAutoMageTimer.Elapsed -= NecroAutoMageTimer_Elapsed;

            if (NecroAutoMage && NecroAutoMageInterval > 0)
            {
                _necroAutoMageTimer.Interval = NecroAutoMageInterval;
                _necroAutoMageTimer.Elapsed += NecroAutoMageTimer_Elapsed;
                _necroAutoMageTimer.Start();
            }
        }

        private void NecroAutoMageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!CanSendTo(Misc.D3ProcessNames)) return;

            if (Avatar.Current.GetHealthPercent() < 0.25) return;

            if (Avatar.Current.GetMonsterWithin(50).Length == 0) return;

            var necro = Avatar.Current.As<Necromancer>();
            if (necro.GetSkeletonMages().Length > 9) return;

            Input.Keyboard.KeyPress(NecroAutoMageKey);
        }

        public override void AfterDisabled()
        {

        }

        public override void AfterEnabled()
        {

        }
    }
}
