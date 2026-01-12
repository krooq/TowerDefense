using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public abstract class BaseSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField] protected int _slotIndex;
        [SerializeField] protected Image _icon;

        protected ShopUI ShopUI => this.GetSingleton<ShopUI>();
        protected Player Player => this.GetSingleton<Player>();

        public int SlotIndex => _slotIndex;

        public void Init(int index)
        {
            _slotIndex = index;
        }

        protected virtual void OnDisable()
        {
            _slotIndex = -1;
        }

        public abstract void OnDrop(PointerEventData eventData);
    }
}
