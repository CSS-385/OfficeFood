using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Fridge
{
    [RequireComponent(typeof(Animator))]
    public class Fridge : MonoBehaviour
    {
        public UnityEvent Opening = new UnityEvent();
        public UnityEvent Opened = new UnityEvent();
        public UnityEvent Closing = new UnityEvent();
        public UnityEvent Closed = new UnityEvent();

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

        private void InvokeOpening()
        {
            Opening.Invoke();
        }
        private void InvokeOpened()
        {
            Opened.Invoke();
        }

        private void InvokeClosing()
        {
            Closing.Invoke();
        }

        private void InvokeClosed()
        {
            Closed.Invoke();
        }
    }
}
