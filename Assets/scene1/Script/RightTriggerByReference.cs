using UnityEngine;
using UnityEngine.InputSystem;

public class RightTriggerByReference : MonoBehaviour
{
    // Inspector 中拖入“XRI RightHand Interaction/Activate”动作
    public InputActionReference RightTriggerReference;
    public bool isRightTriggerPressed = false;

 
    void Update()
    {
        var action = RightTriggerReference?.action;
        if (action == null) return;

        if (action.WasPressedThisFrame())
        {
            Debug.Log("右扳机按下 (InputActionReference)");
            isRightTriggerPressed = true;
        }

        if (action.WasReleasedThisFrame())
        {
            Debug.Log("右扳机松开");
            isRightTriggerPressed = false;
        }

        float value = action.ReadValue<float>();
        if (value > 0.1f)
        {
            Debug.Log($"右扳机按压值: {value}");
        }
    }
}
