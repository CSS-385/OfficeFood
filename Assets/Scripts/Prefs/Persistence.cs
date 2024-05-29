using UnityEngine;
using UnityEngine.SceneManagement;

namespace OfficeFood.Prefs
{
    public static class Persistence
    {
        static Persistence()
        {
            int levels = SceneManager.sceneCountInBuildSettings - 2;

            PlayerPrefs.SetInt("Level0", 0);
            for (int i = 2; i < levels; i++)
            {
                if (!PlayerPrefs.HasKey("Level" + i))
                {
                    PlayerPrefs.SetInt("Level" + i, 0);
                }
            }

        }

        public static void SetLevelProgress(int level, int progress)
        {
            PlayerPrefs.SetInt("Level" + level, progress);
        }

        public static bool IsLevelAvailable(int level)
        {
            if (level == 1)
            {
                return true;
            }
            else
            {
                return PlayerPrefs.GetInt("Level" + level) != 0;
            }
        }
    }
}
