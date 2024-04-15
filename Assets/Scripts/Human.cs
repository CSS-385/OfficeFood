using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// potential problem: if position is set externally (e.g. teleport), FixedDelta speedResult will be innaccurate
// consequences are minimal tho

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class Human : MonoBehaviour
{
    /* Move Fields */
    public float moveSpeed = 4.0f;// units per second
    public float moveAcceleration = 6.0f;
    public float moveDeceleration = 8.0f;
    public Vector2 moveTarget = Vector2.zero;

    /* Face Fields */ // TODO
    public float faceSpeed = 360.0f;// for visuals (animation) // degrees per second ; TODO

    /* Components */
    private Rigidbody2D _rigidbody = null;
    private Animator _animator = null;

    private Vector2 _positionPrev = Vector2.zero;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _positionPrev = _rigidbody.position;
    }

    // animation notes:
    // animator layer sync share transitions (fade same across layers) (inconvenient)
    // layer indexes start from top layer as 0
    // parameters suck. animator sucks.
    // discrepency between animator and code state might be a problem
    // easier to just use code for state and set animations directly
    // make sure layer weight is set to 1 or nothing animates (0 is default. very cool Unity!)

    private const int ANIM_LAYER_BODY = 0;
    private const int ANIM_LAYER_HAND = 1;

    private int _animIdle = Animator.StringToHash("Idle");
    private int _animWalk = Animator.StringToHash("Walk");
    private void SetAnimation(int hash)
    {
        // keep animations synchronized across layers without using layer sync
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(ANIM_LAYER_BODY);
        if (info.shortNameHash != hash)
        {
            _animator.PlayInFixedTime(hash, ANIM_LAYER_BODY);
            _animator.CrossFadeInFixedTime(hash, 0.125f, ANIM_LAYER_HAND);
        }
    }

    private void FixedUpdate()
    {
        // Movement
        Vector2 moveTargetDirection = moveTarget.normalized;
        float moveTargetSpeed = moveTargetDirection.magnitude;

        float positionDelta = (_rigidbody.position - _positionPrev).magnitude;
        float speedResult = Mathf.Clamp(positionDelta, 0.0f, moveSpeed) / Time.fixedDeltaTime;
        _positionPrev = _rigidbody.position;// NOTE: position isnt immediately updated after MovePosition()

        float acceleration = !Mathf.Approximately(moveTargetSpeed, 0.0f) ? moveAcceleration : moveDeceleration;

        float speed = Mathf.MoveTowards(speedResult, moveTargetSpeed * moveSpeed, acceleration * Time.fixedDeltaTime);
        var movePosition = _rigidbody.position + (moveTargetDirection * Mathf.Clamp(speed, 0.0f, moveSpeed) * Time.fixedDeltaTime);
        _rigidbody.MovePosition(movePosition);

        //_animator.PlayInFixedTime();
        //AnimatorStateInfo
        //_animator.GetCurrentAnimatorStateInfo
        const float SPEED_THRESHOLD = 0.0125f;
        if (speedResult < SPEED_THRESHOLD)
        {
            SetAnimation(_animIdle);
        } else
        {
            SetAnimation(_animWalk);
        }
    }
}
