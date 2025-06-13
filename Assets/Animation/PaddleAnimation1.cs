using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleAnimationLeft : MonoBehaviour
{
    public Transform paddleJoint;
    public Transform paddlePosition;
    public float animationDuration = 1f;

    private float timer = 0f;

    void Start()
    {
        paddleJoint.position = paddlePosition.position;

    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = (timer % animationDuration) / animationDuration;

        // 自定义旋转轨迹（使用插值或曲线）
        float x = Mathf.Sin(t * 2 * Mathf.PI) * (20f);
        float z = Mathf.Sin(t * 2 * Mathf.PI + Mathf.PI / 2) * (-30f);

        paddleJoint.localEulerAngles = new Vector3(x, 0f, z);
    }
}

