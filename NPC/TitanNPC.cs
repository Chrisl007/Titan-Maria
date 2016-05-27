/*
        @Author - Chris Lafond

        @Date -  March 31st

        @Script - This is the bas class for NPC characters

        @Connections - requires AI or AI_Version_2 script attached

        @Modified - 2/25/16
*/

using UnityEngine;
using System.Collections;
using TitanMaria.StateMachine;

[RequireComponent(typeof(AI))]
public abstract class TitanNPC : MonoBehaviour
{
    public string name;
    public int hitPoints;             // just a default for health
    public float moveRate;        // just a default for movement speed does nothing at the moment
    public bool isHostile = false;      // just a default for if NPC is neutral or hostile

    public TitanPlayerHealth playerHealth; //  reference to player health
    //public Animator anim;             // reference to animator
    public static TitanNPC Instance;
    // Use this for initialization
    public void Start()
    {
        Instance = this;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<TitanPlayerHealth>();
        hitPoints = GetComponent<TitanNPCHealth>().currentHealth;
        moveRate = GetComponent<AI>().randomSpeed;

    }


}
