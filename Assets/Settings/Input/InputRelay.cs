using System;

namespace UnityEngine.InputSystem
{
    public class InputRelay : ScriptableObject, Input.IGameActions, Input.IMenuActions
    {
        // poll everything (IMO, events can get messy and many are required for advanced behavior)
        public Vector2 gameMove = Vector2.zero;
        public bool gameInteract = false;

        private Input _input = null;

        private void OnEnable()
        {
            if (_input == null)
            {
                _input = new Input();
                _input.Game.SetCallbacks(this);
                _input.Menu.SetCallbacks(this);
            }
            // TODO: eventually game state directly switches input modes
            _input.Game.Enable();
            _input.Menu.Disable();
        }

        private void OnDisable()
        {
            if (_input != null)
            {
                _input.Game.Disable();
                _input.Menu.Disable();
            }
        }

        void Input.IGameActions.OnMove(InputAction.CallbackContext context)
        {
            gameMove = context.ReadValue<Vector2>();
        }

        void Input.IGameActions.OnInteract(InputAction.CallbackContext context)
        {
            gameInteract = context.ReadValueAsButton();
        }
    }
}
