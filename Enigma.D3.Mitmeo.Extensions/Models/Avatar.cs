﻿using Enigma.D3.Mitmeo.Extensions.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.D3.Mitmeo.Extensions.Models
{
    public class Avatar
    {
        private static Lazy<Avatar> _instance = new Lazy<Avatar>(() => new Avatar());
        public static Avatar Current { get { return _instance.Value; } }

        private List<IBuff> _buff;

        public Avatar()
        {
            var acd = ActorCommonData.Local;
            _buff = new List<IBuff>
            {
                new ConventionOfElements(() => ActorCommonData.Local)
            };
        }

        public IBuff GetBuff(int powerSnoId)
        {
            return _buff.FirstOrDefault(x => x.PowerSnoId == powerSnoId);
        }
    }
}
