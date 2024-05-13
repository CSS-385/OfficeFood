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

                _human.moveDirection = _inputRelay.GameMove.normalized;
                _human.moveModifier = _inputRelay.GameMove.magnitude;
                _human.faceDirection = _inputRelay.GameMove.normalized;
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

                _human.moveDirection = Vector2.zero;
                _human.moveModifier = 0.0f;
                _human.faceDirection = Vector2.zero;
                _human.interact = false;
            }
        }

        private void FixedUpdate()
        {
            _human.moveDirection = _inputRelay.GameMove.normalized;
            _human.moveModifier = _inputRelay.GameMove.magnitude;
            _human.faceDirection = _inputRelay.GameMove.normalized;
        }

        private void OnInputRelayGameMove(Vector2 move)
        {
            _human.moveDirection = move.normalized;
            _human.moveModifier = move.magnitude;
            _human.faceDirection = move.normalized;
        }

        private void OnInputRelayGameInteract(bool interact)
        {
            _human.interact = interact;
        }
    }
}
