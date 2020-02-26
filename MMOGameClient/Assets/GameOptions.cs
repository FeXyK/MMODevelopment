using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Screen.SetResolution(1920, 1080, true);
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
