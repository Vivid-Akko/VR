using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float stepInterval = 0.6f;
    public Transform headTransform;

    private AudioSource audioSource;
    private float stepTimer;
    private Vector3 lastPosition;

    private bool isMoving = false;
    private bool hasPlayedFirstStep = false;
    private float moveThreshold = 0.002f;

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
        if (footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[index]);
        }
    }
}
