using UnityEngine;
using System;
using System.Collections;

/* found in gamedev.stackexchange thread by user pip 
 * https://gamedev.stackexchange.com/questions/96878/how-to-animate-objects-with-bobbing-up-and-down-motion-in-unity */

public class FloatBehavior : MonoBehaviour
{
    float originalY;

   [SerializeField]
   public float floatStrength = 1; 

    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x,
            originalY + ((float)Math.Sin(Time.unscaledTime) * floatStrength),
            transform.position.z);
    }
}