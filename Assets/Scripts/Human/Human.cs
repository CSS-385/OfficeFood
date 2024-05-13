using System;
using UnityEngine;
using OfficeFood.Carry;
using OfficeFood.Interact;
using Unity.Mathematics;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members

namespace OfficeFood.Human
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(Carrier)), RequireComponent(typeof(Interactor))]
    public class Human : MonoBehaviour
    {
        /* Properties */

        // Base movement speed.
        // Usually set by Inspector.
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

        // Base movement speed modifier.
        // Usually set by Animation (e.g. disallow movement during pick up animation).
        [SerializeField, HideInInspector, Range(0.0f, 1.0f)]
        private float _moveSpeedModifier = 1.0f;
        public float moveSpeedModifier
        {
            get
            {
                return _moveSpeedModifier;
            }
            set
            {
                _moveSpeedModifier = Mathf.Clamp01(value);
            }
        }

        // Sprint speed modifier.
        // Usually set by Inspector.
        [SerializeField, Min(1.0f)]
        private float _moveSprintModifier = 1.5f;
        public float moveSprintModifier
        {
            get
            {
                return _moveSprintModifier;
            }
            set
            {
                _moveSprintModifier = Mathf.Min(value, 1.0f);
            }
        }

        // Movement acceleration.
        // Usually set by Inspector.
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

        // Movement deceleration.
        // Usually set by Inspector.
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

        // Move modifier for walking and sprinting (> 1.0).
        // Can also vary walk speed with stick tilt/key pressure. (TODO)
        // Usually set by AI or Controller.
        private float _moveModifier = 1.0f;
        public float moveModifier
        {
            get
            {
                return _moveModifier;
            }
            set
            {
                _moveModifier = Mathf.Max(value, 0.0f);
            }
        }

        // Move direction for constant movement.
        // Usually set by Controller.
        private Vector2 _moveDirection = Vector2.zero;
        public Vector2 moveDirection
        {
            get
            {
                return _moveDirection;
            }
            set
            {
                _moveDirection = value.normalized;
            }
        }

        // Move target to move towards (global position). Useful for waypoints.
        // Will overshoot then decelerate.
        // Usually set by AI.
        private bool _useMoveTarget = false;
        private Vector2 _moveTarget = Vector2.zero;
        public void SetMoveTarget(Vector2 moveTarget)
        {
            _useMoveTarget = true;
            _moveTarget = moveTarget;
        }
        public Vector2 GetMoveTarget()
        {
            return _moveTarget;
        }
        public void ClearMoveTarget()
        {
            _useMoveTarget = false;
            _moveTarget = Vector2.zero;
        }
        public bool IsMoveTargetCleared()
        {
            return !_useMoveTarget;
        }

        // Minimum distance to moveTarget before stopping movement towards move target.
        // Once within threshold, starts decelerating.
        // Usually set by Inspector.
        [SerializeField, Min(0.0f)]
        private float _moveTargetThreshold = 1e-05f;
        public float moveTargetThreshold
        {
            get
            {
                return _moveTargetThreshold;
            }
            set
            {
                _moveTargetThreshold = Mathf.Max(value, 0.0f);
            }
        }

        // Facing direction (normalized).
        // Usually set by AI or Controller or Inspector.
        [SerializeField, PostNormalize]
        private Vector2 _faceDirection = Vector2.down;
        public Vector2 faceDirection
        {
            get
            {
                return _faceDirection;
            }
            set
            {
                if (value != Vector2.zero)
                {
                    _faceDirection = value.normalized;
                }
            }
        }

        // Interact.
        // Usually set by AI or Controller.
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
            /* Calculate movement */

            if (_useMoveTarget)
            {
                // Check distance between point moveTarget and the segment from last position to current position.
                Vector2 positionCurr = _rigidbody.position;
                Vector2 positionPrev = _rigidbody.position - (_rigidbody.velocity * Time.fixedDeltaTime);
                bool collinear = GetMoveTarget().IsCollinear(positionPrev, positionCurr, moveTargetThreshold);
                if (collinear)
                {
                    ClearMoveTarget();// moveTarget was reached (within threshold).
                }
            }

            Vector2 moveTargetVelocity = Vector2.zero;
            float accelerationMax = moveDeceleration;

            if (_useMoveTarget)
            {
                // Move towards moveTarget with overshooting.
                Vector2 moveTargetDelta = GetMoveTarget() - _rigidbody.position;
                float moveTargetSpeed = moveSpeed * moveSpeedModifier;
                moveTargetVelocity = moveTargetDelta.normalized * moveTargetSpeed;
                accelerationMax = moveAcceleration;
            }
            else if (moveDirection != Vector2.zero)
            {
                // Move towards direction of moveDirection.
                float moveTargetSpeed = moveSpeed * moveSpeedModifier;
                moveTargetVelocity = moveDirection * moveTargetSpeed;
                accelerationMax = moveAcceleration;
            }

            // Find acceleration to apply using desired velocity and current velocity.
            Vector2 acceleration = (moveTargetVelocity - _rigidbody.velocity) / Time.fixedDeltaTime;
            acceleration = Vector2.ClampMagnitude(acceleration, accelerationMax);
            _rigidbody.AddForce(_rigidbody.mass * acceleration, ForceMode2D.Force);

            /* Face direction */
            if (faceDirection == Vector2.zero)
            {
                faceDirection = Vector2.down;// Fix possible user stupidity.
            }
            _carrier.queryDirection = faceDirection;
            _interactor.queryDirection = faceDirection;

            /* Interact stuff */

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
                else if (_carrier.HasCarriable())
                {
                    animParamCarryDrop = true;
                }
            }

            // Animator parameters
            // Animator MoveSpeed and SmoothMoveSpeed
            if (_rigidbody.velocity.sqrMagnitude < 0.01f)
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
            if (faceDirection != Vector2.zero)
            {
                // Snap face direction to nearest cardinal angle.
                const float CardinalAngle = Mathf.PI / 2.0f;
                float moveTargetAngle = Mathf.Atan2(faceDirection.y, faceDirection.x);
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
            //FaceDirection = new Vector2(_animator.GetFloat(_animParamFaceX), _animator.GetFloat(_animParamFaceY));
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
            if (_carrier.TakeCarriable())
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
            if (!_carrier.GiveCarriable())
            {
                _carrier.DropCarriable();
            }
        }
    }
}
