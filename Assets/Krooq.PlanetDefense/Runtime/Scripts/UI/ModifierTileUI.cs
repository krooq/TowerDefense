using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Krooq.PlanetDefense
{
    public class ModifierTileUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Modifier _modifier;
        private bool _isShopItem;
        private Transform _originalParent;
        private Vector3 _originalPosition;

        public Modifier Modifier => _modifier;
        public bool IsShopItem => _isShopItem;

        public void Init(Modifier modifier, bool isShopItem, UnityAction onClick = null)
        {
            _modifier = modifier;
            _isShopItem = isShopItem;

            if (_text != null)
            {
                if (isShopItem)
                    _text.text = $"{modifier.TileName} (${modifier.Cost})";
                else
                    _text.text = modifier.TileName;
            }

            if (_button != null && onClick != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(onClick);
            }
        }

        protected void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
            }
            _modifier = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;
            _originalPosition = transform.position;

            // Move to root or high level canvas to draw over everything
            // For now, just keeping parent but disabling layout might be tricky.
            // Better to reparent to the ShopUI root or a "DragLayer".
            // Assuming ShopUI has a Canvas.

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
            // If dropped successfully, the slot will handle logic and UI refresh will happen.
            // If not dropped on a slot, we just snap back (visual reset).

            // If it was an active tile and dropped outside (not on a slot), maybe sell?
            if (!_isShopItem && !eventData.pointerEnter)
            {
                // Sell logic here? Or check if dropped on "Sell Area".
                // User said "sold from a modifier slot".
                // If we drag it out into nothingness, that could be selling.

                // Trigger sell event?
                // For now, let's leave it.
            }
        }
    }
}
