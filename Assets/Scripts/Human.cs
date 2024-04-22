using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// potential problem: if position is set externally (e.g. teleport), FixedDelta speedResult will be innaccurate
// consequences are minimal tho

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class Human : MonoBehaviour
{
    // Move
    public float moveSpeed = 2.0f;// units per second
    public float moveAcceleration = 8.0f;
    public float moveDeceleration = 16.0f;
    public Vector2 moveTarget = Vector2.zero;

    // Animation
    private int AnimFaceX = Animator.StringToHash("FaceX");
    private int AnimFaceY = Animator.StringToHash("FaceY");
    private int AnimMove = Animator.StringToHash("Move");
    private int AnimSmoothFaceX = Animator.StringToHash("SmoothFaceX");
    private int AnimSmoothFaceY = Animator.StringToHash("SmoothFaceY");
    private int AnimSmoothFaceSpeed = Animator.StringToHash("SmoothFaceSpeed");
    private int AnimSmoothMove = Animator.StringToHash("SmoothMove");
    private int AnimSmoothMoveSpeed = Animator.StringToHash("SmoothMoveSpeed");
    private int AnimEvent = Animator.StringToHash("Event");

    // Components
    private Rigidbody2D _rigidbody = null;
    private Animator _animator = null;

    // Misc
    private Vector2 _positionPrev = Vector2.zero;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _positionPrev = _rigidbody.position;
    }

    private void FixedUpdate()
    {
        // Rigidbody math
        Vector2 moveTargetDirection = moveTarget.normalized;
        float moveTargetSpeed = moveTargetDirection.magnitude;

        float positionDelta = (_rigidbody.position - _positionPrev).magnitude;
        float speedResult = Mathf.Clamp(positionDelta, 0.0f, moveSpeed) / Time.fixedDeltaTime;
        _positionPrev = _rigidbody.position;// NOTE: position isnt immediately updated after MovePosition()

        float acceleration = !Mathf.Approximately(moveTargetSpeed, 0.0f) ? moveAcceleration : moveDeceleration;

        float speed = Mathf.MoveTowards(speedResult, moveTargetSpeed * moveSpeed, acceleration * Time.fixedDeltaTime);
        var movePosition = _rigidbody.position + (moveTargetDirection * Mathf.Clamp(speed, 0.0f, moveSpeed) * Time.fixedDeltaTime);
        _rigidbody.MovePosition(movePosition);

        // Animator parameters
        // Animator MoveSpeed and SmoothMoveSpeed
        const float SpeedThreshold = 0.0125f;
        if (speedResult < SpeedThreshold)
        {
            _animator.SetFloat(AnimMove, 0.0f);
            // Linear interpolate smooth move towards 0.0f.
            float animSmoothMove = _animator.GetFloat(AnimSmoothMove);
            float animSmoothMoveSpeed = _animator.GetFloat(AnimSmoothMoveSpeed);
            _animator.SetFloat(AnimSmoothMove, Mathf.Lerp(animSmoothMove, 0.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
        } else
        {
            _animator.SetFloat(AnimMove, 1.0f);
            // Linear interpolate smooth move towards 1.0f.
            float animSmoothMove = _animator.GetFloat(AnimSmoothMove);
            float animSmoothMoveSpeed = _animator.GetFloat(AnimSmoothMoveSpeed);
            _animator.SetFloat(AnimSmoothMove, Mathf.Lerp(animSmoothMove, 1.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
        }

        // Animator FaceX/FaceY and SmoothFaceX/SmoothFaceY
        Vector2 animFace = new Vector2(_animator.GetFloat(AnimFaceX), _animator.GetFloat(AnimFaceY));
        if (moveTargetSpeed > 0.0f)
        {
            // Snap face direction to nearest cardinal angle.
            const float CardinalAngle = Mathf.PI / 2.0f;
            float moveTargetAngle = Mathf.Atan2(moveTargetDirection.y, moveTargetDirection.x);
            moveTargetAngle = Mathf.Round(moveTargetAngle / CardinalAngle) * CardinalAngle;
            animFace = new Vector2(Mathf.Cos(moveTargetAngle), Mathf.Sin(moveTargetAngle));
            _animator.SetFloat(AnimFaceX, animFace.x);
            _animator.SetFloat(AnimFaceY, animFace.y);
        }

        // Linear rotate smooth face direction towards face direction.
        float animFaceAngle = Mathf.Atan2(animFace.y, animFace.x);
        Vector2 animSmoothFace = new Vector2(_animator.GetFloat(AnimSmoothFaceX), _animator.GetFloat(AnimSmoothFaceY));
        float animSmoothFaceSpeed = _animator.GetFloat(AnimSmoothFaceSpeed);
        float animSmoothFaceAngle = MathF.Atan2(animSmoothFace.y, animSmoothFace.x);
        // why is Mathf.MoveTowardsAngle in degrees if trig functions are in radians? fuck off unity
        const float Rad2Deg = 180.0f / Mathf.PI;
        animSmoothFaceAngle = Mathf.MoveTowardsAngle(Rad2Deg * animSmoothFaceAngle, Rad2Deg * animFaceAngle, Rad2Deg * animSmoothFaceSpeed * Time.fixedDeltaTime) / Rad2Deg;
        animSmoothFace = new Vector2(Mathf.Cos(animSmoothFaceAngle), Mathf.Sin(animSmoothFaceAngle));
        _animator.SetFloat(AnimSmoothFaceX, animSmoothFace.x);
        _animator.SetFloat(AnimSmoothFaceY, animSmoothFace.y);
    }
}
