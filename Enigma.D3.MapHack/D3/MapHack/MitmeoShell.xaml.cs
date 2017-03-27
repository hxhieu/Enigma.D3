using Enigma.D3.MapHack.Markers;
using Mitmeo.D3.App.Commands;
using Mitmeo.D3.App.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Enigma.D3.MapHack
{
    public partial class MitmeoShell : Window, INotifyPropertyChanged
    {
        private const double SHELL_MINIMISE = 82;
        private const double SHELL_MAXIMISE = double.PositiveInfinity;

        private static readonly Lazy<MitmeoShell> _lazyInstance = new Lazy<MitmeoShell>(() => new MitmeoShell());

        public static MitmeoShell Instance { get { return _lazyInstance.Value; } }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool _isAttached;

        public SendKeyViewModel SendKeys { get; set; }

        public MapMarkerOptions Options { get; private set; }
        public bool IsAttached { get { return _isAttached; } set { if (_isAttached != value) { _isAttached = value; Refresh("IsAttached"); } } }

        private double _shellHeight = SHELL_MAXIMISE;
        public double ShellHeight
        {
            get { return _shellHeight; }
            set { _shellHeight = value; Refresh("ShellHeight"); }
        }

        private string _toggleButtonText = "-";
        public string ToggleButtonText
        {
            get { return _toggleButtonText; }
            set { _toggleButtonText = value; Refresh("ToggleButtonText"); }
        }

        private ICommand _toggleSizeCommand;
        public ICommand ToggleSizeCommand
        {
            get
            {
                if (_toggleSizeCommand == null)
                {
                    _toggleSizeCommand = new RelayCommand(x => true, x =>
                    {
                        if (ShellHeight <= SHELL_MINIMISE)
                        {
                            ShellHeight = SHELL_MAXIMISE;
                            ToggleButtonText = "-";
                        }
                        else
                        {
                            ShellHeight = SHELL_MINIMISE;
                            ToggleButtonText = "+";
                        }

                    });
                }

                return _toggleSizeCommand;
            }
        }

        public MitmeoShell()
        {
            Options = MapMarkerOptions.Instance;
            DataContext = this;
            InitializeComponent();
        }

        public void Init(Engine engine = null)
        {
            SendKeys = new SendKeyViewModel("keys.db");
        }

        private void Refresh(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
