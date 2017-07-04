﻿using Enigma.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.D3.MemoryModel.Preferences
{
	public class HotkeyPreferences : MemoryObject
	{
        public Hotkey[] Hotkeys => Read<Hotkey>(0x00, 70);
	}
}
