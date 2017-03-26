using System.Collections.Generic;
using WindowsInput.Native;

namespace Mitmeo.D3.App.Models
{
    public class SendKeyModel
    {
        private List<VirtualKeyCode> _availableKeys = new List<VirtualKeyCode>
        {
            VirtualKeyCode.VK_1,
            VirtualKeyCode.VK_2,
            VirtualKeyCode.VK_3,
            VirtualKeyCode.VK_4,
            VirtualKeyCode.VK_Q,
            VirtualKeyCode.VK_W,
            VirtualKeyCode.VK_E,
            VirtualKeyCode.VK_R,
            VirtualKeyCode.SPACE,
            VirtualKeyCode.TAB
        };

        public List<VirtualKeyCode> AvailableKeys { get { return _availableKeys; } }

        public VirtualKeyCode Code { get; set; }
        public double Interval { get; set; }
        public int MonsterWithin { get; set; }
        public int ResourcePct { get; set; }
        public int RangeCheck { get; set; }
        public bool Selected { get; set; }
    }
}
