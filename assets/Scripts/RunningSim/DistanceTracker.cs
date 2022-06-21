using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 currentPositon;

    private float distanceTravelled;

    private TMP_Text distanceTravelledText;

    public float EnduranceCheckIncrement;

    public float EnduranceCheckFloat;

    public float distance;

    public bool raceStarted;

    public event Action OnDistanceCheck;
    // set by the running entity attached to this game object 
    
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        if (distanceTravelledText == null)
        {
            distanceTravelledText = GameObject.Find("Distance Travelled Text").GetComponent<TMP_Text>();
        }

        distanceTravelled = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (raceStarted)
        {
            var position = transform.position;
            var distance = Vector3.Distance(position, lastPosition);
            distanceTravelled += distance;
            EnduranceCheckFloat += distance;
            lastPosition = position;

            distanceTravelledText.text = Mathf.Round(distanceTravelled).ToString(CultureInfo.InvariantCulture);
            DistanceCheckCallback(EnduranceCheckIncrement); // endurance check increment needs to be set.
        }
        
        
    }


    private void DistanceCheckCallback(float enduranceCheckIncrement)
    {
        if ((int)EnduranceCheckFloat >= (int)enduranceCheckIncrement / 5)
        {
            OnDistanceCheck?.Invoke();
            EnduranceCheckFloat = 0;
        }
    }
    
    
}
