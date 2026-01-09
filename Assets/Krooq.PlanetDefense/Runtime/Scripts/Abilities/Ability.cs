using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public abstract class Ability : IAbility
    {
        protected ICaster Owner;
        protected IAbilitySource Source;

        public virtual void Init(ICaster owner, IAbilitySource source)
        {
            Owner = owner;
            Source = source;
        }

        public abstract UniTask OnGameEvent(IGameEvent gameEvent);
        protected Player Player => Owner as Player;
    }
}
