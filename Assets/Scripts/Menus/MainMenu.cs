using OfficeFood.Prefs;
using UnityEngine;
using UnityEngine.UI;

namespace OfficeFood.Menus
{
    public class MainMenu : MenuManager 
    {
        public void SetLevelSelect(bool isActive)
        {
            if (isActive) 
            {
                Transform levelParent = transform.Find("LevelSelect/LevelButtons");

                for (int i = 0; i < levelParent.childCount; i++)
                {
                    levelParent.GetChild(i).GetComponent<Button>().interactable = Persistence.IsLevelAvailable(i + 1);
                }
            }
            SetPageActive(isActive, "LevelSelect", false);
        }

        public void SetMainMenu(bool isActive)
        {
            SetPageActive(isActive, "MainMenu", false);
        }
    }
}