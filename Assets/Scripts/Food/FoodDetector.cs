using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace OfficeFood.Food
{
    public class FoodDetector : MonoBehaviour
    {
        public Text display = null;

        public int totalItems { get; private set; }

        private List<Food> _foods = new List<Food>();

        private void Update()
        {
            if (display != null)
            {
                display.text = "Foods detected:\n";
                foreach (Food food in _foods)
                {
                    display.text += food.name + "\n";
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Food food = collision.transform?.GetComponent<Food>();
            if (food != null)
            {
                _foods.Add(food);
                totalItems++;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Food food = collision.transform?.GetComponent<Food>();
            if (food != null)
            {
                _foods.Remove(food);
                totalItems--;
            }
        }
    }
}
