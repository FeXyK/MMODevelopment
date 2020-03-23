using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public SliderWithValue SliderMouseX;
    public SliderWithValue SliderMouseY;
    public SliderWithValue SliderVolume;
    public GameObject GameOpt;

    [SerializeField]
    private float mouseX = 1;
    [SerializeField]
    public float MouseX
    {
        get { return mouseX; }
        set
        {
            mouseX = value;
            OnChange();
        }
    }
    [SerializeField]
    private float mouseY = 1;

    [SerializeField]
    public float MouseY
    {
        get { return mouseY; }
        set
        {
            mouseY = value;
            OnChange();
        }
    }
    [SerializeField]
    private float audioVolume = 0;

    [SerializeField]
    public float AudioVolume
    {
        get { return audioVolume; }
        set
        {
            audioVolume = value;
            if (aSource == null)
                aSource = FindObjectOfType<AudioSource>();
            aSource.volume = audioVolume;
            OnChange();
        }
    }

    AudioSource aSource;

    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\Settings.cfg";
    private void Start()
    {
        aSource = FindObjectOfType<AudioSource>();
        //Application.targetFrameRate = 60;
        if (File.Exists(path))
        {
            string[] settings = File.ReadAllLines(path);
            string[] settingData;
            foreach (var setting in settings)
            {
                settingData = setting.Split('=');
                switch (settingData[0].ToLower())
                {
                    case "mousesensitivityx":
                        mouseX = float.Parse(settingData[1]);
                        SliderMouseX.slider.value = mouseX;
                        break;
                    case "mousesensitivityy":
                        mouseY = float.Parse(settingData[1]);
                        SliderMouseY.slider.value = mouseY;
                        break;
                    case "volume":
                        AudioVolume = float.Parse(settingData[1]);
                        SliderVolume.slider.value = audioVolume;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            //File.Create(path);
            List<string> content = new List<string>();
            content.Add("MouseSensitivityX=50");
            content.Add("MouseSensitivityY=50");
            content.Add("Volume=50");
            for (int i = 0; i < 10; i++)
            {
                content.Add("SkillBar" + i + "=-1");
            }
            File.WriteAllLines(path, content);
        }
        GameOpt.SetActive(false);
    }
    void OnChange()
    {
        string[] settings = File.ReadAllLines(path);
        string[] settingData;
        int numOfLine = 0;
        foreach (var setting in settings)
        {
            numOfLine++;
            settingData = setting.Split('=');
            switch (settingData[0].ToLower())
            {
                case "mousesensitivityx":
                    ChangeLine("MouseSensitivityX=" + MouseX.ToString(), settings, numOfLine);
                    break;
                case "mousesensitivityy":
                    ChangeLine("MouseSensitivityY=" + MouseY.ToString(), settings, numOfLine);
                    break;
                case "volume":
                    ChangeLine("Volume=" + AudioVolume.ToString(), settings, numOfLine);
                    break;
                default:
                    break;
            }
        }
    }
    void ChangeLine(string newSetting, string[] arrLine, int editingLine)
    {
        arrLine[editingLine - 1] = newSetting;
        File.WriteAllLines(path, arrLine);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Screen.SetResolution(640, 640, false);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Screen.SetResolution(400, 400, false);
        }
    }
}
