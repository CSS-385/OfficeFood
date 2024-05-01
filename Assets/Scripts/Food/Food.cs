using UnityEngine;
using OfficeFood.Carry;

namespace OfficeFood.Food
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Carriable))]
    public class Food : MonoBehaviour
    {
        public int points = 8;// todo: how will scoring be done? points per food item?

        private Carriable _carriable = null;
        private Rigidbody2D _rigidbody = null;

        private void Awake()
        {
            _carriable = GetComponent<Carriable>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _carriable.CarriedStarted.AddListener(OnCarriableCarriedStarted);
        }

        private void OnDisable()
        {
            _carriable.CarriedStarted.RemoveListener(OnCarriableCarriedStarted);
        }

        private void OnCarriableCarriedStarted()
        {
            //Debug.Log(name + " was picked up!");
        }
    }
}
