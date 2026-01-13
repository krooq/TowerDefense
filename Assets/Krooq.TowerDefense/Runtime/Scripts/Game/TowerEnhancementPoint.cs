using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using UnityEngine.EventSystems;

namespace Krooq.TowerDefense
{
    public class TowerEnhancementPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform _uiRoot;
        [SerializeField] private Vector3 _uiOffset = new Vector3(0, 1, 0);
        [SerializeField] private int _casterIndex;
        [SerializeField] private int[] _relicIndices;

        private TowerEnhancementUI _spawnedUI;
        private bool _isHovered;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected WorldCanvas WorldCanvas => this.GetSingleton<WorldCanvas>();

        private void Start()
        {
            SpawnUI();
            UpdateVisibility();
        }

        private void OnDestroy()
        {
            if (_spawnedUI != null && GameManager != null)
            {
                GameManager.Despawn(_spawnedUI.gameObject);
                _spawnedUI = null;
            }
        }

        private void Update()
        {
            UpdateVisibility();
        }

        private void SpawnUI()
        {
            if (_spawnedUI == null && GameManager.Data.TowerEnhancementUIPrefab != null)
            {
                _spawnedUI = GameManager.Spawn(GameManager.Data.TowerEnhancementUIPrefab);

                Transform parent = _uiRoot;
                if (parent == null && WorldCanvas != null)
                {
                    parent = WorldCanvas.transform;
                }

                // Fallback to self if no WorldCanvas and no uiRoot
                if (parent == null) parent = transform;

                _spawnedUI.transform.SetParent(parent, false);

                // Position relative to this point
                if (parent == transform)
                {
                    _spawnedUI.transform.localPosition = _uiOffset;
                }
                else
                {
                    _spawnedUI.transform.position = transform.position + _uiOffset;
                }

                _spawnedUI.Init(_casterIndex, _relicIndices);
                _spawnedUI.SetVisible(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
        }

        private void UpdateVisibility()
        {
            if (_spawnedUI == null || GameManager == null) return;

            bool shouldShow = false;

            if (GameManager.State == GameState.Shop)
            {
                shouldShow = true;
            }
            else if (GameManager.State == GameState.Playing)
            {
                shouldShow = _isHovered;
            }

            _spawnedUI.SetVisible(shouldShow);
        }
    }
}
