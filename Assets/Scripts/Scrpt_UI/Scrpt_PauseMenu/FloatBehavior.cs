using UnityEngine;
using System;
using System.Collections;

/* found in gamedev.stackexchange thread by user pip 
 * https://gamedev.stackexchange.com/questions/96878/how-to-animate-objects-with-bobbing-up-and-down-motion-in-unity */

public class FloatBehavior : MonoBehaviour
{
    float originalY;

   //Change the intensity of the float in unity
   [SerializeField]
   public float floatStrength = 1; 

    void Start()
    {   
        //uses original position in calculation
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        //Changed to Time.unscaledTime so that the movement of the menu is not tied to the frozen game time
        transform.position = new Vector3(transform.position.x,
            originalY + ((float)Math.Sin(Time.unscaledTime) * floatStrength),
            transform.position.z);
    }
}