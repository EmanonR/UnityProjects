using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string Save_Folder = Application.dataPath + "/Saves/";
    public static string save_separator = "#";

    public static void Init()
    {
        //Create directory if none exists
        if (!Directory.Exists(Save_Folder))
        {
            Directory.CreateDirectory(Save_Folder);
        }
    }

    public static void Save(SaveData newData)
    {
        string json = JsonUtility.ToJson(newData);
        string saveString = string.Join(save_separator, json);

        File.WriteAllText(Save_Folder + "/Save.txt", saveString);
    }

    public static SaveData Load()
    {
        if (File.Exists(Save_Folder + "/Save.txt"))
        {
            string saveString = File.ReadAllText(Save_Folder + "/Save.txt");

            SaveData loadData = JsonUtility.FromJson<SaveData>(saveString);

            return loadData;
        }
        else
        {
            return null;
        }
    }

    public static void SavePrefFloat(string name, float value)
    {
        PlayerPrefs.SetFloat(name, value);
        PlayerPrefs.Save();
    }
    public static void SavePrefInt(string name, int value)
    {
        PlayerPrefs.SetInt(name, value);
        PlayerPrefs.Save();
    }
    public static void SavePrefString(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class SaveData
{
    public string name = "Light";
    public int level = 1;
    public int MapIndex = 1;
    public Vector3 mapPosition;
    public Vector3 orientation;
}
