using UnityEngine;
using UnityEngine.InputSystem;
using Krooq.Common;
using Sirenix.OdinInspector;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class PlayerInputs : MonoBehaviour
    {
        public const string Click = "Click";
        public const string RightClick = "RightClick";
        public const string Point = "Point";
        public const string ScrollWheel = "ScrollWheel";
        public const string Submit = "Submit";
        public const string Cancel = "Cancel";

        public const string Move = "Move";
        public const string Spell1 = "Spell1";
        public const string Spell2 = "Spell2";
        public const string Spell3 = "Spell3";
        public const string Spell4 = "Spell4";
        public const string QuickCast1 = "QuickCast1";
        public const string QuickCast2 = "QuickCast2";
        public const string QuickCast3 = "QuickCast3";
        public const string QuickCast4 = "QuickCast4";

        // UI Map.
        public InputAction ClickAction => PlayerInput.actions.FindAction(Click);
        public InputAction RightClickAction => PlayerInput.actions.FindAction(RightClick);
        public InputAction PointAction => PlayerInput.actions.FindAction(Point);
        public InputAction ScrollAction => PlayerInput.actions.FindAction(ScrollWheel);
        public InputAction SubmitAction => PlayerInput.actions.FindAction(Submit);
        public InputAction CancelAction => PlayerInput.actions.FindAction(Cancel);

        // Player Map.
        public InputAction MoveAction => PlayerInput.actions.FindAction(Move);

        public InputAction Spell1Action => PlayerInput.actions.FindAction(Spell1);
        public InputAction Spell2Action => PlayerInput.actions.FindAction(Spell2);
        public InputAction Spell3Action => PlayerInput.actions.FindAction(Spell3);
        public InputAction Spell4Action => PlayerInput.actions.FindAction(Spell4);

        public InputAction QuickCast1Action => PlayerInput.actions.FindAction(QuickCast1);
        public InputAction QuickCast2Action => PlayerInput.actions.FindAction(QuickCast2);
        public InputAction QuickCast3Action => PlayerInput.actions.FindAction(QuickCast3);
        public InputAction QuickCast4Action => PlayerInput.actions.FindAction(QuickCast4);

        protected PlayerInput PlayerInput => this.GetCachedComponent<PlayerInput>();

        protected void Start()
        {
            // PlayerInput disables all action maps except the default, so we need to enable the UI action map explicitly AFTER it does this.
            PlayerInput.actions.FindActionMap("Player")?.Enable();
            PlayerInput.actions.FindActionMap("UI")?.Enable();
        }
    }
}
