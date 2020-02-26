using UnityEngine;
using System.Collections;


public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 25.0f;
    public float rotSpeed = 125.0f;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        transform.position += transform.right * Input.GetAxis("Vertical")*Time.deltaTime*speed;//, 0.0f, Input.GetAxis("Vertical"));
        transform.eulerAngles += new Vector3(0, Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed, 0);

    }
}