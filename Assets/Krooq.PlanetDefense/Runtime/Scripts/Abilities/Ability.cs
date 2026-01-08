using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public abstract class Ability : IAbility
    {
        protected Player Owner;
        protected object Source;

        public virtual void Init(Player owner, object source)
        {
            Owner = owner;
            Source = source;
        }

        public abstract UniTask OnGameEvent(IGameEvent gameEvent);
    }
}
