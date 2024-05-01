using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OfficeFood.Food
{
    [RequireComponent(typeof(Text))]
    public class FoodDetectorDisplay : MonoBehaviour
    {
        private Text _text = null;

        private void Awake()
        {
            _text = GetComponent<Text>();
            _text.text = "";
        }

        private LinkedList<Food> _foods = new LinkedList<Food>();

        private void Update()
        {
            _text.text = "Foods detected:\n";
            foreach (Food food in _foods)
            {
                _text.text += food.name + "\n";
            }
        }

        public void OnFoodDetectorFoodEntered(Food food)
        {
            _foods.AddLast(food);
        }

        public void OnFoodDetectorFoodExited(Food food)
        {
            _foods.Remove(food);
        }
    }
}
