using UnityEngine;

// TODO: sprite grouping/sorting
// TODO: physics for carriable weight
// TODO: shelves/tables and shafts/slides/chutes

namespace OfficeFood.Carry
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Carrier : MonoBehaviour
    {
        public Transform carryTransform = null;// Used by Carriable to reparent.
        public Transform carryTransformHeight = null;// Used by Carriable to simulate height for visuals.

        public Vector2 queryDirection = Vector2.down;
        public float queryRange = 0.25f;

        private Carriable _carriable = null;
        private bool _carriableSimulated = false;// Preserve Carriable rigidbody simulated.
        private Transform _carriableParent = null;// Track previous parent of carriable.

        private Rigidbody2D _rigidbody = null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!IsCarrying())
            {
                return;
            }
            _carriable._height = carryTransformHeight.position.y - carryTransform.position.y;
        }

        private void FixedUpdate()
        {
            if (!IsCarrying())
            {
                return;
            }
            // Preserve momentum, add mass.
            Vector2 momentum = _rigidbody.mass * _rigidbody.velocity;
            float totalMass = _rigidbody.mass + _carriable._rigidbody.mass;
            _rigidbody.velocity = momentum / totalMass;
        }

        public bool IsCarrying()
        {
            return _carriable != null;
        }

        public Carriable GetCarriable()
        {
            return _carriable;
        }

        public bool CanCarry()
        {
            if (IsCarrying())
            {
                return false;
            }
            RaycastHit2D hit = Physics2D.Raycast(_rigidbody.position, queryDirection.normalized, queryRange, LayerMask.GetMask("Default"));
            Carriable carriable = hit.transform?.GetComponent<Carriable>();
            return (carriable != null) && (carriable.gameObject != gameObject) && !carriable.IsCarried();
        }

        public bool AttemptCarry()
        {
            if (IsCarrying())
            {
                return false;
            }
            RaycastHit2D hit = Physics2D.Raycast(_rigidbody.position, queryDirection.normalized, queryRange, LayerMask.GetMask("Default"));
            Carriable carriable = hit.transform?.GetComponent<Carriable>();
            if ((carriable == null) || (carriable.gameObject == gameObject) || carriable.IsCarried())
            {
                return false;
            }
            _carriable = carriable;
            _carriable._isCarried = true;
            _carriableParent = _carriable.transform.parent;
            _carriable.transform.parent = carryTransform;
            _carriable.transform.position = carryTransform.position;
            _carriableSimulated = _carriable._rigidbody.simulated;
            _carriable._rigidbody.simulated = false;
            return true;
        }

        public bool Drop()
        {
            if (!IsCarrying())
            {
                return false;
            }
            _carriable._isCarried = false;
            _carriable.transform.parent = _carriableParent;
            _carriable._rigidbody.simulated = _carriableSimulated;
            _carriable = null;
            return true;
        }

        public bool Throw(Vector2 force)
        {
            if (!IsCarrying())
            {
                return false;
            }
            _carriable._isCarried = false;
            _carriable.transform.parent = _carriableParent;
            _carriable._rigidbody.simulated = _carriableSimulated;
            _carriable._rigidbody.AddForce(force, ForceMode2D.Impulse);
            _carriable = null;
            return true;
        }
    }
}
