using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayInteractor : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    public string targetTag = "Target";
    public bool isRaycastHit = false;

    void Update()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                Debug.Log("射线检测命中");
                isRaycastHit = true;
            }
            else
            {
                isRaycastHit = false;
            }
        }
        else
        {
            isRaycastHit = false;
        }
    }
}
