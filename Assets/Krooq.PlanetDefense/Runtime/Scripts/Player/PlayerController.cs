using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerCaster _caster;
        [SerializeField] private PlayerTargetingReticle _targetingReticle;

        protected Player Player => this.GetSingleton<Player>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();

        protected void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            // NOTE: Player movement is currently disabled.
            // HandleMovement();
            HandleAiming();
        }

        protected void HandleMovement()
        {
            var moveInput = Player.Inputs.MoveAction.ReadValue<Vector2>();
            transform.Translate(GameManager.Data.MoveSpeed * moveInput.x * Time.deltaTime * Vector3.right);
        }

        protected void HandleAiming()
        {
            if (_targetingReticle == null) return;
            _caster.Aim(_targetingReticle.TargetPosition, GameManager.Data.RotationSpeed);
        }
    }
}
