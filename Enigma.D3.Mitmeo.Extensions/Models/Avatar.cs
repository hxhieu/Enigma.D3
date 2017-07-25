using Enigma.D3.MemoryModel;
using Enigma.D3.MemoryModel.Caching;
using Enigma.D3.MemoryModel.Core;
using Enigma.D3.Mitmeo.Extensions.Buffs;
using Enigma.D3.Mitmeo.Extensions.Models.AvatarHeroClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.D3.Mitmeo.Extensions.Models
{
    /// <summary>
    /// TODO: Fix with new Memory Model
    /// </summary>
    public class Avatar
    {
        private static Lazy<Avatar> _instance = new Lazy<Avatar>(() => new Avatar());
        public static Avatar Current { get { return _instance.Value; } }

        private List<IBuff> _buff;
        public MemoryContext Context { get; private set; }
        private bool _isLocalActorReady;
        private int _previousFrame;
        private Dictionary<string, AvatarHeroClassBase> _heroClassInstances;

        public ContainerCache<ACD> GetAcdObserver()
        {
            var observer = new ContainerCache<ACD>(Context.DataSegment.ObjectManager.ACDManager.ActorCommonData);
            observer.Update();
            return observer;
        }

        public ACD GetCommonData()
        {
            var observer = GetAcdObserver();
            var id = GetPlayerData().ACDID;
            var acd = observer.NewItems.FirstOrDefault(x => x.ID == id);
            return acd;
        }

        public PlayerData GetPlayerData()
        {
            return Context.DataSegment.ObjectManager.PlayerDataManager[Context.DataSegment.ObjectManager.Player.LocalPlayerIndex];
        }

        public Avatar()
        {
            _heroClassInstances = new Dictionary<string, AvatarHeroClassBase>();
            _buff = new List<IBuff>();

            //{
            //    new ConventionOfElements(() => ActorCommonData.Local)
            //};
        }

        public T As<T>() where T : AvatarHeroClassBase, new()
        {
            var typeName = typeof(T).FullName;

            if (_heroClassInstances.ContainsKey(typeName))
            {
                return _heroClassInstances[typeName] as T;
            }
            else
            {
                var instance = new T();
                instance.Init(this);
                _heroClassInstances.Add(typeName, instance);
                return instance;
            }
        }

        public void Init(MemoryContext ctx)
        {
            Context = ctx;
            //var timer = new Timer(2000);
            //timer.Elapsed += (s, e) =>
            //{
            //    var necro = As<Necromancer>();
            //    Debug.WriteLine(necro.GetCorpseWithin().Length);
            //};

            //timer.Start();
        }

        public bool IsValid
        {
            get
            {
                // Don't do anything unless game updated frame.
                // Lesser frame than before = left game probably.
                //int currentFrame = Context.DataSegment.ObjectManager.RenderTick;
                //if (currentFrame <= _previousFrame)
                //    return false;

                //_previousFrame = currentFrame;

                var localData = Context.DataSegment.LocalData;
                localData.TakeSnapshot();

                if (localData.Read<byte>(0) == 0xCD) // structure is being updated, everything is cleared with 0xCD ('-')
                {
                    if (!_isLocalActorReady)
                        return false;
                }
                else
                {
                    if (!localData.IsStartUpGame)
                    {
                        if (!_isLocalActorReady)
                        {
                            _isLocalActorReady = true;
                        }
                    }
                    else
                    {
                        if (_isLocalActorReady)
                        {
                            _isLocalActorReady = false;
                        }
                        return false;
                    }
                }
                return true;
            }
        }

        public ACD[] GetMonsterWithin(float range)
        {
            if (!IsValid) return new ACD[0];
            var playerAcd = GetCommonData();
            var acds = GetAcdObserver().NewItems.Where(x => x.IsMonster() && x.DistanceTo(playerAcd) <= range).ToArray();
            return acds;
        }

        public float GetHealthPercent()
        {
            return GetPlayerData().LifePercentage;
        }
    }
}
