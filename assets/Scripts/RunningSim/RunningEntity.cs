using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(DistanceTracker))]
public class RunningEntity : MonoBehaviour
{

    #region Overview

    /// <summary>
    ///  --- 5 Effort states ---
    ///  1.) Energy Restore
    ///   - The slowest effort state, but your Endurance regenerates slowly while in this state.
    ///      base state speed - 3 m/s (can be upgraded)
    ///      anim State speed - ?
    ///      base energy restored per second - 2% -(can be upgraded and affected by equipment and abilities) 
    ///
    ///  2.) Conserve Energy
    ///    - The second slowest effort State. Endurance degrades the slowest out of all states.
    ///
    ///  3.) Medium Effort
    ///     - the third Fastest effort state. Endurance degrades quicker than Conserve Energy but slower than High Effort.
    ///
    ///  4.) High Effort.
    ///    - The Second fastest effort state. Endurance degrades quicker than Medium Effort but slower than sprint.
    ///
    ///   5.) Sprint
    ///     - The Hightest Effort State. Endurance degrades very quick.
    ///       
    ///
    ///
    ///   ---  Race Mechanics ----
    ///
    ///    If your Endurance hits 0 while you are sprinting, you are locked into energy restore until it regenerates to 75% 
    ///    if your Endurance hits 0 while in HighEffort you are locked into energy restore until 50% filled
    ///    if your Endurance hits 0 while in Medium effort you are locked into energy restore until 25% filled
    ///    if your Endurance hits 0 while in Conserve Energy you are locked into energy restore until 10% filled;
    ///
    ///     Sprint Meter controls how much full out sprinting you can do. It regenerates very slowly.
    ///     If Sprint mode is activated while the sprint meter is full, you get slightly more speed than you would otherwise.  
    ///
    ///     bumping into racers will cause you to lose speed
    ///
    ///     Changing your Effort too frequently will cost you endurance.
    ///     Swerving around too much will cost you endurance.
    ///
    ///     endurance bonuses for consistent pacing
    ///     endurance bonuses for keeping good race lines
    ///      
    ///     you can 
    /// 
    ///     Horrible Start - endurance depletes 3% more than normally for the first 100m of race, and all speed is decreased by 3%.
    ///     Bad Start  - endurance depletes 3% faster than normally for the first 100m of race
    ///     Mediocre Start - no bonus
    ///     Good Start - endurance depletes 3% slower for the first 100m of race
    ///     Excellent Start - endurance depletes 3% slower for the first 100m of race, all speed is increased by 3%
    ///
    ///   ---  Abilities ---
    ///    passive abilities can be equipped that effect the race. These are like "Materia" from FFVII that can be leveled up.
    ///    you start with 1 slot and can go up until 3 slots. 
    ///
    ///    Hot Out The Gate 
    ///    ----------------
    ///    lvl 1 - if high Effort Running is engaged within 5 seconds of the race starting, higher effort Endurance depletes 5% slower for the first 50 meters
    ///    lvl 2 - if high effort running is enganged withtin 5 seconds of the race starting endurance depletes 5% slower for the first 100 meters
    ///    lvl 3 - if high effort running is engaged within 5 seconds of the race starting endurance depletes 8% slower for the first 150 meters
    ///    lvl 4 - if high effort running is engaged within 5 seconds of the race starting endurace depletes 8% slower for the first 200 meters 
    ///    lvl 5 - if Sprint is engaged within 5 seconds of the race starting, Endurance and Sprint meter deplete 10% slower for the first 250 meters,
    ///            High Effort endurance depletes 5% slower for the duration of the race  
    ///
    ///   Heart of a Champion 
    ///   --------------------
    ///   lvl 1 - if endurance falls below 5%, once per race increase all efforts speed by 5% for the next 50m
    ///   lvl 2 - if endurance falls below 10%, once per race increase all efforts speed by 5% for the next 100m
    ///   lvl 3 - if endurance falls below 15%, once per race increase all efforts speed by 8% for the next 150m
    ///   lvl 4 - if endurance falls below 20%, once per race increase all efforts speed by 8% for the next 200m
    ///   lvl 5 - if endurance falls below 30%, twice per race increase all efforts speed by 10% for the next 250m,
    ///           all endurance depletes 2% slower.
    ///
    ///   More to come
    ///
    ///
    ///  Brands
    /// 
    ///
    ///  - Roka
    ///   3 pairs of running shoes
    ///   5 different accessories
    ///
    ///   Vike
    ///   -------
    ///   3 different pairs of running shoes
    ///   5 different accessories
    ///
    ///   Nooks 
    ///  3 pairs of running shoes
    ///  5 different accessories
    ///
    ///  
    ///
    ///
    /// 
    ///
    /// </summary>

    #endregion

    #region Variables
    
    private protected NavMeshAgent NavMeshAgentRef;
    private protected Rigidbody RigidBodyRef;
    private protected Animator AnimatorRef;
    
    [SerializeField] private protected EffortStateData CurrentEffortState;

    private protected EffortStateData DefaultEffortState;

    private protected bool playerEntity;

    private protected DistanceTracker distanceTrackerRef;

    public bool raceStarted;
    

  [SerializeField]  private protected EffortStateData SprintStateData;
  [SerializeField]  private protected EffortStateData HighEffortStateData;
  [SerializeField]  private protected EffortStateData MediumEffortStateData;
  [SerializeField]  private protected EffortStateData ConserveEnergyStateData;
  [SerializeField]  private protected EffortStateData RestoreEnergyStateData;

  [SerializeField] private TMP_Text currentSpeedText;
  [SerializeField] private TMP_Text enduranceText;

  [SerializeField] private protected float endurance;
  [SerializeField] private float MaxEndurance;

  private protected bool energyDepleted;


  #endregion
    // Start is called before the first frame update

    private void Awake()
    {
        NavMeshAgentRef = GetComponent<NavMeshAgent>();
        AnimatorRef = GetComponent<Animator>();
        RigidBodyRef = GetComponent<Rigidbody>();
        distanceTrackerRef = GetComponent<DistanceTracker>();
        distanceTrackerRef.OnDistanceCheck += HandleEndurance;
        distanceTrackerRef.EnduranceCheckIncrement = 50;
        RaceManager.OnRaceStarted += RaceStarted;



        endurance = MaxEndurance;
    }
    
    // Update is called once per frame
     public virtual void Update()
    {
        if (playerEntity)
        {
            enduranceText.text = endurance.ToString("F1");
        }
        
        if (endurance < 0)
        {
            endurance = 0;
        }

        if (endurance > MaxEndurance)
        {
            endurance = MaxEndurance;
        }


        if (endurance <= 0 && energyDepleted == false)
        {
            energyDepleted = true;
            StartCoroutine(EnergyDepletedRoutine());
        }
        
     
    }

    public virtual void Movement()
    {
        
    }

    protected void SwitchEffortState(EffortStateData nextEffortState)
    {
        if (energyDepleted)
        {
            if (CurrentEffortState == SprintStateData)
            {
                AnimatorRef.SetTrigger(CurrentEffortState.animStopString);
            }
            
            CurrentEffortState = RestoreEnergyStateData;
            NavMeshAgentRef.speed = CurrentEffortState.agentSpeed;
            AnimatorRef.speed = CurrentEffortState.animSpeed;
            
            return;
        }
        
        if (CurrentEffortState != null)
        {
            // stop previous phase animation if there is one
            if (CurrentEffortState.animationTrigger)
            {
                AnimatorRef.SetTrigger(CurrentEffortState.animStopString);
            }
        }

        CurrentEffortState = nextEffortState;
        NavMeshAgentRef.speed = CurrentEffortState.agentSpeed;
        AnimatorRef.speed = CurrentEffortState.animSpeed;

        if (playerEntity)
        {
            UpdateEffortUI(CurrentEffortState);
        }
        

        if (CurrentEffortState.animationTrigger)
        {
            AnimatorRef.SetTrigger(CurrentEffortState.animStartString);
        }
    }

    private void UpdateEffortUI(EffortStateData currentEffortState)
    {
        if (currentEffortState.stateButton == null)
        {
            currentEffortState.stateButton = GameObject.Find(currentEffortState.buttonName).GetComponent<Button>();
        }

        if (currentSpeedText == null)
        {
            currentSpeedText = GameObject.Find("Current Speed Text").GetComponent<TMP_Text>();
        }

        currentSpeedText.text = currentEffortState.agentSpeed + " m/s";
        currentEffortState.stateButton.enabled = true;
        currentEffortState.stateButton.onClick.Invoke();
        // de activate all others
    }

    private void HandleEndurance()
    {
        var enduranceChange = (float)(CurrentEffortState.enduranceLossPercentage * MaxEndurance * .01) / 5;
        
        endurance += enduranceChange;

        if (endurance <= 0 && energyDepleted == false)
        {
            energyDepleted = true;
            StartCoroutine(EnergyDepletedRoutine());
            
        }
    }

    private void OnDisable()
    {
        distanceTrackerRef.OnDistanceCheck -= HandleEndurance;
        RaceManager.OnRaceStarted -= RaceStarted;
    }


    public void StartRunning()
    {
        distanceTrackerRef.raceStarted = true;
    }
    
    public void RaceStarted() // hooked up to racemanager
    {
        raceStarted = true;
    }


    private IEnumerator EnergyDepletedRoutine()
    {
        if (CurrentEffortState == SprintStateData)
        {
            AnimatorRef.SetTrigger(CurrentEffortState.animStopString);
        }
        
        Debug.Log("EnergyDepleted");
        SwitchEffortState(RestoreEnergyStateData);
        yield return new WaitUntil( () => endurance > 75);
        energyDepleted = false;
        
        if (!playerEntity)
        {
            SwitchEffortState(SprintStateData);
            yield return new WaitForSeconds(Random.Range(3, 10));
            SwitchEffortState(HighEffortStateData);
        }
        
    }
}
