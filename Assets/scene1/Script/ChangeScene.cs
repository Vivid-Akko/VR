using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject interactManager;
    public GameObject player;
    public float distanceCheck = 2.0f;
    
    private bool isRaycastHit = false;
    private bool isTriggerPressed = false;
    public bool isDistanceClose  =false;
    void Start()
    {
        isRaycastHit = interactManager.GetComponent<RayInteractor>().isRaycastHit;
        isTriggerPressed = interactManager.GetComponent<LeftTriggerByReference>().isLeftTriggerPressed;
        
    }

    // Update is called once per frame
    void Update()
    {
        isRaycastHit = interactManager.GetComponent<RayInteractor>().isRaycastHit;
        isTriggerPressed = interactManager.GetComponent<LeftTriggerByReference>().isLeftTriggerPressed;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < distanceCheck)
        {
            isDistanceClose = true;
        }
        else
        {
            isDistanceClose = false;
        }

        if(isRaycastHit && isDistanceClose)
        {
            if (isTriggerPressed)
            {
                // Load the new scene here
                Debug.Log("Loading SampleScene...");
                // Uncomment the line below to load the scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            }
            
        }
    }
}
