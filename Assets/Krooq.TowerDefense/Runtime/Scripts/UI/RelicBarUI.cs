using UnityEngine;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class RelicBarUI : BaseBarUI
    {
        protected override int MaxSlots => GameManager.Data.MaxRelicSlots;

        protected override BaseSlotUI SpawnSlot()
        {
            return GameManager.Spawn(GameManager.Data.RelicSlotPrefab);
        }

        protected override void UpdateTiles()
        {
            var activeRelics = this.GetSingleton<Player>().Relics;
            if (activeRelics == null) return;

            for (int i = 0; i < _slotContainer.childCount; i++)
            {
                var slot = _slotContainer.GetChild(i);
                var relic = (i < activeRelics.Count) ? activeRelics[i] : null;

                if (slot.childCount > 0)
                {
                    var currentChild = slot.GetChild(0);
                    var currentUI = currentChild.GetComponent<RelicTileUI>();

                    if (currentUI != null && currentUI.Item == relic)
                    {
                        continue;
                    }

                    GameManager.Despawn(currentChild.gameObject);
                }

                if (relic != null)
                {
                    var ui = GameManager.Spawn(GameManager.Data.RelicTilePrefab);
                    ui.transform.SetParent(slot);
                    ui.transform.localPosition = Vector3.zero;
                    ui.Init(relic, false, i);
                }
            }
        }
    }
}
