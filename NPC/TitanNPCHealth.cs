using UnityEngine;
using System.Collections;

public class TitanNPCHealth : MonoBehaviour
{

    public int startingHealth = 100;            // The amount of health the enemy starts the game with.
    public int currentHealth;                   // The current health the enemy has.
    //public AudioClip deathClip;                 // The sound to play when the enemy dies.

    //AudioSource enemyAudio;                     // Reference to the audio source.
    //ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
    CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
    TitanNPCHealth titanNPCHealth;
    bool isDead;                                // Whether the enemy is dead.
    bool isSinking;                             // Whether the enemy has started sinking through the floor.

    void Awake()
    {
        // Setting up the references.
        //enemyAudio = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        titanNPCHealth = this;
        // Setting the current health when the enemy first spawns.
        currentHealth = startingHealth;
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(addHealth());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator addHealth()
    {
        while (true)
        { // loops forever...
            if (currentHealth < 100)
            { // if health < 100...
                currentHealth += 1; // increase health and wait the specified time
                yield return new WaitForSeconds(1);
            }
            else
            { // if health >= 100, just yield 
                yield return null;
            }
        }
    }
    public void TakeDamage(int amount)
    {
        // If the enemy is dead..., Vector3 hitPoint

        //if (isDead)
            // ... no need to take damage so exit the function.
        // return;

        // Play the hurt sound effect.
        //enemyAudio.Play();

        // Reduce the current health by the amount of damage sustained.
        currentHealth -= amount;

        // If the current health is less than or equal to zero...
        if (currentHealth <= 0)
        {
            // ... the enemy is dead.
            Death();
        }
    }


    void Death()
    {
        // The enemy is dead.
        isDead = true;

        // Turn the collider into a trigger so shots can pass through it.
        //capsuleCollider.isTrigger = true;

        // Tell the animator that the enemy is dead.
        //anim.SetTrigger("Dead");

        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
        //enemyAudio.clip = deathClip;
        //enemyAudio.Play();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Defense"))
            TakeDamage(75);
        print("Collision detected with trigger object ");
    }
}