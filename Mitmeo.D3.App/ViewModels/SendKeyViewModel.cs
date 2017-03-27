using Enigma.D3;
using Enigma.D3.Mitmeo.Extensions;
using Mitmeo.Common.Utils;
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
    public class SendKeyViewModel
    {
        private readonly List<Timer> _timers;
        private readonly InputSimulator _input;
        private const string SAVE_FILE = "keys.bin";

        public ObservableCollection<SendKeyModel> Keys { get; set; }

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
                            Keys.Add(new SendKeyModel
                            {
                                Code = VirtualKeyCode.VK_1,
                                Interval = 1000
                            });
                            Disable();
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
                            var toRemove = new List<SendKeyModel>(Keys.Where(x => x.Selected));
                            foreach (var rem in toRemove)
                            {
                                Keys.Remove(rem);
                            }
                            Disable();
                        }
                    );
                }

                return _deleteKeyCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            var serialise = new Serialiser();
                            serialise.WriteToBinaryFile(SAVE_FILE, Keys.ToArray());
                            MessageBox.Show("Your key setup is saved.", "INFO", MessageBoxButton.OK);
                        }
                    );
                }

                return _saveCommand;
            }
        }

        private ICommand _loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            Keys = new ObservableCollection<SendKeyModel>(LoadSaveFile());
                        }
                    );
                }

                return _loadCommand;
            }
        }

        public bool Enabled { get; set; }

        public SendKeyViewModel()
        {
            _timers = new List<Timer>();
            _input = new InputSimulator();

            Keys = new ObservableCollection<SendKeyModel>(LoadSaveFile());
        }

        public void Disable()
        {
            if (!Enabled) Clear();
            Enabled = false;
        }

        public void Clear()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
                timer.Dispose();
            }

            _timers.Clear();
        }

        public void Run()
        {
            var engine = Engine.Current;
            if (engine == null) return;

            var player = ActorCommonData.Local;

            if (player == null || Keys == null || !Keys.Any()) return;

            foreach (var key in Keys)
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

        private IEnumerable<SendKeyModel> LoadSaveFile(bool useDefault = true)
        {
            Disable();

            var savedKeys = new Serialiser().ReadFromBinaryFile<SendKeyModel[]>(SAVE_FILE);
            if (savedKeys == null && useDefault)
            {
                savedKeys = new SendKeyModel[]
                {
                    new SendKeyModel
                    {
                        Code = VirtualKeyCode.VK_1,
                        Interval = 1000
                    },
                    new SendKeyModel
                    {
                        Code = VirtualKeyCode.VK_2,
                        Interval = 1000
                    }
                };
            }

            return savedKeys;
        }
    }
}
