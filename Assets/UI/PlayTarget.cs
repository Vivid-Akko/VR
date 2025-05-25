using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to the Play image with tag "target".
/// Defines the action to start the game (no scene change here, just logic hook).
/// </summary>
public class PlayTarget : MonoBehaviour, IMenuTarget
{
    public void Execute()
    {
        Debug.Log("[PlayTarget] Play selected: unlocking movement.");
        // Any additional Play logic here
    }
}