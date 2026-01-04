using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "PlanetDefense/Modifier")]
    public class Modifier : ScriptableObject
    {
        [SerializeField] private string _tileName;
        [SerializeField] private int _cost;
        [SerializeField] private Color _tileColor = Color.white;

        [SerializeField] private List<ModifierEffect> _effects = new();

        public string TileName => _tileName;
        public int Cost => _cost;
        public Color TileColor => _tileColor;
        public IReadOnlyList<ModifierEffect> Effects => _effects;

        public bool Process(Projectile projectile, ModifierTrigger trigger)
        {
            bool appliedAny = false;
            foreach (var effect in _effects)
            {
                if (effect.Trigger == trigger)
                {
                    effect.Apply(projectile);
                    appliedAny = true;
                }
            }
            return appliedAny;
        }
    }

    public enum ModifierTrigger
    {
        Always,     // Applied immediately when modifier is added
        OnHit,      // Applied when projectile hits a target
        OnDespawn,  // Applied when projectile is despawned
        OnTimer,    // Applied after a delay
    }

    [System.Serializable]
    public class ModifierEffect
    {
        [SerializeField] private ModifierTrigger _trigger = ModifierTrigger.Always;

        [SerializeField, ShowIf("_trigger", ModifierTrigger.OnTimer)]
        private float _timerDuration;

        [SerializeField] private List<StatModifier> _statModifiers = new();
        [SerializeField] private List<string> _tagsToAdd = new();
        [SerializeField] private List<string> _tagsToRemove = new();

        [SerializeField] private GameObject _prefabToSpawn;
        [SerializeField] private int _spawnCount = 1;

        public ModifierTrigger Trigger => _trigger;
        public float TimerDuration => _timerDuration;
        public IReadOnlyList<StatModifier> StatModifiers => _statModifiers;
        public IReadOnlyList<string> TagsToAdd => _tagsToAdd;
        public IReadOnlyList<string> TagsToRemove => _tagsToRemove;
        public GameObject PrefabToSpawn => _prefabToSpawn;
        public int SpawnCount => _spawnCount;

        public void Apply(Projectile projectile)
        {
            foreach (var statMod in _statModifiers) projectile.AddStatModifier(statMod);
            foreach (var tag in _tagsToAdd) projectile.AddTag(tag);
            foreach (var tag in _tagsToRemove) projectile.RemoveTag(tag);
            if (_prefabToSpawn != null) projectile.SpawnChild(_prefabToSpawn, _spawnCount);
        }
    }
}
