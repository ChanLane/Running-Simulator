using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerEntity : RunningEntity
{
    private Vector3 _aimPosition;
    private static readonly int Go = Animator.StringToHash("Go");
    private bool running;
    

    // Start is called before the first frame update
    
    
    void Start()
    {   
        // prototype
        playerEntity = true;
        SwitchEffortState(MediumEffortStateData); // medium effort
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchEffortState(RestoreEnergyStateData);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchEffortState(ConserveEnergyStateData);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchEffortState(MediumEffortStateData);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchEffortState(HighEffortStateData);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (CurrentEffortState != SprintStateData) // add this check here because we
                // dont want the trigger getting tripped twice
            {
                SwitchEffortState(SprintStateData);
            }
        }

        if (raceStarted)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AnimatorRef.SetTrigger(Go);
                StartRunning();
                running = true;
            }
        }
    }

    private void  FixedUpdate()
    {
        if (running)
        {
            Movement();
        } 
    }

    public override void Movement()
    {
        NavMeshAgentRef.destination = GetAimPosition();
    }
    
    private Vector3 GetAimPosition()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 <<LayerMask.NameToLayer("Ground")))
        {
            _aimPosition = hit.point;
        }

        return _aimPosition;
    }

    
}
