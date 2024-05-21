using System;
using System.Collections.Generic;
using UnityEngine;
using OfficeFood.Food;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// incredibly hacky implementation to get some win/fail states in NOW
namespace OfficeFood.UI
{
    public class LevelMenu : MonoBehaviour
    {
        public FoodDetector foodDetector = null;
        public int foodCount = 3;
        public List<Enemy.Enemy> enemies = new List<Enemy.Enemy>();
        public GameObject winUI = null;
        public GameObject loseUI = null;
        public Text foodCountText = null;
        public String nextScene = "";
        public GameObject restartLevelUI = null;
        public GameObject restartGameUI = null;

        private void OnEnable()
        {
            winUI.SetActive(false);
            loseUI.SetActive(false);
            restartLevelUI.SetActive(false);
            restartGameUI.SetActive(false);
        }

        private bool ended = false;
        private void FixedUpdate()
        {
            if (ended)
            {
                return;
            }
            foodCountText.text = "Food remaining: " + (foodCount - foodDetector.totalItems);
            if (foodDetector.totalItems >= foodCount)
            {
                // win!
                // enable some ui thing
                // pause the game
                Time.timeScale = 0.0f;
                ended = true;
                winUI.SetActive(true);
            }
            else
            {
                foreach (Enemy.Enemy enemy in enemies)
                {
                    // check if enemy is hitting player
                    // if so, fail!!!
                    if (enemy.playerColliding)
                    {
                        Time.timeScale = 0.0f;
                        ended = true;
                        loseUI.SetActive(true);
                        restartLevelUI.SetActive(true);
                        restartGameUI.SetActive(true);

                    }
                }
            }
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
            ended = false;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
            ended = false;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("Scenes/tutorial", LoadSceneMode.Single);
            Time.timeScale = 1.0f;
            ended = false;
        }
    }
}
