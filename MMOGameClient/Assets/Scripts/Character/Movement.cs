using UnityEngine;


public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 25.0f;
    private float maxCameraAxis = 66;
    private float minCameraAxis = 10;

    private float cameraAxis;

    GameOptions options;
    public float CameraAxis
    {
        get { return cameraAxis; }
        set { cameraAxis = value; }
    }

    private float mouseSensitivityY = 20.0f;

    public float MouseSensitivityY
    {
        get { return mouseSensitivityY; }
        set { mouseSensitivityY = value; }
    }
    private float mouseSensitivityX = 20.0f;

    public float MouseSensitivityX
    {
        get { return mouseSensitivityX; }
        set { mouseSensitivityX = value; }
    }

    public float jumpSpeed = 10f;
    private Vector3 moveDirection = Vector3.zero;
    new private Rigidbody rigidbody;

    public bool movementEnabled = true;
    private float inputValue = 0;
    void Start()
    {
        options = FindObjectOfType<GameOptions>();
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
    }
    void LateUpdate()
    {
        if (movementEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody.velocity = new Vector3(0, jumpSpeed, 0);
            }
        }
        if (Input.GetMouseButton(1))
        {
            inputValue = InputCheck(Input.GetAxis("Mouse Y") * options.MouseY * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * options.MouseX * Time.deltaTime, 0);
            Camera.main.transform.RotateAround(this.transform.position, this.transform.forward, inputValue);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Horizontal") * options.MouseX * Time.deltaTime, 0);
        }

    }
    float GetRotAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
    private float InputCheck(float Input)
    {
        if (Input < 0)
        {
            if (GetRotAngle(Camera.main.transform.eulerAngles.x - Input) > 60)
            {
                return 0;
            }
        }
        else
        {
            if (GetRotAngle(Camera.main.transform.eulerAngles.x - Input) < -60)
            {
                return 0;
            }
        }
        return Input;
    }
    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            Vector3 movement = transform.right * Input.GetAxis("Vertical") * Time.deltaTime * speed;//, 0.0f, Input.GetAxis("Vertical"));
            if (Input.GetMouseButton(1))
                movement += -transform.forward * Input.GetAxis("Horizontal") * Time.deltaTime * speed;//, 0.0f, Input.GetAxis("Vertical"));
            rigidbody.velocity = movement + new Vector3(0, rigidbody.velocity.y, 0);
        }
    }
}