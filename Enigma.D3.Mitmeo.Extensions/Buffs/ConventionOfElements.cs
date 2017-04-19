﻿using Enigma.D3.Enums;
using Enigma.D3.Mitmeo.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.D3.Mitmeo.Extensions.Buffs
{
    public class ConventionOfElements : IBuff
    {
        private static readonly Dictionary<HeroClass, DamageType[]> _classElements = new Dictionary<HeroClass, DamageType[]>
        {
            {HeroClass.Barbarian, new DamageType[] { DamageType.Cold, DamageType.Fire, DamageType.Lightning, DamageType.Physical, }},
            {HeroClass.Crusader, new DamageType[] { DamageType.Fire, DamageType.Holy, DamageType.Lightning, DamageType.Physical, }},
            {HeroClass.DemonHunter, new DamageType[] { DamageType.Cold, DamageType.Fire, DamageType.Lightning, DamageType.Physical, }},
            {HeroClass.Monk, new DamageType[] { DamageType.Cold, DamageType.Fire, DamageType.Holy, DamageType.Lightning, DamageType.Physical, }},
            {HeroClass.Witchdoctor, new DamageType[] { DamageType.Cold, DamageType.Fire, DamageType.Physical, DamageType.Poison, }},
            {HeroClass.Wizard, new DamageType[] {DamageType.Arcane, DamageType.Cold, DamageType.Fire, DamageType.Lightning}}
        };

        private static readonly Dictionary<DamageType, AttributeId> _elementBuffAttributes = new Dictionary<DamageType, AttributeId>
        {
            { DamageType.Arcane,    AttributeId.BuffIconCount1 },
            { DamageType.Cold,      AttributeId.BuffIconCount2 },
            { DamageType.Fire,      AttributeId.BuffIconCount3 },
            { DamageType.Holy,      AttributeId.BuffIconCount4 },
            { DamageType.Lightning, AttributeId.BuffIconCount5 },
            { DamageType.Physical,  AttributeId.BuffIconCount6 },
            { DamageType.Poison,    AttributeId.BuffIconCount7 }
        };

        private static readonly Dictionary<DamageType, AttributeId> _elementStartTickAttributes = new Dictionary<DamageType, AttributeId>
        {
            { DamageType.Arcane,    AttributeId.BuffIconStartTick1 },
            { DamageType.Cold,      AttributeId.BuffIconStartTick2 },
            { DamageType.Fire,      AttributeId.BuffIconStartTick3 },
            { DamageType.Holy,      AttributeId.BuffIconStartTick4 },
            { DamageType.Lightning, AttributeId.BuffIconStartTick5 },
            { DamageType.Physical,  AttributeId.BuffIconStartTick6 },
            { DamageType.Poison,    AttributeId.BuffIconStartTick7 }
        };

        private static readonly Dictionary<DamageType, AttributeId> _elementEndTickAttributes = new Dictionary<DamageType, AttributeId>
        {
            { DamageType.Arcane,    AttributeId.BuffIconEndTick1 },
            { DamageType.Cold,      AttributeId.BuffIconEndTick2 },
            { DamageType.Fire,      AttributeId.BuffIconEndTick3 },
            { DamageType.Holy,      AttributeId.BuffIconEndTick4 },
            { DamageType.Lightning, AttributeId.BuffIconEndTick5 },
            { DamageType.Physical,  AttributeId.BuffIconEndTick6 },
            { DamageType.Poison,    AttributeId.BuffIconEndTick7 }
        };

        public string Name
        {
            get { return GetType().Name; }
        }

        public int PowerSnoId
        {
            get { return Powers.Convention_PowerSno; }
        }

        private readonly Func<ActorCommonData> _getAcd;

        public ConventionOfElements(Func<ActorCommonData> getAcd)
        {
            _getAcd = getAcd;
        }

        public string GetCurrent()
        {
            string current = null;

            var powers = _getAcd().GetActivePowers(PowerSnoId);
            if (powers != null && powers.Any())
            {
                foreach (var element in _elementBuffAttributes)
                {
                    var buff = powers.FirstOrDefault(x => x.AttrId == (int)element.Value);
                    if (buff != null)
                    {
                        current = element.Key.ToString();
                        break;
                    }
                }
            }

            return current;
        }

        public double GetRemain(string buff = null, int dp = 1)
        {
            var current = (DamageType)Enum.Parse(typeof(DamageType), GetCurrent());
            var end = _getAcd().GetActivePowers(
                PowerSnoId,
                _elementEndTickAttributes[current]
            ).FirstOrDefault();

            var currentRemain = end != null ? (end.Value - Engine.Current.ObjectManager.x7B0_Storage.GetGameTick()) / 60d : 0;

            if (buff == null) return Math.Round(currentRemain, dp);
            else
            {
                var heroElements = _classElements[PlayerData.Local.GetHeroClass()].ToList();
                var type = (DamageType)Enum.Parse(typeof(DamageType), buff);

                var currentIndex = heroElements.FindIndex(x => x == current);
                var targetIndex = heroElements.FindIndex(x => x == type);

                if (currentIndex == targetIndex) return -Math.Round(currentRemain, dp);

                var step = 0;
                if (targetIndex > currentIndex)
                {
                    step = targetIndex - currentIndex;
                }
                else
                {
                    step = heroElements.Count - currentIndex + targetIndex;
                }

                return Math.Round(currentRemain + (step - 1) * 4, dp);
            }
        }
    }
}
