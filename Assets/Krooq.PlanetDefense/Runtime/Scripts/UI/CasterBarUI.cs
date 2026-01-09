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

        protected override void OnRefreshTiles()
        {
            var activeCasters = Player.Casters;
            if (activeCasters == null) return;

            for (int i = 0; i < activeCasters.Count; i++)
            {
                var caster = activeCasters[i];
                if (caster != null && i < _slotContainer.childCount)
                {
                    var slot = _slotContainer.GetChild(i);
                    var ui = GameManager.Spawn(GameManager.Data.CasterTilePrefab);
                    ui.transform.SetParent(slot);
                    ui.transform.localPosition = Vector3.zero;
                    ui.Init(caster, false, i);
                }
            }
        }
    }
}
