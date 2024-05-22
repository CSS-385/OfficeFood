using UnityEngine;
using UnityEngine.SceneManagement;

namespace OfficeFood.Menus
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject ActivePage { get; private set; }

        public GameObject SetPageActive(bool isActive, string page, bool changeTimeScale = true) 
        {
            if (changeTimeScale)
            {
                Time.timeScale = isActive ? 0 : 1;
            }

            if (isActive)
            {
                // Turn off active page if turning on a different page
                if (ActivePage != null)
                {
                    ActivePage.SetActive(false);
                }

                GameObject pageGO = transform.Find(page).gameObject;
                pageGO.SetActive(true);
                ActivePage = pageGO;

                return pageGO;
            }
            else
            {
                if (ActivePage == null)
                {
                    return null;
                }

                ActivePage.SetActive(false);
                ActivePage = null;

                return null;
            }
        }


        public void LoadNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
        }

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            Time.timeScale = 1.0f;
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
            Time.timeScale = 1.0f;
        }
    }
}