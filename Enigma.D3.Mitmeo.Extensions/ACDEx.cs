using Enigma.D3.Enums;
using Enigma.D3.MemoryModel.Core;

namespace Enigma.D3.Mitmeo.Extensions
{
    public static class ACDEx
    {
        public static bool IsMonster(this ACD acd)
        {
            return acd.ActorType == ActorType.Monster && acd.TeamID == 10;
        }

        public static float DistanceTo(this ACD acd, ACD target)
        {
            var localX = acd.Position.X;
            var localY = acd.Position.Y;
            var localZ = acd.Position.Z;

            var diffX = target.Position.X - localX;
            var diffY = target.Position.Y - localY;
            var diffZ = target.Position.Z - localZ;

            var distance = (float)System.Math.Sqrt((diffX * diffX) + (diffY * diffY) + (diffZ * diffZ));

            return distance;
        }

        public static bool IsCorpse(this ACD acd)
        {
            return acd.ActorSNO == 454066;
        }
    }
}
