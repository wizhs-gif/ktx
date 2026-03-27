using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 玩家
    public Vector3 offset = new Vector3(0, 5, -5);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}