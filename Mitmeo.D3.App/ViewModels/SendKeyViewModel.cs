using Enigma.D3.Mitmeo.Extensions.Consts;
using Enigma.D3.Mitmeo.Extensions.Models;
using Mitmeo.D3.App.Commands;
using Mitmeo.D3.App.Models;
using Mitmeo.D3.App.ViewModels.Base;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using WindowsInput.Native;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public class SendKeyViewModel : SaveableViewModel<ObservableCollection<SendKeyModel>>
    {
        private readonly List<Timer> _timers;

        public override ObservableCollection<SendKeyModel> Configuration { get; set; }

        private ICommand _addKeyCommand;
        public ICommand AddKeyCommand
        {
            get
            {
                if (_addKeyCommand == null)
                {
                    _addKeyCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            Configuration.Add(new SendKeyModel
                            {
                                Code = VirtualKeyCode.VK_1,
                                Interval = 1000
                            });
                            Enabled = false;
                        }
                    );
                }

                return _addKeyCommand;
            }
        }

        private ICommand _deleteKeyCommand;
        public ICommand DeleteKeyCommand
        {
            get
            {
                if (_deleteKeyCommand == null)
                {
                    _deleteKeyCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            var toRemove = new List<SendKeyModel>(Configuration.Where(x => x.Selected));
                            foreach (var rem in toRemove)
                            {
                                Configuration.Remove(rem);
                            }
                            Enabled = false;
                        }
                    );
                }

                return _deleteKeyCommand;
            }
        }

        public SendKeyViewModel(string saveFileName) : base(saveFileName)
        {
            _timers = new List<Timer>();

            if (Configuration == null)
            {
                Configuration = new ObservableCollection<SendKeyModel>
                {
                    new SendKeyModel
                    {
                        Code = VirtualKeyCode.VK_1,
                        Interval = 1000
                    }
                };
            }
        }

        public override void AfterDisabled()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
                timer.Dispose();
            }

            _timers.Clear();
        }

        public override void AfterEnabled()
        {
            if (Configuration == null || !Configuration.Any()) return;

            foreach (var key in Configuration)
            {
                if (key.Interval <= 0) continue;

                var timer = new Timer(key.Interval);
                timer.Elapsed += (s, e) =>
                {
                    if (!IsReady)
                    {
                        Enabled = false;
                        return;
                    }

                    //Disable
                    if (!Enabled) return;

                    //D3 window only
                    if (!CanSendTo(Misc.D3ProcessNames)) return;

                    //Not in town
                    //if (Avatar.Current.HasBuff(Powers.InTownBuff, (int)AttributeId.BuffIconCount0)) return;

                    //Enough Resources
                    //if (ActorCommonData.Local.GetResourcePct() < key.ResourcePct) return;

                    //Monster count
                    if (Avatar.Current.GetMonsterWithin(key.RangeCheck).Length < key.MonsterWithin) return;

                    //Now send key
                    Input.Keyboard.KeyPress(key.Code);
                };

                timer.Start();

                _timers.Add(timer);
            }
        }

        public override void Save()
        {
            base.Save();
            MessageBox.Show("Your key setup is saved.", "INFO", MessageBoxButton.OK);
        }
    }
}
