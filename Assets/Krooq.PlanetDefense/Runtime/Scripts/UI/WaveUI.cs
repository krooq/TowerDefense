using UnityEngine;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class WaveUI : MonoBehaviour
    {
        [SerializeField] private Button _nextWaveButton;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected void OnEnable()
        {
            if (_nextWaveButton != null) _nextWaveButton.onClick.AddListener(OnNextWave);
        }

        protected void OnDisable()
        {
            if (_nextWaveButton != null) _nextWaveButton.onClick.RemoveListener(OnNextWave);
        }

        protected void OnNextWave()
        {
            GameManager.NextWave();
        }
    }
}
