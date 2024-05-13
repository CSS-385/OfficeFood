using OfficeFood.Highlight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Carry
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Carrier : MonoBehaviour
    {
        public UnityEvent CarryStarted = new UnityEvent();
        public UnityEvent CarryStopped = new UnityEvent();

        public Transform carryTransform = null;// Used by Carriable to reparent.
        public Transform carryTransformHeight = null;// Used by Carriable to simulate height for visuals.
        public Highlightable highlightable = null;// Used if this has Carriable and butterFingers is true

        [SerializeField, PostNormalize]
        public Vector2 queryDirection = Vector2.down;
        public float queryDistance = 0.25f;

        [SerializeField]
        private Carriable _startingCarriable = null;

        [SerializeField]
        private bool _canHighlight = false;// If this can highlight Carriables and Carriers (should only be set once in Inspector).

        [SerializeField]
        private bool _canSupply = false;// If other Carriers can take this Carrier's Carriable.

        private Carriable _carriable = null;// Current Carriable being carried.
        public Carriable carriable
        {
            get
            {
                return _carriable;
            }
            private set
            {
                if (_carriable != value)
                {
                    if (_carriable != null)
                    {
                        if (_carriable.carrier == this)
                        {
                            _carriable.carrier = null;
                        }
                        CarryStopped.Invoke();
                    }
                    _carriable = value;
                    if (_carriable != null)
                    {
                        if (_carriable.carrier != this)
                        {
                            _carriable.carrier = this;
                        }
                        CarryStarted.Invoke();
                    }
                }
            }
        }

        public bool HasCarriable()
        {
            return _carriable != null;
        }

        private Carriable _queryCarriable = null;// Carriable detected each FixedUpdate.
        private Carriable queryCarriable
        {
            get
            {
                return _queryCarriable;
            }
            set
            {
                if (_queryCarriable != value)
                {
                    if (_queryCarriable != null)
                    {
                        if (_canHighlight && (_queryCarriable.highlightable != null))
                        {
                            _queryCarriable.highlightable.DecrementHighlighterCount();
                        }
                    }
                    _queryCarriable = value;
                    if (_queryCarriable != null)
                    {
                        if (_canHighlight && (_queryCarriable.highlightable != null))
                        {
                            _queryCarriable.highlightable.IncrementHighlighterCount();
                        }
                    }
                }
            }
        }

        private Carrier _queryCarrier = null;// Carrier detected each FixedUpdate.
        private Carrier queryCarrier
        {
            get
            {
                return _queryCarrier;
            }
            set
            {
                if (_queryCarrier != value)
                {
                    if (_queryCarrier != null)
                    {
                        if (_canHighlight && (_queryCarrier.highlightable) != null)
                        {
                            _queryCarrier.highlightable.DecrementHighlighterCount();
                        }
                    }
                    _queryCarrier = value;
                    if (_queryCarrier != null)
                    {
                        if (_canHighlight && (_queryCarrier.highlightable) != null)
                        {
                            _queryCarrier.highlightable.IncrementHighlighterCount();
                        }
                    }
                }
            }
        }

        private Rigidbody2D _rigidbody = null;
        public Rigidbody2D GetRigidbody()
        {
            return _rigidbody;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            if (_startingCarriable != null)
            {
                carriable = _startingCarriable;
            }
        }

        private void Update()
        {
            if (HasCarriable())
            {
                _carriable._height = carryTransformHeight.position.y - carryTransform.position.y;
            }
        }

        private void FixedUpdate()
        {
            if (!isActiveAndEnabled)
            {
                queryCarriable = null;
                queryCarrier = null;
                return;
            }

            // Raycast.
            Vector2 origin = _rigidbody.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, queryDirection.normalized, queryDistance, LayerMask.GetMask("Default"));

            // le logic xd
            if (!HasCarriable())
            {
                queryCarriable = hit.collider?.transform?.GetComponent<Carriable>();
            }
            else
            {
                queryCarriable = null;
            }

            if (queryCarriable == null)
            {
                queryCarrier = hit.collider?.transform?.GetComponent<Carrier>();
            }
            else
            {
                queryCarrier = null;
            }

            if (HasCarriable() && _rigidbody.simulated && (_rigidbody.bodyType == RigidbodyType2D.Dynamic))
            {
                // Preserve momentum, add mass.
                Vector2 momentum = _rigidbody.mass * _rigidbody.velocity;
                float totalMass = _rigidbody.mass + _carriable.GetRigidbody().mass;
                _rigidbody.velocity = momentum / totalMass;
            }
        }

        public bool CanCarry()
        {
            if (HasCarriable())
            {
                return false;
            }

            if ((queryCarriable != null) && !queryCarriable.HasCarrier())
            {
                return true;
            }

            if ((queryCarrier != null) && queryCarrier.HasCarriable() && queryCarrier._canSupply)
            {
                return true;
            }

            return false;
        }

        public bool TakeCarriable()
        {
            if (HasCarriable())
            {
                return false;
            }

            if ((queryCarriable != null) && !queryCarriable.HasCarrier())
            {
                carriable = queryCarriable;
                return true;
            }
            
            if ((queryCarrier != null) && queryCarrier.HasCarriable() && queryCarrier._canSupply)
            {
                carriable = queryCarrier.carriable;
                queryCarrier.carriable = null;
                return true;
            }

            return false;
        }

        public bool DropCarriable()
        {
            if (!HasCarriable())
            {
                return false;
            }
            carriable = null;
            return true;
        }

        public bool GiveCarriable()
        {
            if (!HasCarriable())
            {
                return false;
            }
            if (queryCarrier == null || queryCarrier.HasCarriable())
            {
                return false;
            }
            _queryCarrier.carriable = carriable;
            carriable = null;
            return true;
        }
    }
}
