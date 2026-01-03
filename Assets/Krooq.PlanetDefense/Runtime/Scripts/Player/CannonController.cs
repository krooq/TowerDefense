using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public class CannonController : MonoBehaviour
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private SpringTransform _recoilTransform;

        [SerializeField] private List<GameObject> _fireEffects;

        [SerializeField, ReadOnly] private float _fireTimer;
        private Camera _cam;
        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected InputManager InputManager => this.GetSingleton<InputManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();

        protected void Start()
        {
            _cam = Camera.main;
        }

        protected void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            HandleAiming();
            HandleFiring();
        }

        protected void HandleAiming()
        {
            var mousePos = _cam.ScreenToWorldPoint(InputManager.PointAction.ReadValue<Vector2>());
            mousePos.z = 0f;

            var dir = (mousePos - _pivot.position).normalized;

            // Restrict to upper semicircle
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.x = Mathf.Sign(dir.x);
            }

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // Assuming sprite points up

            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * GameManager.Data.RotationSpeed);
        }

        protected void HandleFiring()
        {
            _fireTimer -= Time.deltaTime;

            if (InputManager.ClickAction.IsPressed() && _fireTimer <= 0)
            {
                Fire();
                _fireTimer = GameManager.Data.FireRate;
            }
        }

        protected void Fire()
        {
            var p = GameManager.SpawnProjectile();
            p.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);

            // Base Stats
            var stats = p.Stats.Clone();

            var context = new ProjectileContext(p, _firePoint.position, _firePoint.up, stats, true);

            TileSequence.RunChain(context, GameManager.ActiveUpgrades, GameManager);

            // Finalize
            p.Init(context.Direction, context.Stats);

            foreach (var effect in _fireEffects)
            {
                effect.SetActive(false);
                effect.SetActive(true);
            }

            _recoilTransform.BumpPosition(_firePoint.localPosition - Vector3.up * 10f);
            AudioManager.PlaySound(GameManager.Data.CannonFireSound);

        }
    }
}
