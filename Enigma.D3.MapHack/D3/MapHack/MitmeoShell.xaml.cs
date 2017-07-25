﻿using Enigma.D3.MapHack.Markers;
using Enigma.D3.MemoryModel;
using Enigma.D3.Mitmeo.Extensions.Models;
using Mitmeo.D3.App.Commands;
using Mitmeo.D3.App.ViewModels;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Input;

namespace Enigma.D3.MapHack
{
    [ImplementPropertyChanged]
    public partial class MitmeoShell : Window
    {
        private const double SHELL_MINIMISE = 72;
        private const double SHELL_MAXIMISE = double.PositiveInfinity;

        private static readonly Lazy<MitmeoShell> _lazyInstance = new Lazy<MitmeoShell>(() => new MitmeoShell());
        public static MitmeoShell Instance { get { return _lazyInstance.Value; } }

        public SendKeyViewModel SendKeys { get; private set; }
        public BuffWatchViewModel BuffWatcher { get; private set; }
        public MapMarkerOptions Options { get; private set; }
        public MiscViewModel Misc { get; private set; }
        public bool IsAttached { get; set; }
        public double ShellHeight { get; private set; } = SHELL_MAXIMISE;
        public string ToggleButtonText { get; private set; } = "-";
        public string TestText { get; private set; }

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
            SendKeys = new SendKeyViewModel("keys.db");
            BuffWatcher = new BuffWatchViewModel();
            Misc = new MiscViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void Start(MemoryContext ctx)
        {
            IsAttached = true;
            Avatar.Current.Init(ctx);
        }

        public void Stop()
        {
            IsAttached = false;
        }
    }
}
