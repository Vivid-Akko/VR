using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

/// <summary>
/// Detects gaze hits on objects tagged "target" and invokes their attached action scripts when the right-hand trigger is pressed.
/// Manages locking/unlocking only the player's movement (rotation remains enabled) until a menu selection (Play or Quit) is made.
/// </summary>
public class GazeMenuController : MonoBehaviour
{
    public GameObject interactorController;
    [Header("Movement Controller")]
    [Tooltip("Drag here the component or GameObject responsible for player translation movement. Can be Move Provider, CharacterController, or a custom movement script.")]
    public MonoBehaviour movementController;

    [Header("Menu UI Canvas")]
    [Tooltip("The parent GameObject of your menu (will be hidden after selection)")]
    public GameObject menuCanvas;

    private InputDevice rightHandDevice;
    public bool isRightTriggerPressed = false;
    public bool isGazeHit = false;

    void Start()
    {
        // Disable movement until selection
        if (movementController != null)
            movementController.enabled = false;
        else
            Debug.LogWarning("[GazeMenuController] movementController not assigned, movement will not be locked.");

        // Cache right-hand controller
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Right |
            InputDeviceCharacteristics.Controller,
            devices);
        if (devices.Count > 0)
            rightHandDevice = devices[0];
        else
            Debug.LogWarning("[GazeMenuController] No right-hand controller found.");

    
        // Validate menuCanvas
        if (menuCanvas == null)
            Debug.LogError("[GazeMenuController] Assign menuCanvas in Inspector.");
    }

    void Update()
    {
        isRightTriggerPressed = interactorController.GetComponent<RightTriggerByReference>().isRightTriggerPressed;
        isGazeHit = interactorController.GetComponent<RayInteractor>().isRaycastHit;

        // Ensure right-hand trigger input
        if (!rightHandDevice.isValid)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.HeldInHand |
                InputDeviceCharacteristics.Right |
                InputDeviceCharacteristics.Controller,
                devices);
            if (devices.Count > 0)
                rightHandDevice = devices[0];
            else
                Debug.LogWarning("[GazeMenuController] No right-hand controller found.");
        }

        if (isGazeHit == true)
        {
            // Invoke whichever target is hit
            if (isRightTriggerPressed == true)
            {

                Debug.Log("11111111111111111111111111");
        
                AfterSelection();
            }
        }
    }

    private void AfterSelection()
    {
       
        // Hide menu
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
        // Enable movement (rotation unaffected)
        if (movementController != null)
            movementController.enabled = true;
    }
}

/// <summary>
/// Interface for menu targets.
/// </summary>
public interface IMenuTarget
{
  
}