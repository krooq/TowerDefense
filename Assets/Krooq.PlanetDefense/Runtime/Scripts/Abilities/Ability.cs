using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public abstract class Ability : IAbility
    {
        protected ISpellCaster Owner;
        protected IAbilitySource Source;

        public virtual void Init(ISpellCaster owner, IAbilitySource source)
        {
            Owner = owner;
            Source = source;
        }

        public abstract UniTask OnGameEvent(IGameEvent gameEvent);
        protected Player Player => Owner as Player;
    }
}
