using System;
using UnityEngine;

// todo: move and slide along walls
// todo: no move animation when not moving?

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(Carrier))]
public class Human : MonoBehaviour
{
    // Move
    public float moveSpeed = 2.0f;// units per second
    public float moveStrength = 64.0f;
    public float moveAcceleration = 16.0f;
    public float moveDeceleration = 32.0f;
    public Vector2 moveTarget = Vector2.zero;

    // Interact
    public bool interact = false;
    private bool _interactOnce = false;

    // Animation Parameters
    //private readonly int _animLayerMain = 0;
    //private readonly int _animLayerHead = 1;
    private readonly int _animLayerCarry = 2;

    private readonly int _animParamFaceX = Animator.StringToHash("FaceX");
    private readonly int _animParamFaceY = Animator.StringToHash("FaceY");
    private readonly int _animParamMove = Animator.StringToHash("Move");
    private readonly int _animParamSmoothFaceX = Animator.StringToHash("SmoothFaceX");
    private readonly int _animParamSmoothFaceY = Animator.StringToHash("SmoothFaceY");
    private readonly int _animParamSmoothFaceSpeed = Animator.StringToHash("SmoothFaceSpeed");
    private readonly int _animParamSmoothMove = Animator.StringToHash("SmoothMove");
    private readonly int _animParamSmoothMoveSpeed = Animator.StringToHash("SmoothMoveSpeed");
    private readonly int _animParamCarry = Animator.StringToHash("Carry");
    private readonly int _animParamCarryDrop = Animator.StringToHash("CarryDrop");

    // Components
    private Rigidbody2D _rigidbody = null;
    private Animator _animator = null;
    private Carrier _carrier = null;
    private Carriable _carriable = null;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _carrier = GetComponent<Carrier>();
        _carriable = GetComponent<Carriable>();
    }

    private void FixedUpdate()
    {
        // Rigidbody math
        Vector2 moveTargetVelocity = moveTarget * moveSpeed;
        Vector2 moveTargetDirection = moveTargetVelocity.normalized;
        float moveTargetSpeed = moveTargetVelocity.magnitude;

        float speedResult = _rigidbody.velocity.magnitude;// TODO: dot with actual movement vector?

        Vector2 accelerationPrevious = _rigidbody.velocity / Time.fixedDeltaTime;
        Vector2 accelerationTarget = moveTargetVelocity / Time.fixedDeltaTime;
        Vector2 acceleration = accelerationTarget - accelerationPrevious;

        // Simple implementation of acceleration.
        float accelerationMax = !Mathf.Approximately(moveTargetSpeed, 0.0f) ? moveAcceleration : moveDeceleration;
        if (acceleration.sqrMagnitude > (accelerationMax * accelerationMax))
        {
            acceleration = acceleration.normalized * accelerationMax;
        }
        _rigidbody.AddForce(moveStrength * acceleration, ForceMode2D.Force);

        // Carrier
        if (moveTargetSpeed > 0.0f)
        {
            _carrier.queryDirection = moveTargetDirection;
        }

        // Interact stuff
        // atm, lift functionality is bundled in with interact
        bool animParamCarry = false;
        bool animParamCarryDrop = false;
        if (!interact)
        {
            _interactOnce = false;
        }
        else if (!_interactOnce)
        {
            _interactOnce = true;
            // TODO: first check for interact, then attempt carry
            if (_carrier.CanCarry())
            {
                animParamCarry = true;
            } else if (_carrier.IsCarrying())
            {
                animParamCarryDrop = true;
            }
        }

        // _animator parameters
        // _animator MoveSpeed and SmoothMoveSpeed
        const float SpeedThreshold = 0.0125f;
        if (speedResult < SpeedThreshold)
        {
            _animator.SetFloat(_animParamMove, 0.0f);
            // Linear interpolate smooth move towards 0.0f.
            float animSmoothMove = _animator.GetFloat(_animParamSmoothMove);
            float animSmoothMoveSpeed = _animator.GetFloat(_animParamSmoothMoveSpeed);
            _animator.SetFloat(_animParamSmoothMove, Mathf.Lerp(animSmoothMove, 0.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
        } else
        {
            _animator.SetFloat(_animParamMove, 1.0f);
            // Linear interpolate smooth move towards 1.0f.
            float animSmoothMove = _animator.GetFloat(_animParamSmoothMove);
            float animSmoothMoveSpeed = _animator.GetFloat(_animParamSmoothMoveSpeed);
            _animator.SetFloat(_animParamSmoothMove, Mathf.Lerp(animSmoothMove, 1.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
        }

        // _animator FaceX/FaceY and SmoothFaceX/SmoothFaceY
        Vector2 animFace = new Vector2(_animator.GetFloat(_animParamFaceX), _animator.GetFloat(_animParamFaceY));
        if (moveTargetSpeed > 0.0f)
        {
            // Snap face direction to nearest cardinal angle.
            const float CardinalAngle = Mathf.PI / 2.0f;
            float moveTargetAngle = Mathf.Atan2(moveTargetDirection.y, moveTargetDirection.x);
            moveTargetAngle = Mathf.Round(moveTargetAngle / CardinalAngle) * CardinalAngle;
            animFace = new Vector2(Mathf.Cos(moveTargetAngle), Mathf.Sin(moveTargetAngle));
            _animator.SetFloat(_animParamFaceX, animFace.x);
            _animator.SetFloat(_animParamFaceY, animFace.y);
        }

        // Linear rotate smooth face direction towards face direction.
        float animFaceAngle = Mathf.Atan2(animFace.y, animFace.x);
        Vector2 animSmoothFace = new Vector2(_animator.GetFloat(_animParamSmoothFaceX), _animator.GetFloat(_animParamSmoothFaceY));
        float animSmoothFaceSpeed = _animator.GetFloat(_animParamSmoothFaceSpeed);
        float animSmoothFaceAngle = MathF.Atan2(animSmoothFace.y, animSmoothFace.x);
        // why is Mathf.MoveTowardsAngle in degrees if trig functions are in radians? fuck off unity
        const float Rad2Deg = 180.0f / Mathf.PI;
        animSmoothFaceAngle = Mathf.MoveTowardsAngle(Rad2Deg * animSmoothFaceAngle, Rad2Deg * animFaceAngle, Rad2Deg * animSmoothFaceSpeed * Time.fixedDeltaTime) / Rad2Deg;
        animSmoothFace = new Vector2(Mathf.Cos(animSmoothFaceAngle), Mathf.Sin(animSmoothFaceAngle));
        _animator.SetFloat(_animParamSmoothFaceX, animSmoothFace.x);
        _animator.SetFloat(_animParamSmoothFaceY, animSmoothFace.y);

        _animator.SetBool(_animParamCarry, animParamCarry);
        _animator.SetBool(_animParamCarryDrop, animParamCarryDrop);
        if (_carrier.IsCarrying())
        {
            _animator.SetLayerWeight(_animLayerCarry, 1.0f);
        }
        else
        {

            _animator.SetLayerWeight(_animLayerCarry, 0.0f);
        }
    }
}
