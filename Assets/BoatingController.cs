using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatingController : MonoBehaviour
{
    public float speed = 10f;
    public GameObject character;
    public GameObject characterCamera;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = character.transform.position;
        //transform.rotation = new Quaternion(transform.rotation.x, characterCamera.transform.rotation.y, transform.rotation.z, transform.rotation.w);

    }
}
