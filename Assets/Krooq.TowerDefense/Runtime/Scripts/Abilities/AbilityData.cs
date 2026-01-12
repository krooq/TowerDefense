using UnityEngine;

namespace Krooq.TowerDefense
{
    public abstract class AbilityData : ScriptableObject
    {
        [SerializeField, TextArea] private string _description;
        public string Description => _description;

        public abstract IAbility Create();
    }
}
