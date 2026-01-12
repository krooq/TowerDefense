using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Krooq.TowerDefense
{
    public class AbilityController : MonoBehaviour, IGameEventListener
    {
        private ICaster _caster;
        private List<IAbility> _abilities = new();

        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        public void Init(ICaster caster)
        {
            _caster = caster;
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

            if (_caster == null) return;

            foreach (var source in _caster.AbilitySources)
            {
                if (source == null) continue;
                foreach (var data in source.Abilities)
                {
                    var ability = data.Create();
                    ability.Init(_caster, source);
                    _abilities.Add(ability);
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
