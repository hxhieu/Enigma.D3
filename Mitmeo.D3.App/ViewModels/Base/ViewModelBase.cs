using Enigma.D3.Mitmeo.Extensions.Models;
using Mitmeo.D3.App.Core;
using PropertyChanged;
using System;
using System.Diagnostics;
using System.Linq;
using WindowsInput;

namespace Mitmeo.D3.App.ViewModels.Base
{
    [ImplementPropertyChanged]
    public abstract class ViewModelBase
    {
        public bool Enabled { get; set; }
        protected readonly InputSimulator Input = new InputSimulator();

        public abstract void AfterDisabled();
        public abstract void AfterEnabled();

        protected bool CanSendTo(params string[] processNames)
        {
            IntPtr _processHandle = IntPtr.Zero;
            foreach (var name in processNames)
            {
                var process = Process.GetProcessesByName(name).FirstOrDefault();
                if (process != null)
                {
                    _processHandle = process.MainWindowHandle;
                    break;
                }
            }

            var currentHandle = Win32Interop.GetForegroundWindow();
            return currentHandle == _processHandle;
        }

        protected bool IsReady
        {
            get
            {
                return Avatar.Current.IsValid;
            }
        }
    }
}
