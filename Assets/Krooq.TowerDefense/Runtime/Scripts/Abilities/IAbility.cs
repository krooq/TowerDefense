using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.TowerDefense
{
    public interface IAbility
    {
        void Init(ICaster owner, IAbilitySource source);
        UniTask OnGameEvent(IGameEvent gameEvent);
    }
}
