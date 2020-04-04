using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{
    public void LoadSave()
    {
        FindObjectOfType<SaveManager>().ActualSaveName = this.name;
        SceneManager.LoadScene("SampleScene");
    }
}
