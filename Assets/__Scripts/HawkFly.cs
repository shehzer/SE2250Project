using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HawkFly : MonoBehaviour

{
    public int speed = 3;
   
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

  

    // Update is called once per frame
    void Update()
    {
        Move();

    }

   

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.x -= speed * Time.deltaTime;
        pos = tempPos;
    }

  
}
