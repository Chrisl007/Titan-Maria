using UnityEngine;
using System.Collections;
using TitanMaria.StateMachine;

//[RequireComponent(typeof(AI))]
[RequireComponent(typeof(TitanNPCHealth))]
public class TitanNeutralNPC : TitanNPC
{
    AI neutralAI;
    TitanNPCHealth npcHealth;
    int _hitPoints;
    // Use this for initialization
    void Start()
    {
        base.Start();
        npcHealth = GetComponent<TitanNPCHealth>();
        neutralAI =GetComponent<AI>();
    }

    // Update is called once per frame
    void Update()
    {
        NeutralAIFunctionality();
    }
    /// <summary>
    /// Neutral AI Functionality. 
    /// Removed from Update to make debugging easier
    /// </summary>
    void NeutralAIFunctionality()
    {
        _hitPoints = npcHealth.currentHealth;
        if (_hitPoints > 0)
        {
            NeutralBehavior();
        }

        else
        {
            RunAway();
        }
    }
    /// <summary>
    /// Neutral Behavior
    /// </summary>
    void NeutralBehavior()
    {
        neutralAI.runAway = false;
    }

    /// <summary>
    /// Runaway!!! AHHH!!!
    /// </summary>
    void RunAway()
    {
        neutralAI.runAway = true;
    }
}
