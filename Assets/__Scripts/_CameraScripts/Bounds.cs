using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
       //Various fields to hold information required for bounds checking
    public float radius = 2f;
    public float camWidth;
    public float camHeight;
    
    public bool isOnScreen = true;
    public bool keepOnScreen = true;
    public bool offLeft, offRight, offTop, offBottom;

    void Start()
    {
    
    }

    //Initialize the height and width based on scene camera
    private void FixedUpdate()
    {
        camHeight = Camera.main.orthographicSize;
        print("in Bounds" + camWidth);
    }

    //Check if the object is out of the defined boundaries (based on camera and specified distance)
    void LateUpdate()
    {
        offLeft = offRight = offTop = offBottom = false;
        Vector3 pos = transform.position;
        isOnScreen = true;

        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offTop = true;
        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offBottom = true;
        }

        //If it is desired to keep this object on screen, transform its position to be on screen
        isOnScreen = !(offTop || offBottom || offRight || offLeft);
        if(keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offLeft = offRight = offTop = offBottom = false;
        }
    }

     //Show the boundaries when the application is not playing
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
    
}