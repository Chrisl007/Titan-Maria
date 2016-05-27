/*
        @Author - Chris Lafond

        @Date -  March 31st

        @Script -Titan Hostile NPC 

        @Connections - requires NPC health script

        @Modified - 1/25/16

*/

using UnityEngine;
using System.Collections;
using TitanMaria.StateMachine;

[RequireComponent(typeof(TitanNPCHealth))]
public class TitanHostileNPC : TitanNPC
{
    AI hostileAI;
    TitanNPCHealth enemyHealth;
    int _hitPoints;
    // Use this for initialization
    void Start()
    {
        base.Start();
        isHostile = true;
        hostileAI = GetComponent<AI>();
        enemyHealth = GetComponent<TitanNPCHealth>();
        _hitPoints = hitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        HostileAIFunctionality();

    }
    void HostileAIFunctionality()
    {
        _hitPoints = enemyHealth.currentHealth;
        if (_hitPoints > 0)
        {
            HostileBehavior();
        }

        else
        {
            RunAway();
        }
    }
    /// <summary>
    /// Hostil AI behavior
    /// </summary>
    void HostileBehavior()
    {
        hostileAI.runTo = true;
        hostileAI.runAway = false;
    }
    /// <summary>
    /// Runaway!!! AHHHH!!!
    /// </summary>
    void RunAway()
    {
        hostileAI.runAway = true;
        hostileAI.runTo = false;
    }
}
