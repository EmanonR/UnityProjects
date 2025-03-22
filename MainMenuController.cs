using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] public List<GameObject> panels;
    public int newGameScene = 1;

    public TMP_Dropdown frameRate;
    public Button continueButton;

    private void Awake()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == 0);
        }

        SaveSystem.Init();

        continueButton.interactable = SaveSystem.Load() != null;
    }


    private void Update()
    {
        //Debugging
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveData testData = new()
            {
                name = "Peter",
                level = 40
            };

            SaveSystem.Save(testData);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        //Create New save data
        SaveData newData = new()
        {
            name = "Loid"
        };
        SaveSystem.Save(newData);

        //Load new game
        GameManager.instance.saveData = newData;
        SceneManager.LoadScene(newGameScene);
    }

    public void ContinueGame()
    {
        //Load save data
        SaveData loadedData = SaveSystem.Load();

        //Load game
        GameManager.instance.saveData = loadedData;
        SceneManager.LoadScene(newGameScene);
    }

    public void SaveSettings()
    {
        SaveSystem.SavePrefInt("TargetFPS", frameRate.value);
    }

    public void UpdateSettingsUI()
    {
        frameRate.value = PlayerPrefs.GetInt("TargetFPS");
    }

    public void ChangeToPanel(int index)    
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
