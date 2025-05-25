using UnityEngine;
using UnityEngine.UI;

public class ProximityCanvas : MonoBehaviour
{
    [Header("Distance threshold")]
    public float showDistance = 3f;    
    public float hideDistance = 5f;  
    public float checkInterval = 0.3f; 

    

    private Transform xrCamera;
    private Canvas canvasComponent;
    private Graphic[] uiElements;
    private Color[] originalColors;
    private bool isVisible;

    void Awake()
    {
        xrCamera = Camera.main.transform;
        
        canvasComponent = GetComponent<Canvas>();
        uiElements = GetComponentsInChildren<Graphic>(true);
        
        originalColors = new Color[uiElements.Length];
        for(int i=0; i<uiElements.Length; i++)
        {
            originalColors[i] = uiElements[i].color;
        }

        SetCanvasVisible(false);
        StartCoroutine(DistanceCheck());
    }

    System.Collections.IEnumerator DistanceCheck()
    {
        while(true)
        {
            float distance = Vector3.Distance(transform.position, xrCamera.position);
            
            bool shouldShow = isVisible ? 
                (distance <= hideDistance) : 
                (distance <= showDistance);

            if(shouldShow != isVisible)
            {
                SetCanvasVisible(shouldShow);
            }

            

            yield return new WaitForSeconds(checkInterval);
        }
    }

    void SetCanvasVisible(bool state)
    {
        isVisible = state;
        canvasComponent.enabled = state;
        
        if(!state)
        {
            for(int i=0; i<uiElements.Length; i++)
            {
                uiElements[i].color = originalColors[i];
            }
        }
    }

       void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, showDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hideDistance);
    }
}