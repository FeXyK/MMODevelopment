using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour
{
    [SerializeField]
    private float mouseX = 1;
    [SerializeField]
    public float MouseX
    {
        get { return mouseX; }
        set { mouseX = value;  }
    }
    [SerializeField]
    private float mouseY = 1;

    [SerializeField]
    public float MouseY
    {
        get { return mouseY; }
        set { mouseY = value;  }
    }
    [SerializeField]
    private float audioVolume = 0;

    [SerializeField]
    public float AudioVolume
    {
        get { return audioVolume; }
        set { audioVolume = value; aSource.volume = audioVolume; }
    }

    AudioSource aSource;
    private void Start()
    {
        aSource = FindObjectOfType<AudioSource>();
        AudioVolume = 0.1f;
        MouseX = 20;
        MouseY = 10;
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
