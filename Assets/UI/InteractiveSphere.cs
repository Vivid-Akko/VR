using UnityEngine;

public class InteractiveSphere : MonoBehaviour
{
    [Header("�������")]
    public float minDistance = 0.5f;    // ��С��������
    public float maxDistance = 3f;     // ���Ӱ�����
    public float checkInterval = 0.1f; // ���Ƶ��

    [Header("��������")]
    public float minScale = 0.1f;      // ��С����
    public float maxScale = 1f;        // �������
    public AnimationCurve scaleCurve;  // �������߿���

    [Header("���ӻ�����")]
    public bool showGizmos = true;
    public Color rangeColor = Color.cyan;

    private Transform xrCamera;
    private Vector3 originalScale;
    private Coroutine checkCoroutine;

    void Awake()
    {
        xrCamera = Camera.main.transform;
        originalScale = transform.localScale;
        
        // ��ʼ���������ߣ����δ���ã�
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
        // �����׼�����루0-1��Χ��
        float normalizedDistance = Mathf.Clamp01(
            (currentDistance - minDistance) / (maxDistance - minDistance)
        );

        // ʹ�����߿������ű���
        float scaleFactor = scaleCurve.Evaluate(1 - normalizedDistance);
        
        // Ӧ����������
        float targetScale = Mathf.Lerp(minScale, maxScale, scaleFactor);
        transform.localScale = originalScale * targetScale;
    }

    // �����༭�����ӻ�
    void OnDrawGizmosSelected()
    {
        if(!showGizmos) return;

        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}