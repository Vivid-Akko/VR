using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class UpBoat : MonoBehaviour
{
    public BoatingController boatController; // 引用 BoatingController 脚本
    public Transform boatTransform; // 引用船只的 Transform
    public DynamicMoveProvider dynamicMoveProvider; // 动态移动提供者

    public GameObject interactManager; // 引用交互管理器
    public bool isBoatUp = false; // 是否上船的标志

    public PaddleAnimation paddleAnimation; // 船桨动画脚本引用
    public PaddleAnimationLeft paddleAnimationLeft; // 左船桨动画脚本引用

    [Header("Input Settings")]
    public float inputCooldown = 0.5f; // 输入冷却时间(秒)
    private float lastInputTime = 0f; // 上次输入时间

    private bool isRightTriggerPressed = false; // 右触发器是否被按下
    private bool isInTrigger = false; // 标记是否在触发器内
    private GameObject playerInTrigger; // 存储在触发器内的玩家对象
    public AudioSource audioSource; // 音频源，用于播放音效
    public AudioSource footstepAudioSource; // 脚步音频源，用于播放脚步音效

    void Update()
    {
        // 获取当前触发器状态
        isRightTriggerPressed = interactManager.GetComponent<RightTriggerByReference>().isRightTriggerPressed;

        // 检查是否在冷却期内
        bool isCooldownOver = Time.time - lastInputTime > inputCooldown;

        // 只有当在触发器内、按下触发器且冷却期结束时才处理
        if (isInTrigger && isRightTriggerPressed && isCooldownOver)
        {
            lastInputTime = Time.time; // 记录本次输入时间

            if (playerInTrigger.CompareTag("Player"))
            {
                ToggleBoatState();
            }
        }
    }

    private void ToggleBoatState()
    {
        if (isBoatUp)
        {
            // 下船逻辑
            if (dynamicMoveProvider != null)
            {
                dynamicMoveProvider.enabled = true;
            }

            if (audioSource != null)
            {
                audioSource.enabled = false;
                footstepAudioSource.enabled = true; // 启用脚步音频源
            }

            boatController.enabled = false;
            isBoatUp = false;
            Debug.Log("已下船，恢复玩家移动控制");
            paddleAnimation.enabled = false; // 禁用右船桨动画
            paddleAnimationLeft.enabled = false; // 禁用左船桨动画
        }
        else
        {
            // 上船逻辑
            if (dynamicMoveProvider != null)
            {
                dynamicMoveProvider.enabled = false;
            }

            // 将玩家传送到船上
            playerInTrigger.transform.SetPositionAndRotation(
                boatTransform.position,
                boatTransform.rotation);

            boatController.enabled = true;

            if (audioSource != null)
            {
                audioSource.enabled = true;
                footstepAudioSource.enabled = false; // 禁用脚步音频源
            }

            isBoatUp = true;
            Debug.Log("已上船，启用船只控制");

            paddleAnimation.enabled = true; // 启用右船桨动画
            paddleAnimationLeft.enabled = true; // 启用左船桨动画
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
            playerInTrigger = other.gameObject;
            Debug.Log("玩家进入上船区域");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
            playerInTrigger = null;
            Debug.Log("玩家离开上船区域");
        }
    }
}