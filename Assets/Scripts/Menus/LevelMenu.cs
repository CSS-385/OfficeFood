using UnityEngine;
using OfficeFood.Food;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using OfficeFood.Prefs;
using UnityEngine.SceneManagement;

namespace OfficeFood.Menus
{
    public class LevelMenu : MenuManager
    {
        [SerializeField]
        private FoodDetector _foodDetector = null;
        [SerializeField]
        private Text _foodDisplay = null;

        [Header("Level Config")]
        [SerializeField]
        private int _foodTotalCount = 3;
        [SerializeField]
        private int[] _goldThresholds = { 1, 2, 3 };

        public bool IsPaused { get; private set; } = false;
        public bool IsComplete { get; private set; } = false;

        [SerializeField]
        private InputRelay _inputRelay = null;

        private void OnEnable()
        {
            _inputRelay.GamePauseEvent += OnMenuPause;
            Enemy.Enemy.OnPlayerCaught += OnPlayerCaught;
        }

        private void OnDisable()
        {
            _inputRelay.GamePauseEvent -= OnMenuPause;
            Enemy.Enemy.OnPlayerCaught -= OnPlayerCaught;
        }

        private void Update()
        {
            // Update display
            if (_foodDisplay != null)
            {
                _foodDisplay.text = (_foodTotalCount - _foodDetector.totalItems).ToString();
            }
        }

        public void SetPause(bool isPaused)
        {
            if (IsComplete)
            {
                return;
            }

            SetPageActive(isPaused, "Pause");
            IsPaused = isPaused;
        }

        // Elevator button invokes this
        public void SetComplete(bool isComplete)
        {
            GameObject finishPage = transform.Find("Finish").gameObject;
            IsComplete = isComplete;

            // Set failed state
            if (_foodDetector.totalItems < _goldThresholds[0])
            {
                finishPage.transform.Find("Content/LevelComplete").GetComponent<Text>().text = "Level Failed!";
                finishPage.transform.Find("Content/Buttons/NextLevel").GetComponent<Button>().interactable = false;
            }

            // Set gold bars on/off
            Transform goldParent = finishPage.transform.Find("Content/Gold");
            int goldCount = 0;
            int foodCount = _foodDetector.totalItems;
            for (int i = 0; i < _goldThresholds.Length; i++)
            {
                Image img = goldParent.GetChild(i).GetComponent<Image>();
                if (foodCount >= _goldThresholds[i])
                {
                    img.color = Color.white;
                    goldCount++;
                }
                else
                {
                    img.color = Color.black;
                }
            }

            // Save progress
            SetPageActive(isComplete, "Finish");
            Persistence.SetLevelProgress(SceneManager.GetActiveScene().buildIndex - 1, goldCount);
        }

        private void OnMenuPause(bool pressed)
        {
            if (pressed)
            {
                SetPause(!IsPaused);
            }
        }

        private void OnPlayerCaught()
        {
            SetComplete(true);
        }
    }
}
