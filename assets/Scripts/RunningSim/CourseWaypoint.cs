using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class CourseWaypoint : MonoBehaviour
{
    public static event Action<CourseWaypoint> OnRunnerEnter;
  
    
    private void OnTriggerEnter(Collider other)
    {
       Debug.Log("Waypoint Reached");
        
        if (other.CompareTag("Runner"))
        {
            OnRunnerEnter?.Invoke(this);
        }
    }
}
