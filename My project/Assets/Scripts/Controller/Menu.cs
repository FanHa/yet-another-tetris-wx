using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class Menu : MonoBehaviour
    {
        [Header("Levels To Load")]
        public string newGameLevel;
        private string levelToLoad;
        [SerializeField] private GameObject noSavedGameDialog;

        public void NewGameDialogYes()
        {
            SceneManager.LoadScene(newGameLevel);
        }

        public void LoadGameDialogYes()
        {
            if (PlayerPrefs.HasKey("SavedLevel"))
            {
                levelToLoad = PlayerPrefs.GetString("SavedLevel");
                SceneManager.LoadScene(levelToLoad);
            }
            else 
            {
                noSavedGameDialog.SetActive(true);
            }
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}

