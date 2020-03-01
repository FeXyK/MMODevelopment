using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOverHead : MonoBehaviour
{
    //public Text Name;
    public Transform target;
    void Update()
    {
        //var wantedPos = (target.position + Vector3.up * 0.1f);
        //transform.position = wantedPos;
        transform.LookAt(Camera.main.transform.position);

        //var wantedPos = Camera.main.WorldToViewportPoint(target.position);
        //transform.position = wantedPos;
        transform.eulerAngles = Camera.main.transform.eulerAngles;
        //Name.transform.position = this.gameObject.transform.position + Vector3.up * 2;
    }
}
