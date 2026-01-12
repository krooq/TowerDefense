using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class BackgroundManager : MonoBehaviour
    {
        [System.Serializable]
        public class BackgroundLayer
        {
            [SerializeField] private Transform _transform;
            [SerializeField] private float _parallaxFactor;

            public Transform Transform => _transform;
            public float ParallaxFactor => _parallaxFactor;
            public Vector3 InitialPosition { get; set; }
            public Rigidbody2D Body { get; set; }
        }

        [SerializeField] private List<BackgroundLayer> _layers = new();
        [SerializeField] private float _parallaxStrength = 1f;
        [SerializeField] private float _smoothing = 5f;

        private Vector2 _currentNormalizedOffset;

        protected PlayerInputs PlayerInputs => this.GetSingleton<PlayerInputs>();

        private void Start()
        {
            foreach (var layer in _layers)
            {
                if (layer.Transform != null)
                {
                    layer.InitialPosition = layer.Transform.localPosition;
                    layer.Body = layer.Transform.GetComponent<Rigidbody2D>();
                }
            }
        }

        private void Update()
        {
            if (PlayerInputs == null) return;

            var mousePos = PlayerInputs.PointAction.ReadValue<Vector2>();

            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var offset = mousePos - screenCenter;

            // Normalize offset so edge of screen is roughly 1.0
            var normalizedOffset = -new Vector2(
                offset.x / screenCenter.x,
                0.1f * offset.y / screenCenter.y
            );

            _currentNormalizedOffset = Vector2.ClampMagnitude(normalizedOffset, 1.5f);

            // Move non-physics layers in Update for smoothness
            foreach (var layer in _layers)
            {
                if (layer.Transform == null || layer.Body != null) continue;
                MoveLayer(layer, Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            // Move physics layers in FixedUpdate for consistency
            foreach (var layer in _layers)
            {
                if (layer.Transform == null || layer.Body == null) continue;
                MoveLayer(layer, Time.fixedDeltaTime);
            }
        }

        private void MoveLayer(BackgroundLayer layer, float deltaTime)
        {
            var moveAmount = _currentNormalizedOffset * layer.ParallaxFactor * _parallaxStrength;

            // Calculate target in Local space
            var targetLocalPos = layer.InitialPosition + (Vector3)moveAmount;

            if (layer.Body != null)
            {
                // For Rigidbody, we strictly need World positions for MovePosition.
                // We convert the target local position to world position.
                var targetWorldPos = layer.Transform.parent != null
                    ? layer.Transform.parent.TransformPoint(targetLocalPos)
                    : targetLocalPos;

                // Lerp the position for smoothing
                var newPos = Vector3.Lerp(
                    layer.Body.position,
                    targetWorldPos,
                    deltaTime * _smoothing
                );

                layer.Body.MovePosition(newPos);
            }
            else
            {
                // Standard Transform movement
                layer.Transform.localPosition = Vector3.Lerp(
                    layer.Transform.localPosition,
                    targetLocalPos,
                    deltaTime * _smoothing
                );
            }
        }
    }
}
