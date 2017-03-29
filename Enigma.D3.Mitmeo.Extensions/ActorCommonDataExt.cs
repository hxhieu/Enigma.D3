using Enigma.D3.Collections;
using Enigma.D3.Enums;
using Enigma.D3.Helpers;
using Enigma.D3.Mitmeo.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enigma.D3.Mitmeo.Extensions
{
    public static class ActorCommonDataExt
    {
        private static bool IsValidMonster(this ActorCommonData acd)
        {
            return acd.x180_Hitpoints > 0.00001 &&
             (acd.x190_Flags_Is_Trail_Proxy_Etc & 1) == 0 &&
             acd.x17C_ActorType == ActorType.Monster &&
             acd.x188_TeamId == 10;
        }

        public static ActorCommonData[] GetMonsterWithin(this ActorCommonData acd, float range, bool elite = true)
        {
            var monsters = ActorCommonDataHelper.EnumerateMonsters().Where(x => x.IsValidMonster() && acd.DistanceTo(x) <= range);
            if (!elite)
            {
                monsters = monsters.Where(x => x.x0B8_MonsterQuality == MonsterQuality.Normal);
            }

            return monsters.ToArray();
        }

        public static bool HasMonstersWithin(this ActorCommonData acd, float range, int count, bool boss = true)
        {
            var monsters = ActorCommonDataHelper.EnumerateMonsters().Where(x => x.IsValidMonster() && acd.DistanceTo(x) <= range);
            if (boss && monsters.Any(x => x.x0B8_MonsterQuality == MonsterQuality.Boss)) return true;

            return monsters.Count() >= count;
        }

        public static float DistanceTo(this ActorCommonData from, ActorCommonData to)
        {
            try
            {
                float localX = from.x0D0_WorldPosX;
                float localY = from.x0D4_WorldPosY;
                float localZ = from.x0D8_WorldPosZ;

                float diffX = to.x0D0_WorldPosX - localX;
                float diffY = to.x0D4_WorldPosY - localY;
                float diffZ = to.x0D8_WorldPosZ - localZ;


                float distance = (diffX * diffX) + (diffY * diffY) + (diffZ * diffZ);

                return (float)Math.Sqrt(distance);
            }
            catch { return float.MaxValue; }

        }

        public static ResourceType GetPrimaryResourceType(this ActorCommonData acd)
        {
            return (ResourceType)Attributes.ResourceTypePrimary.GetValue(acd);
        }

        public static double GetResourceCurrent(this ActorCommonData acd)
        {
            return acd.GetAttributeValue(AttributeId.ResourceCur, (int)GetPrimaryResourceType(acd));
        }

        public static double GetResourceMax(this ActorCommonData acd)
        {
            return acd.GetAttributeValue(AttributeId.ResourceMaxTotal, (int)GetPrimaryResourceType(acd));
        }

        public static double GetResourcePct(this ActorCommonData acd)
        {
            return (GetResourceCurrent(acd) / GetResourceMax(acd)) * 100;
        }

        public static async Task<IEnumerable<ActivePower>> GetActivePowers(this ActorCommonData acd, int? powerSnoId = null, AttributeId attrId = AttributeId.AxeBadData, bool hasValue = true)
        {
            return await Task.Run(() =>
            {
                if (acd == null) return null;

                var attributes = new List<Map<int, AttributeValue>.Entry>(acd.EnumerateAttributes());

                var result = attributes.Select(x => new ActivePower
                {
                    AttrId = x.x04_Key & 0xFFF,
                    SnoId = x.x04_Key >> 12,
                    Value = x.x08_Value.Int32
                });

                if (powerSnoId.HasValue) result = result.Where(x => x.SnoId == powerSnoId);
                if (hasValue) result = result.Where(x => x.Value > 0);
                if (attrId != AttributeId.AxeBadData) result = result.Where(x => x.AttrId == (int)attrId);

                return result;
            });
        }

        public static async Task<float> GetCurrentHp(this ActorCommonData acd)
        {
            return await Task.Run(() =>
            {
                return Attributes.HitpointsCur.GetValue(acd);
            });
        }

        public static async Task<bool> HasBuff(this ActorCommonData acd, int powerSnoId, int attrId)
        {
            var activePowers = await acd.GetActivePowers(powerSnoId);
            return activePowers != null && activePowers.FirstOrDefault() != null;
        }
    }
}