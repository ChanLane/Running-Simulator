using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIEntity : RunningEntity
{

    [SerializeField] private Transform target;
    private static readonly int Go = Animator.StringToHash("Go");


    public bool routineStarted;

    public bool stoppedCoroutine;

    private void Start()
    {
        SwitchEffortState(MediumEffortStateData);
        playerEntity = false;
        routineStarted = false;
    }

    public override void Movement()
    {
        NavMeshAgentRef.destination = target.position;
    }
    
    public override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            AnimatorRef.SetTrigger(Go);
            Movement();
            StartRunning();
            StartCoroutine(AIRaceRoutine());
        }

        if (routineStarted == false && raceStarted && energyDepleted == false)
        {
            StartCoroutine(AIRaceRoutine());
        }


        if (endurance == 0)
        {
            StopCoroutine(AIRaceRoutine());
            routineStarted = false;
        }
    }



    public IEnumerator AIRaceRoutine()
    {

        routineStarted = true;

        NavMeshAgentRef.speed += Random.Range(.03f, .02f);
        
        yield return new WaitForSeconds(Random.Range(20, 35f));
        
        var index = GenerateRandomindex(0, 3);

        
        if (endurance < 10)
        {
            index = GenerateRandomindex(0, 2);

            if (index == 0)
            {
                SwitchEffortState(ConserveEnergyStateData);
            }

            if (index == 1)
            {
                SwitchEffortState(SprintStateData);
            }
        }
        
        if (index == 0)
        {
            SwitchEffortState(SprintStateData);
            
        }

        if (index == 1)
        {
            SwitchEffortState(HighEffortStateData);
            
        }

        if (index == 2)
        {
            SwitchEffortState(MediumEffortStateData);

        }

         
        var randomModifier = Random.Range(.01f, .03f);
        NavMeshAgentRef.speed += randomModifier;
        AnimatorRef.speed += randomModifier;

        routineStarted = false;

    }



    private int GenerateRandomindex(int min, int max)
    {
        return Random.Range(min, max);
    }
}
