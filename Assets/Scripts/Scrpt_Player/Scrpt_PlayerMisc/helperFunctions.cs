using UnityEngine;
using System;

public class helperFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public bool togglePause()
	{
		if(Time.timeScale == 0f)
		{
			Time.timeScale = 1f;
			return(false);
		}
		else
		{
			Time.timeScale = 0f;
			return(true);	
		}
	}
}
