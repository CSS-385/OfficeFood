using UnityEngine;

namespace OfficeFood.Fridge
{
    [RequireComponent(typeof(Animator))]
    public class Fridge : MonoBehaviour
    {
        // Animation Parameters
        private readonly int _animParamOpen = Animator.StringToHash("Open");

        private Animator _animator = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Toggle()
        {
            bool open = _animator.GetBool(_animParamOpen);
            _animator.SetBool(_animParamOpen, !open);
        }

        public void Open()
        {
            _animator.SetBool(_animParamOpen, true);
        }

        public void Close()
        {
            _animator.SetBool(_animParamOpen, false);
        }
    }
}
