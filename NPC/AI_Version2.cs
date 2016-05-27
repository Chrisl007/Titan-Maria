/*
 
        @Author - Chris Lafond
 
        @Date -  March 1st 2016
        
        @Script -
 
        @Connections - requires rigidbody attached to enemy.
        
        @Modified - 1/25/16
 
        @TODO - Implement Nav mesh agent for better collision detections
 
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))] //need rigidbody to work
public class AI_Version2 : MonoBehaviour
{
    #region Variables
    //--------------------------------------Public Variables------------------------------\\
    //Inspector initiated variables. Defaults are set for ease of use.
    //They will be set by level designers/programmers
    public bool on = true; //Is the AI active? this can be used to place pre-set enemies in you scene.
    public bool runAway = false; //Is it the goal of this AI to keep it's distance? If so, it needs to have runaway active.
    public bool runTo = false; //Opposite to runaway, within a certain distance, the enemy will run toward the target.
    public float runDistance = 25.0f; //If the enemy should keep its distance, or charge in, at what point should they begin to run?
    public float runBufferDistance = 50.0f; //Smooth AI buffer. How far apart does AI/Target need to be before the run reason is ended.
    public float visualRadius = 100.0f; //How close does the player need to be to be seen by the enemy? Set to 0 to remove this limitation.
    public float moveableRadius = 200.0f; //If the player is too far away, the AI will auto-matically shut down. Set to 0 to remove this limitation.
    public float attackRange = 10.0f; //How close does the enemy need to be in order to attack?
    public float attackTime = 0.50f; //How frequent or fast an enemy can attack (cool down time).
    public bool useWaypoints = false; //If true, the AI will make use of the waypoints assigned to it until over-ridden by another functionality.
    public Transform[] waypoints; //define a set path for them to follow.
    int index = 0;
    public bool pauseAtWaypoints = false; //if true, patrol units will pause momentarily at each waypoint as they reach them.
    public float pauseMin = 1.0f; //If pauseAtWaypoints is true, the unit will pause momentarily for minmum of this time.
    public float pauseMax = 3.0f; //If pauseAtWaypoints is true, the unit will pause momentarily formaximum of this time.
    public float huntingTimer = 5.0f; //Search for player timer in seconds. Minimum of 0.1

    public bool requireTarget = true; //Waypoint ONLY functionality (still can fly and hover).
    public Transform target; //The target, or whatever the AI is looking for.
    //public Transorm wayPoints
    public float maxSpeed = 30f; //Movement speed if it needs to run.
    public float normalSpeed = 10f;
    public float attackSpeed = 25f;
    public List<Vector3> EscapeDirections = new List<Vector3>();
    public TitanNPCHealth npcHealth; //  reference to player health

    //----------------private script handled variables-----------------------------------\\
    private bool initialGo = false; //AI cannot function until it is initialized.
    private bool go = true; //An on/off override variable

    Vector3 acceleration;
    Vector3 velocity;
    float storeMaxSpeed;//this will store max speed
    float targetSpeed; //speed of target
    Vector3 storeTarget; //target position
    Vector3 newTargetPos;
    bool savePos;// saves position of target
    bool overrideTarget;//this will get new target by overriding previous
    Rigidbody rigidbody; //this version will use rigidbodies will update to better method later
    Transform obstacle;
    private Vector3 lastVisTargetPos; //Monitor target position if we lose sight of target. provides semi-intelligent AI.
    #endregion Variables

    /// <summary>
    /// ADD TITAN STATE MACHINE!!!! OR behavior tree
    /// </summary>
    public enum AIstates
    {
        normal,
        attacking,
        evade
    }
    public AIstates aiState;
    //---Starting/Initializing functions---//
    void Start()
    {
        storeMaxSpeed = maxSpeed;
        targetSpeed = storeMaxSpeed;
        rigidbody = GetComponent<Rigidbody>();
        npcHealth = GetComponent<TitanNPCHealth>();
    }

    void Update()
    {
        ManagerSpeed();
        switch (aiState)
        {
            case AIstates.attacking:
                targetSpeed = attackSpeed;

                break;
            case AIstates.evade:
                targetSpeed = storeMaxSpeed;

                break;
            case AIstates.normal:
                targetSpeed = normalSpeed;
                break;
        }
    }

    void FixedUpdate()
    {
        if (!on)
        {

            return;

        }
        else
        {

            AIFunctionality();

        }

    }

    void AIFunctionality()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        //draw line to target
        Debug.DrawLine(transform.position, target.position);

        if (npcHealth.currentHealth > 0)
        {
            Vector3 forces = MoveTowardsTarget(target.position);
            //  Vector3 forces = MoveTowardsTarget(waypoints[index.position);

            acceleration = forces; //this can be tweaked and avoidance can be done using this

            velocity += 2 * acceleration * Time.deltaTime; //automatically will clamp to position

            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            rigidbody.velocity = velocity;

            Quaternion desiredRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 3);

            ObstacleAvoidance(transform.forward, 0);
            //ObstacleAvoidance(transform.forward, 0); use this method to optimaze raycast/collision points
            if (distance > 250)
            {
                rigidbody.velocity = Vector3.zero;
                //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (overrideTarget)
            {
                target.position = newTargetPos;
            }

        }

        else
        {

            //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 forces = MoveTowardsTarget(target.position);
            //  Vector3 forces = MoveTowardsTarget(waypoints[index.position);

            acceleration = forces; //this can be tweaked and avoidance can be done using this

            velocity += -2 * acceleration * Time.deltaTime; //automatically will clamp to position

            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            rigidbody.velocity = velocity;
            Quaternion desiredRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 3);

            ObstacleAvoidance(transform.forward, 0);
        }
    }
    /// <MoveTowardsTarget>
    /// Helper Vector for storing target position
    /// </MoveTowardsTarget>
    /// <param name="target"></param>
    Vector3 MoveTowardsTarget(Vector3 target)
    {
        //distance is target minus position
        Vector3 distance = target - transform.position;

        if (distance.magnitude < 5)//this number can be tweaked
        {
            //if close
            return distance.normalized * -maxSpeed;
        }
        else
        {
            //if not close
            return distance.normalized * maxSpeed;
        }
    }

    /// <ObstacleAvoidance>
    /// Helper Function for avoiding obstacle
    /// Uses RaycastHit[] array
    /// </ObstacleAvoidance>
    /// <param name="direction"></param>
    /// <param name="offsetX"></param>
    void ObstacleAvoidance(Vector3 direction, float offsetX)
    {
        RaycastHit[] hit = Rays(direction, offsetX);

        for (int i = 0; i < hit.Length - 1; i++)
        {
            //ignore colliders attached to object
            if (hit[i].transform.root.gameObject != this.gameObject)
            {
                if (!savePos)
                {
                    storeTarget = target.position;
                    obstacle = hit[i].transform;
                    savePos = true;
                }
                FindEscapeDirections(hit[i].collider);
            }
        }

        if (EscapeDirections.Count > 0)
        {
            if (!overrideTarget)
            {
                newTargetPos = getClosets();
                overrideTarget = true;
            }
        }

        float distance = Vector3.Distance(transform.position, target.position);

        //Debug.Log(distance);
        if (distance < 5)
        {
            if (savePos)
            {
                target.position = storeTarget;
                savePos = false;
            }
            /******Waypoint functionality*****/
            //else if (!savePos)
            //{
            //index++;
            // }
            // */
            overrideTarget = false;

            EscapeDirections.Clear();
        }
    }

    /// <summary>
    /// Helper function to closest escape
    /// </summary>
    /// <returns></returns>
    Vector3 getClosets()
    {
        Vector3 clos = EscapeDirections[0];
        float distance = Vector3.Distance(transform.position, EscapeDirections[0]);

        foreach (Vector3 dir in EscapeDirections)
        {
            float tempDistance = Vector3.Distance(transform.position, dir);

            if (tempDistance < distance)
            {
                distance = tempDistance;
                clos = dir;
            }
        }

        return clos;
    }

    /// <summary>
    /// This Method will find obstacles and determine escape directions
    /// Todo: Add co-routine to optimize performance
    /// </summary>
    /// <param name="col"></param>
    void FindEscapeDirections(Collider col)
    {
        RaycastHit hitUp;
        //Can it swim up?
        if (Physics.Raycast(col.transform.position, col.transform.up, out hitUp, col.bounds.extents.y))
        { }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(0, col.bounds.extents.y * 2, 0);
            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        RaycastHit hitDown;
        //Can it swim down?
        if (Physics.Raycast(col.transform.position, -col.transform.up, out hitDown, col.bounds.extents.y))
        { }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2, 0);
            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        RaycastHit hitRight;
        //Can it swim right?
        if (Physics.Raycast(col.transform.position, col.transform.right, out hitRight, col.bounds.extents.x * 2))
        { }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(col.bounds.extents.x * 2, 0, 0);
            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        RaycastHit hitLeft;
        //Can it swim left?
        if (Physics.Raycast(col.transform.position, -col.transform.right, out hitLeft, col.bounds.extents.x))
        { }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x, 0, 0);
            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        /* Method to add more directions
         RaycastHit hitVar;
        //Can it swim var?
        if (Physics.Raycast(col.transform.position, -col.transform.right, out hitLeft, col.bounds.extents.x * 2))
        { }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x * 2, 0, 0);
            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        } 
         */
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="offsetX"></param>
    /// <returns></returns>
    RaycastHit[] Rays(Vector3 direction, float offsetX)
    {
        Ray ray = new Ray(transform.position + new Vector3(offsetX, 0, 0), direction);
        Debug.DrawRay(transform.position + new Vector3(offsetX, 0, 0), direction * 5 * maxSpeed, Color.red);  //debug function
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 7;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;
        //how far ahead to look
        float distanceToLookAhead = maxSpeed * 5; //can be tweaked
        RaycastHit[] hits = Physics.SphereCastAll(ray, 5, distanceToLookAhead, layerMask);//can be tweaked

        return hits;
    }

    void ManagerSpeed()
    {
        maxSpeed = Mathf.MoveTowards(maxSpeed, targetSpeed, Time.deltaTime * 5);
    }

    void Patrol()
    {

    }
}