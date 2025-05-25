using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CanvasTriggerControl : MonoBehaviour
{
    [Header("Trigger Settings")]
    public InputActionReference leftTriggerAction; 

    [Header("UI Settings")]
    public Canvas targetCanvas1;
    public Canvas targetCanvas2;
    public float activationThreshold = 0.5f; 

    [Header("Cooldown Settings")]
    public float cooldownTime = 0.3f; 
    private float lastTriggerTime;
    private bool isCanvasVisible = false; 

    void Awake()
    {
        targetCanvas1.enabled = false;
        targetCanvas2.enabled = false;
        
        leftTriggerAction.action.Enable();
    }

    void OnEnable()
    {
        leftTriggerAction.action.performed += OnTriggerPressed;
    }

    void OnDisable()
    {
        leftTriggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (Time.time - lastTriggerTime < cooldownTime) return;

        float triggerValue = context.ReadValue<float>();
        
        if (triggerValue >= activationThreshold)
        {
            ToggleCanvas();
            lastTriggerTime = Time.time;
        }
    }

    void ToggleCanvas()
    {
        isCanvasVisible = !isCanvasVisible;
        targetCanvas1.enabled = isCanvasVisible;
        targetCanvas2.enabled = isCanvasVisible;

    }
}