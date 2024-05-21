using System;

namespace UnityEngine.InputSystem
{
    [CreateAssetMenu(fileName = "InputRelay", menuName = "Input System/Input Relay", order = 1)]
    public class InputRelay : ScriptableObject, InputMap.IGameActions, InputMap.IMenuActions
    {
        // every input needs events ; frame taps might not be picked up during physics frame
        // input can be optionally polled

        public Vector2 gameMove { get; private set; } = Vector2.zero;
        public event Action<Vector2> GameMoveEvent;

        public bool gameInteract { get; private set; } = false;
        public event Action<bool> GameInteractEvent;

        public bool gameSprint { get; private set; } = false;
        public event Action<bool> GameSprintEvent;

        public bool gamePause { get; private set; } = false;
        public event Action<bool> GamePauseEvent;

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
            _input.Menu.Enable();
        }

        private void OnDisable()
        {
            _input.Game.Disable();
            _input.Menu.Disable();
        }

        void InputMap.IGameActions.OnMove(InputAction.CallbackContext context)
        {
            gameMove = context.ReadValue<Vector2>().normalized;
            GameMoveEvent?.Invoke(gameMove);
        }

        void InputMap.IGameActions.OnInteract(InputAction.CallbackContext context)
        {
            gameInteract = context.ReadValueAsButton();
            GameInteractEvent?.Invoke(gameInteract);
        }

        void InputMap.IGameActions.OnSprint(InputAction.CallbackContext context)
        {
            gameSprint = context.ReadValueAsButton();
            GameSprintEvent?.Invoke(gameSprint);
        }

        void InputMap.IGameActions.OnPause(InputAction.CallbackContext context)
        {
            gamePause = context.ReadValueAsButton();
            GamePauseEvent?.Invoke(gamePause);
        }

        // These don't need impls
        void InputMap.IMenuActions.OnNavigate(InputAction.CallbackContext context) {  }

        void InputMap.IMenuActions.OnSubmit(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnCancel(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnPoint(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnClick(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnScrollWheel(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnMiddleClick(InputAction.CallbackContext context) { }

        void InputMap.IMenuActions.OnRightClick(InputAction.CallbackContext context) { }
    }
}
