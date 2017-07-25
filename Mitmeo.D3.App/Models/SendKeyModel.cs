using Enigma.D3.Mitmeo.Extensions.Consts;
using System;
using System.Collections.Generic;
using WindowsInput.Native;

namespace Mitmeo.D3.App.Models
{
    [Serializable]
    public class SendKeyModel
    {
        public List<VirtualKeyCode> AvailableKeys { get { return Misc.VitualKeys; } }

        public VirtualKeyCode Code { get; set; }
        public double Interval { get; set; }
        public int MonsterWithin { get; set; }
        public int ResourcePct { get; set; }
        public int RangeCheck { get; set; }
        public bool Selected { get; set; }
    }
}
