using UnityEngine;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class CasterBarUI : BaseBarUI
    {
        protected override int MaxSlots => GameManager.Data.MaxCasterSlots;

        protected Player Player => this.GetSingleton<Player>();

        protected override BaseSlotUI SpawnSlot()
        {
            return GameManager.Spawn(GameManager.Data.CasterSlotPrefab);
        }

        protected override void UpdateTiles()
        {
            var activeCasters = Player.Casters;
            if (activeCasters == null) return;

            for (int i = 0; i < _slotContainer.childCount; i++)
            {
                var slot = _slotContainer.GetChild(i);
                var caster = (i < activeCasters.Count) ? activeCasters[i] : null;

                if (slot.childCount > 0)
                {
                    var currentChild = slot.GetChild(0);
                    var currentUI = currentChild.GetComponent<CasterTileUI>();

                    if (currentUI != null && currentUI.Item == caster)
                    {
                        continue;
                    }

                    GameManager.Despawn(currentChild.gameObject);
                }

                if (caster != null)
                {
                    var ui = GameManager.Spawn(GameManager.Data.CasterTilePrefab);
                    ui.transform.SetParent(slot);
                    ui.transform.localPosition = Vector3.zero;
                    ui.Init(caster, false, i);
                }
            }
        }
    }
}
