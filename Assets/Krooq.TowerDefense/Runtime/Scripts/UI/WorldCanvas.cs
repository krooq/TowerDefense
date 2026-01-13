using UnityEngine;

namespace Krooq.TowerDefense
{
    public class WorldCanvas : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null || canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("WorldCanvas should be attached to a Canvas with RenderMode set to WorldSpace.");
            }
        }
    }
}
