using UnityEngine;

public class InteractiveSphere : MonoBehaviour
{
    [Header("距离控制")]
    public float minDistance = 0.5f;    // 最小触发距离
    public float maxDistance = 3f;     // 最大影响距离
    public float checkInterval = 0.1f; // 检测频率

    [Header("缩放设置")]
    public float minScale = 0.1f;      // 最小缩放
    public float maxScale = 1f;        // 最大缩放
    public AnimationCurve scaleCurve;  // 缩放曲线控制

    [Header("可视化调试")]
    public bool showGizmos = true;
    public Color rangeColor = Color.cyan;

    private Transform xrCamera;
    private Vector3 originalScale;
    private Coroutine checkCoroutine;

    void Awake()
    {
        xrCamera = Camera.main.transform;
        originalScale = transform.localScale;
        
        // 初始化缩放曲线（如果未设置）
        if(scaleCurve == null || scaleCurve.length < 2)
        {
            scaleCurve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 1)
            );
        }

        StartCoroutine(DistanceCheck());
    }

    System.Collections.IEnumerator DistanceCheck()
    {
        while(true)
        {
            float distance = Vector3.Distance(transform.position, xrCamera.position);
            UpdateSphereScale(distance);
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void UpdateSphereScale(float currentDistance)
    {
        // 计算标准化距离（0-1范围）
        float normalizedDistance = Mathf.Clamp01(
            (currentDistance - minDistance) / (maxDistance - minDistance)
        );

        // 使用曲线控制缩放比例
        float scaleFactor = scaleCurve.Evaluate(1 - normalizedDistance);
        
        // 应用缩放限制
        float targetScale = Mathf.Lerp(minScale, maxScale, scaleFactor);
        transform.localScale = originalScale * targetScale;
    }

    // 场景编辑器可视化
    void OnDrawGizmosSelected()
    {
        if(!showGizmos) return;

        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}