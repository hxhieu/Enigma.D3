using Enigma.D3.Enums;
using Enigma.D3.Mitmeo.Extensions.Buffs;
using Enigma.D3.Mitmeo.Extensions.Enums;
using Enigma.D3.Mitmeo.Extensions.Models;
using Mitmeo.D3.App.UIs;
using Mitmeo.D3.App.ViewModels.Base;
using PropertyChanged;
using System.Collections.Generic;
using System.Windows;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public class BuffWatchViewModel : ViewModelBase
    {
        public bool CoE { get; set; } = true;
        public DamageType SelectedCoE { get; set; }
        public IEnumerable<DamageType> Elements { get; set; } = ConventionOfElements.GetHeroElements();
        public int CoEPosX { get; set; } = 860;
        public int CoEPosY { get; set; } = 540;

        private readonly Overlay _overlay;

        public BuffWatchViewModel()
        {
            _overlay = new Overlay();
        }

        public override void AfterDisabled()
        {
            _overlay.Clear();
            _overlay.Hide();
        }

        public override void AfterEnabled()
        {
            if (CoE)
            {
                _overlay.Add(new Alert("CoE", GetCoE), new Point(CoEPosX, CoEPosY));
            }
            _overlay.Show();
        }

        private object GetCoE()
        {
            var coe = Avatar.Current.GetBuff(Powers.Convention_PowerSno);
            var remain = coe.GetRemain(SelectedCoE.ToString(), 0);
            return remain;
        }
    }
}
