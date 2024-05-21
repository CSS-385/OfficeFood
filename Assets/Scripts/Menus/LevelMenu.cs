using UnityEngine;
using OfficeFood.Food;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OfficeFood.Menus
{
    public class LevelMenu : MenuManager
    {
        public FoodDetector foodDetector = null;
        public int[] goldThresh = new int[3];
        public int foodCount = 3;

        public bool IsPaused { get; private set; } = false;
        public bool IsComplete { get; private set; } = false;

        private Text foodCountText;

        [SerializeField]
        private InputRelay _inputRelay = null;

        private void Start()
        {
            foodCountText = transform.Find("FoodCount").GetComponent<Text>();
            _inputRelay.GamePauseEvent += OnMenuPause;
            Enemy.Enemy.OnPlayerCaught += OnPlayerCaught;
        }

        private void FixedUpdate()
        {
            foodCountText.text = "Food remaining: " + (foodCount - foodDetector.totalItems);
            if (foodDetector.totalItems >= foodCount)
            {
                // win!
                // enable some ui thing
                // pause the game
                SetComplete(true);
            }
        }

        private void OnDestroy()
        {
            _inputRelay.GamePauseEvent -= OnMenuPause;
            Enemy.Enemy.OnPlayerCaught -= OnPlayerCaught;
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

        public void SetComplete(bool isComplete)
        {
            GameObject finishPage = SetPageActive(isComplete, "Finish");
            IsComplete = isComplete;

            if (foodDetector.totalItems < goldThresh[0])
            {
                finishPage.transform.Find("Content/LevelComplete").GetComponent<Text>().text = "Level Failed!";
                finishPage.transform.Find("Content/Buttons/NextLevel").GetComponent<Button>().interactable = false;
            }

            Transform goldParent = finishPage.transform.Find("Content/Gold");
            for (int i = 0; i < goldThresh.Length; i++)
            {
                Image img = goldParent.GetChild(i).GetComponent<Image>();
                img.color = goldThresh[i] > foodDetector.totalItems ? Color.gray : Color.white;
            }
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
