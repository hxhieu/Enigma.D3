using System.Threading.Tasks;

namespace Enigma.D3.Mitmeo.Extensions.Buffs
{
    public interface IBuff
    {
        int PowerSnoId { get; }
        string Name { get; }
        Task<float> GetRemain();
        Task<T> GetCurrent<T>();
    }
}
