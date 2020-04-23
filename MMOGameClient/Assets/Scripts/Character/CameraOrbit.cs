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
       
    }

}
