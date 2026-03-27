using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity = -9.8f;

    private CharacterController controller;
    private float yVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 获取相机方向
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // 忽略Y轴（关键！！）
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 计算移动方向
        Vector3 move = forward * z + right * x;

        // 重力
        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveSpeed * move + Vector3.up * yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}