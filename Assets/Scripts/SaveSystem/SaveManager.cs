using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public string ActualSaveName = "";

    public static SaveManager Instance;

    public GameObject Button;
    public GameObject Vievport;

    public List<GameObject> Buttons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<SaveManager>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }

        Instance = this;

        ShowAllWorlds();
    }

    public void ShowAllWorlds()
    {
        Buttons.Clear();

        foreach (string s in Directory.GetDirectories(GetDariectorie()))
        {
            GameObject gb = Instantiate(Button, Vievport.transform);
            gb.GetComponentInChildren<Text>().text = s.Replace($@"{GetDariectorie()}\", "");
            gb.name = s.Replace($@"{GetDariectorie()}\", "");
            Buttons.Add(gb);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetSavePath(string SaveName)
    {
        return $@"{Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)}\My Games\Perlin Noise Test\{SaveName}"; 
    }
    
    public string GetDariectorie()
    {
        return $@"{Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)}\My Games\Perlin Noise Test"; 
    }

    public string GetSaveFilePath(string SaveName)
    {
        string filepath = $@"{GetSavePath(SaveName)}\save.txt";

        if (!File.Exists(filepath))
        {
            Directory.CreateDirectory(filepath.Replace(@"\save.txt", ""));
            File.WriteAllText(filepath, BetterRandom.Between(-15000000, 15000000) + Environment.NewLine);
        }

        return filepath;
    }

    public void CreateWorld(InputField input)
    {
        GetSaveFilePath(input.text);
        foreach(GameObject g in Buttons)
        {
            Destroy(g);
        }
        ShowAllWorlds();
    }

    public void RefreshSaveList()
    {
        ShowAllWorlds();
    }
}