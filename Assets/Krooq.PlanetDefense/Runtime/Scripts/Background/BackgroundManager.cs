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
        }

        [SerializeField] private List<BackgroundLayer> _layers = new();
        [SerializeField] private float _parallaxStrength = 1f;
        [SerializeField] private float _smoothing = 5f;

        protected PlayerInputs PlayerInputs => this.GetSingleton<PlayerInputs>();

        private void Start()
        {
            foreach (var layer in _layers)
            {
                if (layer.Transform != null)
                {
                    layer.InitialPosition = layer.Transform.localPosition;
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
                0.5f * offset.y / screenCenter.y
            );

            normalizedOffset = Vector2.ClampMagnitude(normalizedOffset, 1.5f);

            foreach (var layer in _layers)
            {
                if (layer.Transform == null) continue;

                var moveAmount = normalizedOffset * layer.ParallaxFactor * _parallaxStrength;
                var targetPos = layer.InitialPosition + (Vector3)moveAmount;

                layer.Transform.localPosition = Vector3.Lerp(
                    layer.Transform.localPosition,
                    targetPos,
                    Time.deltaTime * _smoothing
                );
            }
        }
    }
}
