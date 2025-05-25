using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to the Quit image with tag "target".
/// Defines the action to quit menu (no scene change here, just logic hook).
/// </summary>
public class QuitTarget : MonoBehaviour, IMenuTarget
{
    public void Execute()
    {
        Debug.Log("[QuitTarget] Quit selected: unlocking movement.");
        // Any additional Quit logic here
    }
}