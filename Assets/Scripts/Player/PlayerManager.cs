using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 3f;
    public float gravity = -9.8f;

    [Header("鼠标")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    
    public float headBobSpeed = 10f;
    public float headBobAmount = 0.05f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    private float defaultCamY;
    private float bobTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCamY = cameraTransform.localPosition.y;
    }

    void Update()
    {
        Look();
        Move();
        HeadBob();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveSpeed * move + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void HeadBob()
    {
        if (controller.velocity.magnitude > 0.1f && controller.isGrounded)
        {
            bobTimer += Time.deltaTime * headBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * headBobAmount;

            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = defaultCamY + bobOffset;
            cameraTransform.localPosition = camPos;
        }
        else
        {
            bobTimer = 0;
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, defaultCamY, Time.deltaTime * 5f);
            cameraTransform.localPosition = camPos;
        }
    }
}