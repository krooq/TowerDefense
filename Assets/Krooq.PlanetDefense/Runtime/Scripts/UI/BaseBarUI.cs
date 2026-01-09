using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public abstract class BaseBarUI : MonoBehaviour
    {
        [SerializeField] protected Transform _slotContainer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected virtual void OnEnable()
        {
            if (_slotContainer.childCount == 0)
            {
                for (int i = 0; i < MaxSlots; i++)
                {
                    var slot = SpawnSlot();
                    slot.transform.SetParent(_slotContainer, false);
                    slot.Init(i);
                }
            }
        }

        protected virtual void Update()
        {
            UpdateTiles();
        }

        protected abstract int MaxSlots { get; }
        protected abstract BaseSlotUI SpawnSlot();
        protected abstract void UpdateTiles();
    }
}
