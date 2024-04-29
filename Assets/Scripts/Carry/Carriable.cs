using UnityEngine;

namespace OfficeFood.Carry
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Carriable : MonoBehaviour
    {
        public Transform heightTransform = null;// Visuals transform to simulate height.

        [Range(0.0f, 1.0f)]
        public float weightCoefficient = 0.25f;// Effect on Carrier speed.
        public float fallAcceleration = 4.0f;

        internal bool _isCarried = false;
        internal float _height = 0.0f;// Set by Carrier.
        private float _heightPrev = 0.0f;// Prevent the thing.
        private float _fallSpeed = 0.0f;

        internal Rigidbody2D _rigidbody = null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void LateUpdate()
        {
            if (_height != _heightPrev)
            {
                heightTransform.position += new Vector3(0.0f, _height - _heightPrev, 0.0f);
                _heightPrev = _height;
            }
            if (_isCarried || _height == 0.0f)
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

        public bool IsCarried()
        {
            return _isCarried;
        }

        public bool IsFalling()
        {
            return _fallSpeed > 0.0f;
        }
    }
}
