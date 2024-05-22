using UnityEngine;

namespace OfficeFood.ElevatorDoor
{
    [RequireComponent(typeof(Animator))]
    public class ElevatorDoor : MonoBehaviour
    {
        // Animation Parameters
        private readonly int _animParamOpen = Animator.StringToHash("Open");

        private Animator _animator = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            // Temporary until elevator button is functional.
            Open();
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
