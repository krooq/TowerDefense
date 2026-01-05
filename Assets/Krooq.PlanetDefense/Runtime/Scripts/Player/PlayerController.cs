using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public class PlayerController : MonoBehaviour
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

            HandleMovement();
            HandleAiming();
            HandleFiring();
        }

        protected void HandleMovement()
        {
            var moveInput = InputManager.MoveAction.ReadValue<Vector2>();
            transform.Translate(GameManager.Data.MoveSpeed * moveInput.x * Time.deltaTime * Vector3.right);
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
                if (GameManager.SelectedWeapon != null)
                {
                    Fire();
                }
            }
        }

        protected void Fire()
        {
            var p = GameManager.SpawnProjectile();
            if (p == null) return;

            p.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);

            // Get Modifiers
            var modifiers = GameManager.ActiveModifiers;

            // Finalize
            p.Init(_firePoint.up, GameManager.SelectedWeapon, modifiers);

            // Set Fire Timer based on projectile stat
            _fireTimer = 1f / p.FireRate;

            foreach (var effect in _fireEffects)
            {
                effect.SetActive(false);
                effect.SetActive(true);
            }

            _recoilTransform.BumpPosition(_firePoint.localPosition - Vector3.up * 10f);
            if (GameManager.SelectedWeapon.FireSound != null) AudioManager.PlaySound(GameManager.SelectedWeapon.FireSound);
        }
    }
}
