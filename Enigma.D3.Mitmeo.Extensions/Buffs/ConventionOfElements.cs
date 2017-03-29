using Enigma.D3.Enums;
using Enigma.D3.Mitmeo.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<T> GetCurrent<T>()
        {
            return await Task.Run(async () =>
            {
                var current = string.Empty;

                foreach (var element in _elementBuffAttributes)
                {
                    var powers = await _getAcd().GetActivePowers(Powers.Convention_PowerSno, element.Value);
                    if (powers == null) return default(T);

                    var buff = powers.FirstOrDefault();
                    if (buff != null)
                    {
                        current = element.Key.ToString();
                        break;
                    }
                }

                return (T)Convert.ChangeType(current, typeof(T));
            });
        }

        public async Task<float> GetRemain()
        {
            return await Task.FromResult(0);
        }
    }
}
