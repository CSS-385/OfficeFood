using UnityEngine;
using UnityEngine.UI;

namespace OfficeFood.Human
{
    [RequireComponent(typeof(Slider), typeof(CanvasGroup))]
    public class HumanSprintBar : MonoBehaviour
    {
        public Human human = null;

        private Slider _slider = null;
        private CanvasGroup _canvasGroup = null;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _canvasGroup = GetComponent<CanvasGroup>();
            if (human == null)
            {
                Debug.LogError("Human not configured!");
            }
        }

        private void Update()
        {
            if (human != null)
            {
                _slider.value = human.sprintDurationTime / human.sprintDuration;
            }
            if (_slider.value != _slider.maxValue)
            {
                _canvasGroup.alpha = 1.0f;
            }
            else
            {
                _canvasGroup.alpha = 0.0f;
            }
        }
    }
}
