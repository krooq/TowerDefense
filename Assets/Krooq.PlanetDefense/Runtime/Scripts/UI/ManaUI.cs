using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class ManaUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        protected Player Player => this.GetSingleton<Player>();

        protected void Update()
        {
            if (Player == null) return;
            float current = Player.CurrentMana;
            float max = Player.MaxMana;

            if (_fillImage != null && max > 0)
            {
                _fillImage.fillAmount = current / max;
            }

            if (_valueText != null)
            {
                _valueText.text = $"{Player.CurrentMana} / {Player.MaxMana}"; // Use int values for text
            }
        }
    }
}
