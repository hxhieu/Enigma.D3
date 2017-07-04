using Enigma.D3.ApplicationModel;
using E3M = Enigma.D3.MemoryModel;
using Enigma.D3.Mitmeo.Extensions.Buffs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Enigma.D3.Mitmeo.Extensions.Models
{
    public class Avatar
    {
        private static Lazy<Avatar> _instance = new Lazy<Avatar>(() => new Avatar());
        public static Avatar Current { get { return _instance.Value; } }

        private List<IBuff> _buff;
        private E3M.MemoryContext _ctx;
        private bool _isLocalActorReady;

        private ContainerObserver<E3M.Core.ACD> GetAcdObserver()
        {
            var observer = new ContainerObserver<E3M.Core.ACD>(_ctx.DataSegment.ObjectManager.ACDManager.ActorCommonData);
            observer.Update();
            return observer;
        }

        private E3M.Core.ACD GetCommonData()
        {
            var observer = GetAcdObserver();
            var id = GetPlayerData().ACDID;
            var acd = observer.NewItems.FirstOrDefault(x => x.ID == id);
            if (acd == null)
                acd = Enigma.Memory.MemoryObjectFactory.UnsafeCreate<E3M.Core.ACD>(new Enigma.Memory.BufferMemoryReader(observer.CurrentData),
                    Array.IndexOf(observer.CurrentMapping, id) * observer.Container.ItemSize);
            return acd;
        }

        private E3M.Core.PlayerData GetPlayerData()
        {
            return _ctx.DataSegment.ObjectManager.PlayerDataManager[_ctx.DataSegment.ObjectManager.Player.LocalPlayerIndex];
        }

        public Avatar()
        {
            _buff = new List<IBuff>();

            //{
            //    new ConventionOfElements(() => ActorCommonData.Local)
            //};
        }

        public void Init(E3M.MemoryContext ctx)
        {
            _ctx = ctx;
            var timer = new Timer(1000);
            //timer.Elapsed += (s, e) =>
            //{
            //    var test = GetCorpseWithin(60);
            //    Debug.WriteLine(test.Length);
            //};

            //timer.Start();
        }

        public bool IsValid
        {
            get
            {
                var _localData = _ctx.DataSegment.LocalData;
                _localData.TakeSnapshot();

                if (_localData.Read<byte>(0) == 0xCD) // structure is being updated, everything is cleared with 0xCD ('-')
                {
                    if (!_isLocalActorReady)
                        return false;
                }
                else
                {
                    if (!_localData.IsStartUpGame)
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

        public E3M.Core.ACD[] GetMonsterWithin(float range)
        {
            if (!IsValid) return new E3M.Core.ACD[0];
            var playerAcd = GetCommonData();
            var acds = GetAcdObserver().NewItems.Where(x => x.IsMonster() && x.DistanceTo(playerAcd) <= range).ToArray();
            return acds;
        }

        public E3M.Core.ACD[] GetCorpseWithin(float range)
        {
            if (!IsValid) return new E3M.Core.ACD[0];
            var playerAcd = GetCommonData();
            var acds = GetAcdObserver().NewItems.Where(x => x.IsCorpse() && x.DistanceTo(playerAcd) <= range).ToArray();
            return acds;
        }

        public IBuff GetBuff(int powerSnoId)
        {
            return _buff.FirstOrDefault(x => x.PowerSnoId == powerSnoId);
        }

        public bool HasBuff(int powerSnoId, int attrId)
        {
            var activePowers = ActorCommonData.Local.GetActivePowers(powerSnoId);
            return activePowers != null && activePowers.FirstOrDefault() != null;
        }
    }
}
