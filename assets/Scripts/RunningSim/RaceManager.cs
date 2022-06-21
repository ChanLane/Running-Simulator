using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class RaceManager : MonoBehaviour
{

    [SerializeField] private TMP_Text RaceTimeText;

    [SerializeField] private bool raceStarted;
    [SerializeField] private bool raceFinished;

    private Stopwatch raceTimer;
    private Stopwatch reactionTimer;

    [SerializeField] private TMP_Text setText;
    [SerializeField] private TMP_Text goText;

    public static event Action OnRaceStarted;
    
    // Start is called before the first frame update
    void Start()
    {
        RaceTimeText = GameObject.Find("RaceTimeText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (raceStarted)
        {
            UpdateRaceTimerUI();
        }
    }


    public void StartRaceTimer()
    { 
        raceTimer = new Stopwatch();
        raceTimer.Start();
        raceStarted = true;
    }

    public void UpdateRaceTimerUI()
    {
        RaceTimeText.text = raceTimer.Elapsed.TotalSeconds.ToString("F2");
    }


    public IEnumerator StartRaceRoutine()
    {
        yield return new WaitForSeconds(.5f);
        setText.gameObject.SetActive(true);

        var randomTime = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(randomTime);
        setText.gameObject.SetActive(false);
        goText.gameObject.SetActive(true);
        
        OnRaceStarted?.Invoke();
        goText.gameObject.SetActive(false);
        StartRaceTimer();
    }
}
