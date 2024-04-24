using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Carrier : MonoBehaviour
{
    public Vector2 queryDirection = Vector2.down;
    public float queryRange = 0.25f;

    public Transform carryTransform = null;// Used by Carriable to track transform to.
    public Transform carryTransformHeight = null;// Used by Carriable to simulate height relative to carryTransform.

    private Carriable _carriable = null;

    private Rigidbody2D _rigidbody = null;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public bool IsCarrying()
    {
        return _carriable != null;
    }

    public bool CanCarry()
    {
        if (IsCarrying())
        {
            return false;
        }
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        LayerMask layerMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(position, queryDirection.normalized, queryRange, layerMask);
        Carriable carriable = hit.transform?.GetComponent<Carriable>();
        return (carriable != null) && (carriable.gameObject != gameObject) && carriable.IsCarriable();
    }

    public bool AttemptCarry()
    {
        if (IsCarrying())
        {
            return false;
        }
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        LayerMask layerMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(position, queryDirection.normalized, queryRange, layerMask);
        Carriable carriable = hit.transform?.GetComponent<Carriable>();
        if ((carriable == null) || (carriable.gameObject == gameObject) || !carriable.SetCarryTransform(carryTransform, carryTransformHeight))
        {
            return false;
        }
        _carriable = carriable;
        Rigidbody2D carriableRigidBody = _carriable.GetRigidbody2D();
        _rigidbody.mass += carriableRigidBody.mass;
        Collider2D[] colliders = new Collider2D[_rigidbody.attachedColliderCount];
        _rigidbody.GetAttachedColliders(colliders);
        Collider2D[] carriableColliders = new Collider2D[carriableRigidBody.attachedColliderCount];
        carriableRigidBody.GetAttachedColliders(carriableColliders);
        foreach (Collider2D collider in colliders)
        {
            foreach (Collider2D carriableCollider in carriableColliders)
            {
                Physics2D.IgnoreCollision(collider, carriableCollider, true);
            }
        }
        return true;
    }

    public void Drop()
    {
        if (!IsCarrying())
        {
            return;
        }
        _carriable.SetCarryTransform(null, null);
        Rigidbody2D carriableRigidBody = _carriable.GetRigidbody2D();
        _rigidbody.mass -= carriableRigidBody.mass;
        Collider2D[] colliders = new Collider2D[_rigidbody.attachedColliderCount];
        _rigidbody.GetAttachedColliders(colliders);
        Collider2D[] carriableColliders = new Collider2D[carriableRigidBody.attachedColliderCount];
        carriableRigidBody.GetAttachedColliders(carriableColliders);
        foreach (Collider2D collider in colliders)
        {
            foreach (Collider2D carriableCollider in carriableColliders)
            {
                Physics2D.IgnoreCollision(collider, carriableCollider, false);
            }
        }
        _carriable = null;

    }

    public void Throw(Vector2 force)
    {
        if (!IsCarrying())
        {
            return;
        }
        _carriable.SetCarryTransform(null, carryTransformHeight);
        Rigidbody2D carriableRigidBody = _carriable.GetRigidbody2D();
        _rigidbody.mass -= carriableRigidBody.mass;
        Collider2D[] colliders = new Collider2D[_rigidbody.attachedColliderCount];
        _rigidbody.GetAttachedColliders(colliders);
        Collider2D[] carriableColliders = new Collider2D[carriableRigidBody.attachedColliderCount];
        carriableRigidBody.GetAttachedColliders(carriableColliders);
        foreach (Collider2D collider in colliders)
        {
            foreach (Collider2D carriableCollider in carriableColliders)
            {
                Physics2D.IgnoreCollision(collider, carriableCollider, false);
            }
        }
        carriableRigidBody.AddForce(force, ForceMode2D.Impulse);
        _carriable = null;

    }
}
