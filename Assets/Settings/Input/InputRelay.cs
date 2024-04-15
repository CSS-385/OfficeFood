using System;

namespace UnityEngine.InputSystem
{
    [CreateAssetMenu(fileName = "InputRelay", menuName = "Input System/Input Relay", order = 1)]
    public class InputRelay : ScriptableObject, InputMap.IGameActions, InputMap.IMenuActions
    {
        // every input needs events ; frame taps might not be picked up during physics frame
        // input can be optionally polled

        public Vector2 GameMove { get; private set; } = Vector2.zero;
        public event Action<Vector2> GameMoveEvent = delegate { };

        public bool GameInteract { get; private set; } = false;
        public event Action<bool> GameInteractEvent = delegate { };

        public Vector2 MenuMove { get; private set; } = Vector2.zero;
        public event Action<Vector2> MenuMoveEvent = delegate { };

        private InputMap _input = null;

        private void OnEnable()
        {
            // TODO: eventually game state directly switches input modes
            if (_input == null)
            {
                _input = new InputMap();
                _input.Game.SetCallbacks(this);
                _input.Menu.SetCallbacks(this);
            }
            _input.Game.Enable();
            _input.Menu.Disable();
        }

        private void OnDisable()
        {
            _input.Game.Disable();
            _input.Menu.Disable();
        }

        void InputMap.IGameActions.OnMove(InputAction.CallbackContext context)
        {
            GameMove = context.ReadValue<Vector2>();
            GameMoveEvent.Invoke(GameMove);
        }

        void InputMap.IGameActions.OnInteract(InputAction.CallbackContext context)
        {
            GameInteract = context.ReadValueAsButton();
            GameInteractEvent.Invoke(GameInteract);
        }

        void InputMap.IMenuActions.OnMove(InputAction.CallbackContext context)
        {
            MenuMove = context.ReadValue<Vector2>();
            MenuMoveEvent.Invoke(MenuMove);
        }
    }
}
