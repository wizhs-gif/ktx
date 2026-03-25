using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = Camera.main.transform.position;

        // 保持物体自身高度
        targetPosition.y = transform.position.y;

        // 让物体朝向目标
        transform.LookAt(targetPosition);
    }
}
