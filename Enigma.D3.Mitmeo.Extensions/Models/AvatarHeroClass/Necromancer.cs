using Enigma.D3.MemoryModel.Core;
using System.Linq;

namespace Enigma.D3.Mitmeo.Extensions.Models.AvatarHeroClass
{
    public class Necromancer : AvatarHeroClassBase
    {
        public ACD[] GetCorpseWithin(float range = 60)
        {
            if (!Avatar.IsValid) return new ACD[0];
            var playerAcd = Avatar.GetCommonData();
            var acds = Avatar.GetAcdObserver().NewItems.Where(x => x.IsCorpse() && x.DistanceTo(playerAcd) <= range).ToArray();
            return acds;
        }

        public ACD[] GetSkeletonMages()
        {
            if (!Avatar.IsValid) return new ACD[0];
            var acds = Avatar.GetAcdObserver().NewItems.Where(x => x.IsSkeletonMage()).ToArray();
            return acds;
        }
    }
}
