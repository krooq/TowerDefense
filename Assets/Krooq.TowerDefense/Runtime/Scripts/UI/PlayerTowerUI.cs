using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class PlayerTowerUI : MonoBehaviour
    {
        [SerializeField] private Transform _relicRoot;
        [SerializeField, ReadOnly] private List<RelicSlotUI> _activeRelicSlots = new();

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected CanvasGroup CanvasGroup => this.GetCachedComponent<CanvasGroup>();


        private void Update()
        {
            if (GameManager.State == GameState.Shop)
            {
                SetVisible(true);
            }
            else
            {
                SetVisible(false);
            }
        }

        public void SetVisible(bool visible)
        {
            if (CanvasGroup == null) return;
            CanvasGroup.alpha = visible ? 1 : 0;
            CanvasGroup.interactable = visible;
            CanvasGroup.blocksRaycasts = visible;
        }

        public void Init(int[] relicIndices)
        {
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
