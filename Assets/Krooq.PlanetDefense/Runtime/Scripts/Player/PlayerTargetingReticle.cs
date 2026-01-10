using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class PlayerTargetingReticle : MonoBehaviour, ITargetingInfo
    {
        [Header("Visuals")]
        [SerializeField] private GameObject _groundVisual;
        [SerializeField] private GameObject _airVisual;

        public Vector3 TargetPosition { get; private set; }
        public bool IsGroundTarget { get; private set; }
        public bool IsValid => true;

        private Camera _cam;
        protected Player Player => this.GetSingleton<Player>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();


        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (GameManager.State != GameState.Playing)
            {
                if (_groundVisual) _groundVisual.SetActive(false);
                if (_airVisual) _airVisual.SetActive(false);
                return;
            }

            UpdateReticle();
        }

        private void UpdateReticle()
        {
            var mouseScreenPos = Player.Inputs.PointAction.ReadValue<Vector2>();
            if (_cam == null) return;

            var worldPos = _cam.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = 0f;

            float groundLevel = GameManager.Data.HorizonY;

            if (worldPos.y <= groundLevel)
            {
                IsGroundTarget = true;
                TargetPosition = new Vector3(worldPos.x, worldPos.y, 0f);
                if (_groundVisual) _groundVisual.SetActive(true);
                if (_airVisual) _airVisual.SetActive(false);
            }
            else
            {
                IsGroundTarget = false;
                TargetPosition = worldPos;

                if (_groundVisual) _groundVisual.SetActive(false);
                if (_airVisual) _airVisual.SetActive(true);
            }

            transform.position = TargetPosition;
        }
    }
}
