using UnityEngine;
using System.Collections;

public class BoatingSound : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float stepInterval = 0.6f;
    public Transform headTransform;

    [Header("Audio Settings")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minVolume = 0.8f;
    public float maxVolume = 1.0f;

    private AudioSource audioSource;
    private float stepTimer;
    private Vector3 lastPosition;
    private bool isMoving = false;
    private bool hasPlayedFirstStep = false;
    private float moveThreshold = 0.002f;
    private bool isPlayingSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (headTransform == null)
        {
            headTransform = transform;
        }

        lastPosition = headTransform.position;
        stepTimer = stepInterval;
    }

    void Update()
    {
        Vector3 currentPosition = headTransform.position;
        float distance = Vector3.Distance(currentPosition, lastPosition);
        isMoving = distance > moveThreshold;

        // 如果正在播放声音，不触发新的播放
        if (isPlayingSound) return;

        if (isMoving && !hasPlayedFirstStep)
        {
            PlayFootstep();
            stepTimer = stepInterval;
            hasPlayedFirstStep = true;
        }

        if (isMoving && hasPlayedFirstStep)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }

        if (!isMoving && hasPlayedFirstStep)
        {
            hasPlayedFirstStep = false;
            stepTimer = stepInterval;
        }

        lastPosition = currentPosition;
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0 && !isPlayingSound)
        {
            int index = Random.Range(0, footstepClips.Length);

            // 设置随机音高和音量增加变化
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = Random.Range(minVolume, maxVolume);

            isPlayingSound = true;
            audioSource.PlayOneShot(footstepClips[index]);

            // 启动协程来跟踪声音播放状态
            StartCoroutine(WaitForSoundToFinish(footstepClips[index].length));
        }
    }

    IEnumerator WaitForSoundToFinish(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        isPlayingSound = false;
    }
}