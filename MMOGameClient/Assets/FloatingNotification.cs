using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingNotification : MonoBehaviour
{
    public float Timer = 3;
    public float Grow = 1.005f;
    public float Speed = 0.1f;
    public float FadeSpeed = 0.9999999999f;
    public TMP_Text Message;
    float temp;
    private void Start()
    {
        temp = Timer;
    }
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            Destroy(this.gameObject);
        }
        this.transform.position = new Vector3(this.transform.position.x, transform.position.y + Speed, 0);
        this.transform.localScale *= Grow;
        float t = Timer / temp;
        Message.color = new Color(0, 0, 0, t);
    }
}
