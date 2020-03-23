using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    float time;
    public float timeToAlive=2;
    private void Start()
    {
        time = timeToAlive;
    }
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
            DestroyImmediate(this.gameObject); 
    }
}
