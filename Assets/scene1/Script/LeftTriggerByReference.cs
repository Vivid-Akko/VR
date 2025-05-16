using UnityEngine;
using UnityEngine.InputSystem;

public class LeftTriggerByReference : MonoBehaviour
{
    // Inspector 中拖入“XRI LeftHand Interaction/Activate”动作
    public InputActionReference leftTriggerReference;
    public bool isLeftTriggerPressed = false;

 
    void Update()
    {
        var action = leftTriggerReference?.action;
        if (action == null) return;

        if (action.WasPressedThisFrame())
        {
            Debug.Log("右扳机按下 (InputActionReference)");
            isLeftTriggerPressed = true;
        }

        if (action.WasReleasedThisFrame())
        {
            Debug.Log("右扳机松开");
            isLeftTriggerPressed = false;
        }

        float value = action.ReadValue<float>();
        if (value > 0.1f)
        {
            Debug.Log($"右扳机按压值: {value}");
        }
    }
}
