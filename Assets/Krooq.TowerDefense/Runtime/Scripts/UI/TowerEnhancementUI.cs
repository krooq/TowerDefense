using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class TowerEnhancementUI : MonoBehaviour
    {
        [SerializeField] private CasterSlotUI _casterSlot;
        [SerializeField] private Transform _relicRoot;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected CanvasGroup CanvasGroup => this.GetCachedComponent<CanvasGroup>();

        private List<RelicSlotUI> _activeRelicSlots = new();

        public void SetVisible(bool visible)
        {
            if (CanvasGroup == null) return;
            CanvasGroup.alpha = visible ? 1 : 0;
            CanvasGroup.interactable = visible;
            CanvasGroup.blocksRaycasts = visible;
        }

        public void Init(int casterIndex, int[] relicIndices)
        {
            if (_casterSlot != null)
            {
                _casterSlot.Init(casterIndex);
            }

            // Clear existing
            foreach (var slot in _activeRelicSlots)
            {
                if (slot != null) GameManager.Despawn(slot.gameObject);
            }
            _activeRelicSlots.Clear();

            if (GameManager != null && GameManager.Data.RelicSlotPrefab != null && _relicRoot != null && relicIndices != null)
            {
                foreach (var index in relicIndices)
                {
                    var slot = GameManager.Spawn(GameManager.Data.RelicSlotPrefab);
                    slot.transform.SetParent(_relicRoot, false);
                    slot.Init(index);
                    _activeRelicSlots.Add(slot);
                }
            }
        }
    }
}
