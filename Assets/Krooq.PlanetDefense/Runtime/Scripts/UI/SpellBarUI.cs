using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using System.Linq;

namespace Krooq.PlanetDefense
{
    public class SpellBarUI : MonoBehaviour
    {
        [SerializeField] private Transform _spellSlotContainer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected void OnEnable()
        {
            for (int i = 0; i < GameManager.Data.MaxSlots; i++)
            {
                var slot = GameManager.Spawn(GameManager.Data.SpellSlotPrefab);
                slot.transform.SetParent(_spellSlotContainer, false);
                slot.Init(i);
            }
            Refresh();
        }

        public void Refresh()
        {
            // Clear Active Slots (remove children of slots)
            foreach (Transform slotTransform in _spellSlotContainer)
            {
                for (var i = slotTransform.childCount - 1; i >= 0; i--) GameManager.Despawn(slotTransform.GetChild(i).gameObject);
            }

            // Populate Active Slots
            var activeSpells = GameManager.Spells;
            if (activeSpells == null) return;

            for (int i = 0; i < activeSpells.Count; i++)
            {
                var spell = activeSpells[i];
                if (spell != null && i < _spellSlotContainer.childCount)
                {
                    var slot = _spellSlotContainer.GetChild(i);
                    var ui = GameManager.Spawn(GameManager.Data.SpellTilePrefab);
                    ui.transform.SetParent(slot);
                    ui.transform.localPosition = Vector3.zero;
                    ui.Init(spell, false, i);
                }
            }
        }
    }
}
