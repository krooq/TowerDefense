using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class ManaUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        [SerializeField, ReadOnly] private Caster _caster;

        protected PlayerCaster PlayerCaster => this.GetSingleton<PlayerCaster>();

        public void Init(Caster caster)
        {
            _caster = caster;
        }

        protected void Update()
        {
            var caster = _caster != null ? _caster : PlayerCaster;
            if (caster == null) return;
            float current = caster.CurrentMana;
            float max = caster.MaxMana;

            if (_fillImage != null && max > 0)
            {
                _fillImage.fillAmount = current / max;
            }

            if (_valueText != null)
            {
                _valueText.text = $"{caster.CurrentMana} / {caster.MaxMana}"; // Use int values for text
            }
        }
    }
}
