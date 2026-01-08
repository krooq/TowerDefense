using UnityEngine;

namespace Krooq.PlanetDefense
{
    public abstract class AbilityDefinition : ScriptableObject
    {
        [SerializeField, TextArea] private string _description;
        public string Description => _description;

        public abstract IAbility Create();
    }
}
