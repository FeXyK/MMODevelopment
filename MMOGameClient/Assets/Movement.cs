using UnityEngine;
using System.Collections;


public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 25.0f;
    public float rotSpeed = 125.0f;
    public float jumpSpeed = 10f;
    private Vector3 moveDirection = Vector3.zero;
    new private Rigidbody rigidbody;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.velocity = new Vector3(0, jumpSpeed, 0);
        }
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    private void FixedUpdate()
    {
        Vector3 movement = transform.right * Input.GetAxis("Vertical") * Time.deltaTime * speed;//, 0.0f, Input.GetAxis("Vertical"));
        rigidbody.velocity = movement + new Vector3(0, rigidbody.velocity.y, 0);
        transform.eulerAngles += new Vector3(0, Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed, 0);
    }
}