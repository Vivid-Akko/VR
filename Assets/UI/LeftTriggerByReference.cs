using UnityEngine;
using UnityEngine.InputSystem;

public class LeftTriggerByReference : MonoBehaviour
{
    // Inspector 中拖入“XRI LefhtHand Interaction/Activate”动�?
    public InputActionReference LeftTriggerReference;
    public bool isLeftTriggerPressed = false;

 
    void Update()
    {
        var action = LeftTriggerReference?.action;
        if (action == null) return;

        if (action.WasPressedThisFrame())
        {
            Debug.Log("右扳机按�? (InputActionReference)");
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
            Debug.Log($"右扳机按压�?: {value}");
        }
    }
}
