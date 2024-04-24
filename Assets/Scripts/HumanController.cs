using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// idea is to attach this script to any Human GameObject to control directly
// AI will most likely be another component controlling Human

public class HumanController : MonoBehaviour
{
    [SerializeField]
    private InputRelay _inputRelay = null;
    private bool _inputRelaySubscribed = false;

    private Human _human = null;

    private void Start()
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
        }
    }

    private void OnDisable()
    {
        if (_inputRelaySubscribed)
        {
            _inputRelay.GameMoveEvent -= OnInputRelayGameMove;
            _inputRelay.GameInteractEvent -= OnInputRelayGameInteract;
            _inputRelaySubscribed = false;
        }
    }

    private void OnInputRelayGameMove(Vector2 move)
    {
        _human.moveTarget = move;
    }

    private void OnInputRelayGameInteract(bool interact)
    {
        _human.interact = interact;
    }

}
