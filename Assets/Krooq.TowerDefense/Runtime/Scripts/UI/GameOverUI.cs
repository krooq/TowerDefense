using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private TextMeshProUGUI _wavesSurvivedText;

        [SerializeField, ReadOnly] private bool _shown = false;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected CanvasGroup CanvasGroup => this.GetCachedComponent<CanvasGroup>();

        protected void OnEnable()
        {
            Hide();
            if (_restartButton)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        protected void OnDisable()
        {
            if (_restartButton)
            {
                _restartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        protected void Update()
        {
            if (GameManager == null) return;

            if (GameManager.State == GameState.GameOver)
            {
                if (!_shown)
                {
                    Show();
                }
            }
            else
            {
                if (_shown)
                {
                    Hide();
                }
            }
        }

        private void Show()
        {
            _shown = true;
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            if (_wavesSurvivedText != null)
            {
                _wavesSurvivedText.text = $"Waves Survived: {GameManager.Wave - 1}";
            }
        }

        private void Hide()
        {
            _shown = false;
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        private void OnRestartClicked()
        {
            GameManager.StartGame();
            Hide();
        }
    }
}
