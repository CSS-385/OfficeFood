using UnityEngine;
using UnityEngine.Events;
using OfficeFood.Highlight;
using UnityEngine.Animations;

namespace OfficeFood.Carry
{
    [RequireComponent(typeof(PositionConstraint))]
    public class Carriable : MonoBehaviour
    {
        public UnityEvent CarriedStarted = new UnityEvent();
        public UnityEvent CarriedStopped = new UnityEvent();

        public Transform heightTransform = null;// Visuals transform to simulate height.
        public Highlightable highlightable = null;// Used if a Carrier can carry this.

        public float fallAcceleration = 4.0f;

        private Carrier _carrier = null;
        public Carrier carrier
        {
            get
            {
                return _carrier;
            }
            internal set
            {
                if (_carrier != value)
                {
                    if (_carrier != null)
                    {
                        rigidbody.simulated = true;// ? move this somewhere else? perhaps fixed update

                        while (_positionConstraint.sourceCount > 0)
                        {
                            _positionConstraint.RemoveSource(0);// Handle user stupidity.
                        }
                        _positionConstraint.locked = true;
                        _positionConstraint.constraintActive = true;

                        CarriedStopped.Invoke();
                    }
                    _carrier = value;
                    if (_carrier != null)
                    {
                        rigidbody.simulated = false;// ? move this somewhere else? perhaps fixed update

                        ConstraintSource constraintSource = new ConstraintSource();
                        constraintSource.weight = 1.0f;
                        constraintSource.sourceTransform = _carrier.carryTransform;
                        _positionConstraint.AddSource(constraintSource);
                        _positionConstraint.locked = true;
                        _positionConstraint.constraintActive = true;

                        CarriedStarted.Invoke();
                    }
                }
            }
        }

        public bool HasCarrier()
        {
            return _carrier != null;
        }

        internal float _height = 0.0f;// Set by Carrier.
        private float _heightPrev = 0.0f;// Prevent the thing.
        private float _fallSpeed = 0.0f;

        public Rigidbody2D rigidbody = null;
        private PositionConstraint _positionConstraint = null;

        private void Awake()
        {
            _positionConstraint = GetComponent<PositionConstraint>();
        }

        private void LateUpdate()
        {
            if (_height != _heightPrev)
            {
                heightTransform.position += new Vector3(0.0f, _height - _heightPrev, 0.0f);
                _heightPrev = _height;
            }
            if (HasCarrier() || _height == 0.0f)
            {
                _fallSpeed = 0.0f;
            }
            else
            {
                _fallSpeed += fallAcceleration * Time.deltaTime * Time.deltaTime;
                _height -= _fallSpeed;
                if (_height < 0.0f)
                {
                    _height = 0.0f;
                }
            }
        }

        public bool IsFalling()
        {
            return _fallSpeed > 0.0f;
        }
    }
}
