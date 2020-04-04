using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public struct ConsoleCommand
{
    public string cmd;
    public string Description;
    public string[] arguments;

    public ConsoleCommand(string Cmd, string desc, string[] args)
    {
        cmd = Cmd;
        Description = desc;
        arguments = args;
    }
}

public class ConsoleCommands : MonoBehaviour
{
    public GameObject ConsoleCanvas;
    public List<ConsoleCommand> commands = new List<ConsoleCommand>() 
    {
        new ConsoleCommand("ho_showinteracts", "Show break and build cube position.", new string[] { "enabled" })
    };

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ConsoleCanvas.SetActive(!ConsoleCanvas.activeSelf);
            Cursor.visible = true;
            if(FindObjectOfType<FirstPersonController>() != null)
            {
                Cursor.lockState = ConsoleCanvas.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
                FindObjectOfType<FirstPersonController>().enabled = !ConsoleCanvas.activeSelf;
            }
        }
    }

    public void SendCommand(InputField commandField)
    {
        string cmd = commandField.text;
        List<string> args = new List<string>();

        foreach(string arg in Regex.Split(cmd, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
        {
            args.Add(arg.Replace("\"", ""));
        }

        switch (args[0].ToLower())
        {
            case "ho_showinteracts":
                bool enabled = false;
                if (int.TryParse(args[1], out int boolinteract))
                {
                    enabled = Convert.ToBoolean(boolinteract);
                }
                else
                {
                    enabled = Convert.ToBoolean(args[1]);
                }
                BlockInteract.Instance.InteractShow = enabled;
                BlockInteract.Instance.BreakBlock.SetActive(enabled);
                BlockInteract.Instance.PlaceBlock.SetActive(enabled);
                break;
        }

        /*int startQ = -1;
        int endQ = -1;

        for (int i = 0; i < cmd.Length; i++)
        {
            if(cmd[i].ToString() == "\"")
            {
                if(startQ == -1)
                {
                    startQ = i;
                }
                else if(endQ == -1)
                {
                    endQ = i;
                }

                if(startQ > -1 && endQ > -1)
                {
                    string argBuilder = string.Empty;
                    for(int x = startQ + 1; x < endQ; x++)
                    {
                        argBuilder += cmd[x];
                    }

                    args.Add(argBuilder);

                    startQ = -1;
                    endQ = -1;
                }
            }
        }*/
    }
}