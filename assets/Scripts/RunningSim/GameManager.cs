using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private RaceManager _raceManagerRef;
    
    // Start is called before the first frame update
    void Start()
    {
        _raceManagerRef = GameObject.Find("RaceManager").GetComponent<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(_raceManagerRef.StartRaceRoutine());
        }
    }
}
