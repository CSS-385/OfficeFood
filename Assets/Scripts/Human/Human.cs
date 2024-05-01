using System;
using UnityEngine;
using OfficeFood.Carry;
using OfficeFood.Interact;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members

// TODO: fix sorting w/ SortingGroup and carriable (layer property is not exposed to animation!)

namespace OfficeFood.Human
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(Carrier)), RequireComponent(typeof(Interactor))]
    public class Human : MonoBehaviour
    {
        // Move properties (primarily set by Inspector)
        [SerializeField, Min(0.0f)]
        private float _moveSpeed = 2.0f;// Units per second.
        public float moveSpeed
        {
            get
            {
                return _moveSpeed;
            }
            set
            {
                _moveSpeed = Mathf.Max(value, 0.0f);
            }
        }

        [SerializeField, HideInInspector, Range(0.0f, 1.0f)]
        private float _moveModifier = 1.0f;// Move speed multiplier. Set by Animation.
        public float moveModifier
        {
            get
            {
                return _moveModifier;
            }
            set
            {
                _moveModifier = Mathf.Clamp01(value);
            }
        }

        [SerializeField, Min(0.0f)]
        private float _moveAcceleration = 16.0f;
        public float moveAcceleration
        {
            get
            {
                return _moveAcceleration;
            }
            set
            {
                _moveAcceleration = Mathf.Max(value, 0.0f);
            }
        }

        [SerializeField, Min(0.0f)]
        private float _moveDeceleration = 64.0f;
        public float moveDeceleration
        {
            get
            {
                return _moveDeceleration;
            }
            set
            {
                _moveDeceleration = Mathf.Max(value, 0.0f);
            }
        }

        // Move Target properties (set by Controller/AI)
        private Vector2 _moveTarget = Vector2.zero;// Relative position to move towards. Will not overshoot.
        public Vector2 moveTarget
        {
            get
            {
                return _moveTarget;
            }
            set
            {
                _moveTarget = value;
            }
        }

        private float _moveTargetModifier = 1.0f;// Move speed multiplier. (For e.g. stick tilt).
        public float moveTargetModifier
        {
            get
            {
                return _moveTargetModifier;
            }
            set
            {
                _moveTargetModifier = Mathf.Clamp01(value);
            }
        }

        // Interact properties (set by Controller/AI)
        private bool _interact = false;
        private bool _interactOnce = false;
        public bool interact
        {
            get
            {
                return _interact;
            }
            set
            {
                if (_interact != value)
                {
                    _interact = value;
                    _interactOnce = _interact && _interactOnce;
                }
            }
        }

        // Animation Parameters
        private readonly int _animLayerMain = 0;
        private readonly int _animLayerHead = 1;
        private readonly int _animLayerCarry = 2;

        private readonly int _animParamFaceX = Animator.StringToHash("FaceX");
        private readonly int _animParamFaceY = Animator.StringToHash("FaceY");
        private readonly int _animParamMove = Animator.StringToHash("Move");
        private readonly int _animParamSmoothFaceX = Animator.StringToHash("SmoothFaceX");
        private readonly int _animParamSmoothFaceY = Animator.StringToHash("SmoothFaceY");
        private readonly int _animParamSmoothFaceSpeed = Animator.StringToHash("SmoothFaceSpeed");
        private readonly int _animParamSmoothMove = Animator.StringToHash("SmoothMove");
        private readonly int _animParamSmoothMoveSpeed = Animator.StringToHash("SmoothMoveSpeed");
        private readonly int _animParamCarryAttempt = Animator.StringToHash("CarryAttempt");
        private readonly int _animParamCarrySuccess = Animator.StringToHash("CarrySuccess");
        private readonly int _animParamCarryFailure = Animator.StringToHash("CarryFailure");
        private readonly int _animParamCarryDrop = Animator.StringToHash("CarryDrop");

        // Components
        private Rigidbody2D _rigidbody = null;
        private Animator _animator = null;
        private Carrier _carrier = null;
        private Carriable _carriable = null;
        private Interactor _interactor = null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _carrier = GetComponent<Carrier>();
            _carriable = GetComponent<Carriable>();
            _interactor = GetComponent<Interactor>();
        }

        private void FixedUpdate()
        {
            // Rigidbody math
            Vector2 moveTargetDirection = moveTarget.normalized;
            float moveTargetSpeed = Mathf.Clamp(moveTarget.magnitude, 0.0f, _moveSpeed * _moveModifier * _moveTargetModifier);
            Vector2 moveTargetVelocity = moveTargetDirection * moveTargetSpeed;

            Vector2 accelerationPrevious = _rigidbody.velocity / Time.fixedDeltaTime;
            Vector2 accelerationTarget = moveTargetVelocity / Time.fixedDeltaTime;
            Vector2 acceleration = accelerationTarget - accelerationPrevious;

            // Simple acceleration.
            float accelerationMax = moveTargetSpeed > 0.0f ? moveAcceleration : moveDeceleration;
            acceleration = Vector2.ClampMagnitude(acceleration, accelerationMax);
            _rigidbody.AddForce(_rigidbody.mass * acceleration, ForceMode2D.Force);

            // Query directions
            if (moveTargetSpeed > 0.0f)
            {
                _carrier.queryDirection = moveTargetDirection;
                _interactor.queryDirection = moveTargetDirection;
            }

            // Interact stuff
            bool animParamCarryAttempt = false;
            bool animParamCarryDrop = false;
            if (_interact && !_interactOnce)
            {
                _interactOnce = true;
                if (_interactor.Interact())
                {

                }
                else if (_carrier.CanCarry())
                {
                    animParamCarryAttempt = true;
                }
                else if (_carrier.IsCarrying())
                {
                    animParamCarryDrop = true;
                }
            }

            // Animator parameters
            // Animator MoveSpeed and SmoothMoveSpeed
            if (Mathf.Approximately(moveTargetSpeed, 0.0f) || Mathf.Approximately(_rigidbody.velocity.sqrMagnitude, 0.0f))
            {
                _animator.SetFloat(_animParamMove, 0.0f);// Idle
                                                         // Linear interpolate smooth move towards 0.0f.
                float animSmoothMove = _animator.GetFloat(_animParamSmoothMove);
                float animSmoothMoveSpeed = _animator.GetFloat(_animParamSmoothMoveSpeed);
                _animator.SetFloat(_animParamSmoothMove, Mathf.Lerp(animSmoothMove, 0.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
            }
            else
            {
                _animator.SetFloat(_animParamMove, 1.0f);// Move
                                                         // Linear interpolate smooth move towards 1.0f.
                float animSmoothMove = _animator.GetFloat(_animParamSmoothMove);
                float animSmoothMoveSpeed = _animator.GetFloat(_animParamSmoothMoveSpeed);
                _animator.SetFloat(_animParamSmoothMove, Mathf.Lerp(animSmoothMove, 1.0f, animSmoothMoveSpeed * Time.fixedDeltaTime));
            }

            // Animator FaceX/FaceY and SmoothFaceX/SmoothFaceY
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

            _animator.SetBool(_animParamCarryAttempt, animParamCarryAttempt);
            _animator.SetBool(_animParamCarryDrop, animParamCarryDrop);

            _carryAttempted = false;// temporary fix
        }

        private bool _carryAttempted = false;// temporary fix (blended animations call event twice!)
        // will probably just animate a bool field and query carry attempt in fixedupdate
        // Called by Animation event.
        private void CarryAttempt()
        {
            if (_carryAttempted)
            {
                return;
            }
            _carryAttempted = true;
            if (_carrier.TryCarry())
            {
                _animator.SetTrigger(_animParamCarrySuccess);
            }
            else
            {
                _animator.SetTrigger(_animParamCarryFailure);
            }
        }

        private void CarryDrop()
        {
            _carrier.Drop();
        }
    }
}
