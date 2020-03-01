using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    float camDist = 20;
    private void Start()
    {
    }
    void LateUpdate()
    {
        camDist = -Input.GetAxis("Mouse ScrollWheel") * 20;

        this.transform.position += -transform.forward * camDist;
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, -transform.up, out hit, 1))
        //{
        //    Debug.DrawRay(transform.position, -transform.up, Color.green);
        //    Debug.Log("Did Hit");
        //    this.transform.position = hit.point;
        //}
        //else
        //{
        //    Debug.DrawRay(transform.position - transform.forward * 2, -transform.forward * 20, Color.white);
        //    Debug.Log("Did not Hit");
        //}
    }

}
