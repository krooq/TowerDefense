using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public class AbilityController : MonoBehaviour, IGameEventListener
    {
        private Player _player;
        private List<IAbility> _abilities = new();

        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        public void Init(Player player)
        {
            _player = player;
            GameEventManager.AddListener(this);
            RebuildAbilities();
        }

        private void OnDestroy()
        {
            if (GameEventManager != null) GameEventManager.RemoveListener(this);
        }

        public void RebuildAbilities()
        {
            _abilities.Clear();

            // Order: Relics then Spells
            if (_player.Relics != null)
            {
                foreach (var relic in _player.Relics)
                {
                    if (relic == null) continue;
                    foreach (var def in relic.Abilities)
                    {
                        var ability = def.Create();
                        ability.Init(_player, relic);
                        _abilities.Add(ability);
                    }
                }
            }

            if (_player.Spells != null)
            {
                foreach (var spell in _player.Spells)
                {
                    if (spell == null) continue;
                    foreach (var def in spell.Abilities)
                    {
                        var ability = def.Create();
                        ability.Init(_player, spell);
                        _abilities.Add(ability);
                    }
                }
            }
        }

        public async UniTask OnGameEvent(IGameEvent gameEvent)
        {
            foreach (var ability in _abilities)
            {
                await ability.OnGameEvent(gameEvent);
            }
        }
    }
}
