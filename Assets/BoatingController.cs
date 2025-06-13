using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BoatingController : MonoBehaviour
{
    [Header("Boat Settings")]
    public float moveSpeed = 3f;     // 前进速度
    public float turnSpeed = 50f;    // 转向速度
    public Transform playerTransform; // 玩家 Transform（用于定位）

    [Header("Input Action")]
    public InputActionReference rightJoystick; // 右手柄摇杆（Vector2）



    void Start()
    {

    }
    private void Update()
    {
        // 获取摇杆输入（Vector2: x为水平，y为垂直）
        Vector2 input = rightJoystick.action.ReadValue<Vector2>();
        Debug.Log($"摇杆输入: {input}");
        // 前后移动（以x轴为前进方向）
        Vector3 move = input.y * moveSpeed * Time.deltaTime * transform.right;
        transform.position += move;

        // 左右旋转（绕Y轴）
        float turn = input.x * turnSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);

        // 更新玩家位置（如果需要）
        if (playerTransform != null)
        {
            playerTransform.position = transform.position;
        }
    }
}
