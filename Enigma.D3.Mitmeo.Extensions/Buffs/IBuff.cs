using System.Threading.Tasks;

namespace Enigma.D3.Mitmeo.Extensions.Buffs
{
    public interface IBuff
    {
        int PowerSnoId { get; }
        string Name { get; }
        double GetRemain(string buff = null, int dp = 1);
        string GetCurrent();
    }
}
