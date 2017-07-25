using System.Collections.Generic;
using WindowsInput.Native;

namespace Enigma.D3.Mitmeo.Extensions.Consts
{
    public struct Powers
    {
        public const int InTownBuff = 220304;
        public const int DrinkHealthPotion = 30211;
        public const int RessurectPlayer = 30021;
        public const int UseStoneOfRecall = 191590;
        public const int UseDungeonStoneOfRecall = 220318;
        public const int TeleportToWaypoint = 349060;
        public const int TeleportToPlayer_1 = 371139;
        public const int TeleportToPlayer_2 = 318242;
        public const int Convention_PowerSno = 430674;
    }

    public struct AcdSNO
    {
        public const int Corpse = 454066;
        public const int SkeletonMage = 472715;
    }

    public struct Misc
    {
        public static List<VirtualKeyCode> VitualKeys = new List<VirtualKeyCode>
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

        public static string[] D3ProcessNames = new string[] { "Diablo III64", "Diablo III" };
    }
}
