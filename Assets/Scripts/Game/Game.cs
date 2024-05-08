using System;
using System.Collections.Generic;
using UnityEngine;
using OfficeFood.Food;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// incredibly hacky implementation to get some win/fail states in NOW
namespace OfficeFood.Game
{
    public class Game : MonoBehaviour
    {
        public FoodDetector foodDetector = null;
        public List<Enemy.Enemy> enemies = new List<Enemy.Enemy>();
        public Text wintext = null;
        public Text losetext = null;

        private void OnEnable()
        {
            wintext.enabled = false;
            losetext.enabled = false;
        }

        private bool ended = false;
        private void FixedUpdate()
        {
            if (ended)
            {
                return;
            }
            if (foodDetector.totalItems == 4)
            {
                // win!
                // enable some ui thing
                // pause the game
                Time.timeScale = 0.0f;
                ended = true;
                wintext.enabled = true;
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
                        losetext.enabled = true;
                    }
                }
            }
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
            ended = false;
        }
    }
}
