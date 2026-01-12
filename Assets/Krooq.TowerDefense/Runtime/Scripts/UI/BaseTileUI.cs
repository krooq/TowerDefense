using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public abstract class BaseTileUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected TextMeshProUGUI _text;
        [SerializeField] protected CanvasGroup _canvasGroup;

        protected bool _isShopItem;
        protected Transform _originalParent;
        protected Vector3 _originalPosition;
        protected int _slotIndex = -1;

        protected Player Player => this.GetSingleton<Player>();
        protected ShopUI ShopUI => this.GetSingleton<ShopUI>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public IShopItem Item { get; protected set; }
        public bool IsShopItem => _isShopItem;

        public virtual void Init(IShopItem item, bool isShopItem, int slotIndex = -1, UnityAction onClick = null)
        {
            Item = item;
            _isShopItem = isShopItem;
            _slotIndex = slotIndex;

            if (_text != null)
            {
                if (isShopItem)
                    _text.text = $"{item.Name} (${item.ShopCost})";
                else
                    _text.text = item.Name;
            }

            if (_button != null && onClick != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(onClick);
            }
        }

        protected virtual void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
            }
            Item = null;
            _slotIndex = -1;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!_isShopItem && GameManager.State == GameState.Shop && _slotIndex != -1)
                {
                    OnSell();
                }
            }
        }

        protected abstract void OnSell();

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;
            _originalPosition = transform.position;

            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            transform.position = _originalPosition;
        }
    }
}
