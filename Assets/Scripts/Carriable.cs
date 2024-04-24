using UnityEngine;

// TODO: simulate strength + mass?

[RequireComponent(typeof(Rigidbody2D))]
public class Carriable : MonoBehaviour
{
    public Transform heightTransform = null;// Visuals transform to simulate height.

    private float _height = 0.0f;// Stored when _liftTransform is set to null.
    private float _heightOffset = 0.0f;
    private Transform _carryTransform = null;
    private Transform _carryTransformHeight = null;
    private Rigidbody2D _rigidbody = null;

    public Rigidbody2D GetRigidbody2D()
    {
        return _rigidbody;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_carryTransform != null)
        {
            transform.position = _carryTransform.position;
            float height = _carryTransformHeight.position.y - _carryTransform.position.y;
            heightTransform.position += new Vector3(0.0f, height - _heightOffset, 0.0f);
            _heightOffset = height;
        }
        else
        {
            heightTransform.position += new Vector3(0.0f, _height - _heightOffset, 0.0f);
            _heightOffset = _height;
            _height = Mathf.Max(_height - (8.0f * Time.deltaTime), 0.0f);
        }
    }

    public bool IsCarriable()
    {
        return _carryTransform == null;
    }

    public bool SetCarryTransform(Transform carryTransform, Transform carryTransformHeight)
    {
        if (_carryTransform != null)
        {
            if (carryTransform != null)
            {
                return false;
            }
            _height = _carryTransformHeight.position.y - _carryTransform.position.y;
            _carryTransform = null;
            _carryTransformHeight = null;
            return true;
        }
        else
        {
            if (carryTransform == null)
            {
                return false;
            }
            _height = 0.0f;
            _carryTransform = carryTransform;
            _carryTransformHeight = carryTransformHeight;
            return true;
        }
    }
}
