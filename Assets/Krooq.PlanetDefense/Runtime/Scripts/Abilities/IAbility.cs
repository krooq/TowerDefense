using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public interface IAbility
    {
        void Init(ISpellCaster owner, IAbilitySource source);
        UniTask OnGameEvent(IGameEvent gameEvent);
    }
}
