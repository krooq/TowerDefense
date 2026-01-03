using UnityEngine;
using System.Collections.Generic;
using System;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(menuName = "PlanetDefense/Tiles/Split")]
    public class SplitTile : UpgradeTile
    {
        [SerializeField] private float _splitAngle = 15f;
        public float SplitAngle => _splitAngle;

        public override bool Process(ProjectileContext context, List<UpgradeTile> remainingChain, GameManager gameManager)
        {
            // Rotate current
            Quaternion rot1 = Quaternion.Euler(0, 0, -SplitAngle);
            Quaternion rot2 = Quaternion.Euler(0, 0, SplitAngle);

            Vector3 originalDir = context.Direction;

            // Modify current
            context.SetDirection(rot1 * originalDir);
            if (context.ProjectileObject != null)
            {
                context.ProjectileObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, context.Direction);
                // Update projectile component if it exists
                if (context.ProjectileObject != null) context.ProjectileObject.Init(context.Direction, context.Stats);
            }

            // Spawn new
            var newProjectile = gameManager.SpawnProjectile();
            newProjectile.transform.SetPositionAndRotation(context.Position, Quaternion.LookRotation(Vector3.forward, rot2 * originalDir));
            newProjectile.Init(rot2 * originalDir, context.Stats.Clone());

            // Create context for new projectile
            var newContext = new ProjectileContext(newProjectile, context.Position, rot2 * originalDir, context.Stats.Clone(), context.IsSetupPhase);

            // Run chain on new projectile
            TileSequence.RunChain(newContext, remainingChain, gameManager);

            // Re-initialize with potentially modified stats
            newProjectile.Init(newContext.Direction, newContext.Stats);

            return true; // Continue chain on current projectile
        }
    }
}
