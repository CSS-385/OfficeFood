using UnityEngine;
using UnityEngine.InputSystem;

// idea is to attach this script to any Human GameObject to control directly
// AI will be another component controlling Human

namespace OfficeFood.Human
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Human))]
    public class HumanController : MonoBehaviour
    {
        [SerializeField]
        private InputRelay _inputRelay = null;
        private bool _inputRelaySubscribed = false;

        private Human _human = null;

        private void Awake()
        {
            _human = GetComponent<Human>();
        }

        private void OnEnable()
        {
            if (_inputRelay != null)
            {
                _inputRelay.GameMoveEvent += OnInputRelayGameMove;
                _inputRelay.GameInteractEvent += OnInputRelayGameInteract;
                _inputRelaySubscribed = true;

                Vector2 position = new Vector2(transform.position.x, transform.position.y);
                _human.SetMoveTarget(position + (_inputRelay.GameMove * _human.moveSpeed));
                _human.moveTargetModifier = _inputRelay.GameMove.magnitude;
                _human.interact = _inputRelay.GameInteract;
            }
        }

        private void OnDisable()
        {
            if (_inputRelaySubscribed)
            {
                _inputRelay.GameMoveEvent -= OnInputRelayGameMove;
                _inputRelay.GameInteractEvent -= OnInputRelayGameInteract;
                _inputRelaySubscribed = false;

                _human.ClearMoveTarget();
                _human.moveTargetModifier = 0.0f;
                _human.interact = false;
            }
        }

        private void FixedUpdate()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            _human.SetMoveTarget(position + (_inputRelay.GameMove * _human.moveSpeed));
            _human.moveTargetModifier = _inputRelay.GameMove.magnitude;
        }

        private void OnInputRelayGameMove(Vector2 move)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            _human.SetMoveTarget(position + (move * _human.moveSpeed));
            _human.moveTargetModifier = move.magnitude;
        }

        private void OnInputRelayGameInteract(bool interact)
        {
            _human.interact = interact;
        }
    }
}