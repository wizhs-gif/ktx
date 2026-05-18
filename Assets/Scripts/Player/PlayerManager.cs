using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum ViewMode { FirstPerson, ThirdPerson }

    [Header("视角模式")]
    public ViewMode viewMode = ViewMode.FirstPerson;

    [Header("移动")]
    public float moveSpeed = 3f;
    public float gravity = -9.8f;

    [Header("鼠标")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;

    [Header("第一人称")]
    public float headBobSpeed = 10f;
    public float headBobAmount = 0.05f;

    [Header("第三人称")]
    public float thirdPersonDistance = 4f;
    public float thirdPersonHeight = 2f;
    public float thirdPersonLookOffset = 1.5f;
    public float orbitSmoothSpeed = 10f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    private float defaultCamY;
    private float bobTimer;
    private float orbitYAngle = 0f;

    private bool isFirstPerson => viewMode == ViewMode.FirstPerson;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (isFirstPerson)
        {
            defaultCamY = cameraTransform.localPosition.y;
        }
    }

    void Update()
    {
        Look();
        Move();
        if (isFirstPerson)
        {
            HeadBob();
        }
    }

    void LateUpdate()
    {
        if (!isFirstPerson)
        {
            ThirdPersonCamera();
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (isFirstPerson)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // 第三人称：水平旋转相机围绕玩家，垂直调整俯仰
            orbitYAngle += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -20f, 60f);
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move;
        if (isFirstPerson)
        {
            move = transform.right * x + transform.forward * z;
        }
        else
        {
            // 第三人称：相对相机方向移动
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            move = camRight * x + camForward * z;

            // 朝移动方向转身
            if (move.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }
        }

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

    void ThirdPersonCamera()
    {
        // 计算相机目标位置：在玩家身后偏上
        Quaternion rotation = Quaternion.Euler(xRotation, orbitYAngle, 0f);
        Vector3 offset = rotation * new Vector3(0f, thirdPersonHeight, -thirdPersonDistance);
        Vector3 targetPos = transform.position + offset;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, orbitSmoothSpeed * Time.deltaTime);

        // 相机看向玩家偏上位置
        Vector3 lookTarget = transform.position + Vector3.up * thirdPersonLookOffset;
        cameraTransform.LookAt(lookTarget);
    }
}