using Krooq.Common;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public interface IAbility
    {
        void Init(Player owner, object source);
        UniTask OnGameEvent(IGameEvent gameEvent);
    }
}
