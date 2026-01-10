using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Krooq.PlanetDefense
{
    public class PlayerTargetingReticle : MonoBehaviour, ITarget
    {
        [Header("Visuals")]
        [SerializeField] private GameObject _groundVisual;
        [SerializeField] private GameObject _airVisual;

        public bool IsValid { get; set; }
        public Transform Transform => null;
        public Vector3 Position { get; set; }
        public bool IsGroundTarget { get; private set; }
        public UnityEvent OnTargetDestroyed => new();

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

            float groundLevel = GameManager.Data.HorizonLevel;

            if (worldPos.y <= groundLevel)
            {
                IsValid = true;
                IsGroundTarget = true;
                Position = new Vector3(worldPos.x, worldPos.y, 0f);
                if (_groundVisual) _groundVisual.SetActive(true);
                if (_airVisual) _airVisual.SetActive(false);
            }
            else
            {
                IsGroundTarget = false;
                IsValid = true;
                Position = new Vector3(worldPos.x, worldPos.y, 0f);

                if (_groundVisual) _groundVisual.SetActive(false);
                if (_airVisual) _airVisual.SetActive(true);
            }

            transform.position = Position;
        }
    }
}
