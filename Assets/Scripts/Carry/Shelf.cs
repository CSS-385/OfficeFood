using UnityEngine;

namespace OfficeFood.Carry
{
    public class Shelf : MonoBehaviour
    {
        public Transform carryTransform = null;// Used by Carriable to reparent.
        public Transform carryTransformHeight = null;// Used by Carriable to simulate height for visuals.

        internal Carriable _carriable = null;

        public bool HasCarriable()
        {
            return _carriable != null;
        }

        public bool AddCarriable(Carriable carriable)
        {
            if (!HasCarriable())
            {
                _carriable = carriable;
                _carriable.transform.parent = carryTransform;
                _carriable.transform.position = carryTransform.position;
                _carriable.SetCarried(true);
                return true;
            }
            return false;
        }

        public Carriable RemoveCarriable()
        {
            Carriable carriable = _carriable;
            _carriable.SetCarried(false);
            _carriable = null;
            return carriable;
        }

        private void Update()
        {
            if (!HasCarriable())
            {
                return;
            }
            _carriable._height = carryTransformHeight.position.y - carryTransform.position.y;
        }
    }
}
