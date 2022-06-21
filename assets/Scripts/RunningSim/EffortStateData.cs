using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EffortState", menuName = "Create Effort State")]
public class EffortStateData : ScriptableObject
{

     public string buttonName;
     public float animSpeed;
     public float agentSpeed;

     public float enduranceLossPercentage;

     public bool animationTrigger;
     public string animStartString;
     public string animStopString;

     public Button stateButton;
     
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    
}
