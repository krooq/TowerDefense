using UnityEngine;
using TMPro;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class ResourcesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private int _lastResources = -1;
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected void Update()
        {
            if (GameManager == null) return;

            if (_lastResources != GameManager.Resources)
            {
                _lastResources = GameManager.Resources;
                _text.text = $"{_lastResources}";
            }
        }
    }
}
