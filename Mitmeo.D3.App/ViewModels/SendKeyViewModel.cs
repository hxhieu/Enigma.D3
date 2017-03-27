using Enigma.D3;
using Enigma.D3.Mitmeo.Extensions;
using Mitmeo.D3.App.Commands;
using Mitmeo.D3.App.Core;
using Mitmeo.D3.App.Models;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public class SendKeyViewModel : SaveableViewModel<ObservableCollection<SendKeyModel>>
    {
        private readonly List<Timer> _timers;
        private readonly InputSimulator _input;

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

        public SendKeyViewModel(string saveFileName): base(saveFileName)
        {
            _timers = new List<Timer>();
            _input = new InputSimulator();

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
            var engine = Engine.Current;
            if (engine == null) return;

            var player = ActorCommonData.Local;

            if (player == null || Configuration == null || !Configuration.Any()) return;

            foreach (var key in Configuration)
            {
                if (key.Interval <= 0) continue;

                var timer = new Timer(key.Interval);
                timer.Elapsed += (s, e) =>
                {
                    //Disable
                    if (!Enabled) return;

                    //D3 window only
                    var currentHandle = Win32Interop.GetForegroundWindow();
                    if (currentHandle != engine.Process.MainWindowHandle) return;

                    //Enough Resources
                    if (player.GetResourcePct() < key.ResourcePct) return;

                    //Monster count
                    if (!player.HasMonstersWithin(key.RangeCheck, key.MonsterWithin)) return;

                    //Now send key
                    _input.Keyboard.KeyPress(key.Code);
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
