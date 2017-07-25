namespace Enigma.D3.Mitmeo.Extensions.Models.AvatarHeroClass
{
    public abstract class AvatarHeroClassBase
    {
        protected Avatar Avatar { get; set; }
        public void Init(Avatar avatar)
        {
            Avatar = avatar;
        }
    }
}
