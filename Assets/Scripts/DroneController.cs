using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    public float tiltAngle = 45f;

    public float cameraRotateSpeed = 50f;

    private Rigidbody rb;
    public Camera droneCamera;
    private bool isCameraMode = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the drone!");
            enabled = false;
        }
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (droneCamera == null)
        {
            Debug.LogError("Drone camera is not assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isCameraMode = !isCameraMode;

            Debug.Log("Camera Mode: " + isCameraMode);
        }
    }

    void FixedUpdate()
    {
        if (isCameraMode)
        {
            HandleCameraMovement();
        }
        else
        {
            HandleDroneMovement();
        }
    }

    void HandleCameraMovement()
    {
        float cameraVerticalRotation = Input.GetAxis("Vertical");
        float cameraHorizontalRotation = Input.GetAxis("Horizontal");

        droneCamera.transform.RotateAround(transform.position, transform.up, cameraHorizontalRotation * cameraRotateSpeed * Time.deltaTime);
        droneCamera.transform.RotateAround(transform.position, droneCamera.transform.right, cameraVerticalRotation * cameraRotateSpeed * Time.deltaTime);

        float angleX = droneCamera.transform.localEulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;
        angleX = Mathf.Clamp(angleX, -80, 80);
        droneCamera.transform.localEulerAngles = new Vector3(angleX, droneCamera.transform.localEulerAngles.y, droneCamera.transform.localEulerAngles.z);

    }

    void HandleDroneMovement()
    {
        float moveForwardBackward = Input.GetAxis("Vertical");
        float rotateYaw = Input.GetAxis("Horizontal");
        float moveUpDown = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            moveUpDown = 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            moveUpDown = -1;
        }

        float moveLeftRight = 0;
        if (Input.GetKey(KeyCode.Z))
        {
            moveLeftRight = -1;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            moveLeftRight = 1;
        }

        MoveDrone(moveForwardBackward, rotateYaw, moveUpDown, moveLeftRight);
    }

    void MoveDrone(float forwardBackward, float yaw, float upDown, float leftRight)
    {
        transform.Rotate(0, yaw * rotateSpeed * Time.deltaTime, 0);

        Vector3 forwardMovement = transform.forward * forwardBackward * moveSpeed * Time.deltaTime;

        Vector3 verticalMovement = Vector3.up * upDown * moveSpeed * Time.deltaTime;

        Vector3 sidewaysMovement = transform.right * leftRight * moveSpeed * Time.deltaTime;


        float targetTiltForwardBackward = forwardBackward * tiltAngle;
        float targetTiltLeftRight = -leftRight * tiltAngle;

        Quaternion targetRotation = Quaternion.Euler(targetTiltForwardBackward, transform.rotation.eulerAngles.y, targetTiltLeftRight);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

        rb.MovePosition(rb.position + forwardMovement + verticalMovement + sidewaysMovement);

    }

}