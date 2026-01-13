using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class GameMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        [SerializeField, ReadOnly] private bool _isPaused;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected Player Player => this.GetSingleton<Player>();
        protected CanvasGroup CanvasGroup => this.GetCachedComponent<CanvasGroup>();

        private void OnEnable()
        {
            if (Player != null && Player.Inputs != null && Player.Inputs.PauseAction != null)
            {
                Player.Inputs.PauseAction.performed += OnMenuActionPerformed;
            }

            if (_resumeButton) _resumeButton.onClick.AddListener(Resume);
            if (_restartButton) _restartButton.onClick.AddListener(Restart);
            if (_quitButton) _quitButton.onClick.AddListener(Quit);

            Hide();
        }

        private void OnDisable()
        {
            if (Player != null && Player.Inputs != null && Player.Inputs.PauseAction != null)
            {
                Player.Inputs.PauseAction.performed -= OnMenuActionPerformed;
            }

            if (_resumeButton) _resumeButton.onClick.RemoveListener(Resume);
            if (_restartButton) _restartButton.onClick.RemoveListener(Restart);
            if (_quitButton) _quitButton.onClick.RemoveListener(Quit);
        }

        private void OnMenuActionPerformed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        public void TogglePause()
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            // Do not allow pausing if the game is over
            if (GameManager.State == GameState.GameOver) return;

            _isPaused = true;
            Time.timeScale = 0f;
            Show();
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            Hide();
        }

        public void Restart()
        {
            Resume();
            GameManager.StartGame();
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void Show()
        {
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        private void Hide()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
    }
}
