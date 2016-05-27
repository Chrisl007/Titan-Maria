/*
        @Author - Chris Lafond

        @Date -  September 25th

        @Script -

        @Connections - requires CharacterController attached to enemy.

        @Modified - 4/25/16

        @TODO -  Math for time.deltaTime
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TitanMaria.StateMachine;

public class TitanPlayerState : StateBehaviour
{
    private const float NORMALSPEED = 30.0f;
    private const float NORMALJUMPSPEED = 10.0f;
    private const float UWSPEED = 30.0f;
    private const float UWJUMPSPEED = 38.0f;
    private const float UW2SPEED = 40.0f;
    private const float UW2JUMPSPEED = 48.0f;

    //Declare which states we'd like use
    public enum States
    {
        Floating,
        Normal,
        UnderWater,
        UnderWater2,
        NormalAboveWater,
        Death,
        DebugMode
        //,Win,
        //Lose
    }

   
    public GameObject skyPlaneBarrier;
   
    public Transform shotSpawn;
    public float fireRate;
    private float nextFire;
  

    public float rotationSpeed = 5.0f;
    public float currentRotation = 1.0f;

    public static TitanPlayerState Instance;

    public Transform myTransform;

    public static Rigidbody myRigidbody;
    
    public float Speed = NORMALSPEED;
    public float JumpForce = NORMALJUMPSPEED;
    public float SpecialJumpForce = UWJUMPSPEED;
    public float maxVelocityChange = 10.0f;
    public float waterLiftingForce = 5.5f;//1.55f;
    public float swimmingForce = 5.5f;// 1.25f;
    public float gravityForce = 9.81f;
    public float depth;
    //public bool isFalling = true;
    public bool waterLifting = false;
    public bool grounded = false;
    public bool canJump = true;
    public bool isFloating = false;
    public bool isDebugging = false;
    public bool isDead = false;
    public Text depthText;
    public TitanPlayerState.States pState = States.Floating;

    //Private Variables
    TitanPlayerHealth playerHealth;
    Animator anim; // Reference to the animator component. 
    GameObject defend;
    GameObject scan;
    GameObject radar;           
              
    private void Awake()
    {


        //Initialize State Machine Engine
        Initialize<States>();

        //Change to our first state
        ChangeState(States.Floating);
        playerHealth = GetComponent<TitanPlayerHealth>();
        defend = transform.FindChild("Sonar_Ping_Defense_Attack").gameObject;
        scan = transform.FindChild("Long_Range_Scan").gameObject;
        radar = transform.FindChild("Sonar_Radar_Scan").gameObject;
        //anim = GetComponentInChildren<Animator>();
        myRigidbody = GetComponent<Rigidbody>() as Rigidbody;
        Instance = this;
        myTransform = transform;
        myRigidbody.freezeRotation = true;
        myRigidbody.mass = 10;

    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        StateChanger();
        UpdatePlayerState(pState);
        GetLocomotionInput();
        GetDepth();
        SetDepthText();
        EnableDrag();

    }
    void OnGUI()
    {
        string myString = "Currently in God Mode";
        string uDead = "Sorry you dead!";
        var textWidth = myString.Length * 10;
        if (isDebugging)
        {
            GUI.Label(new Rect((Screen.width / 2 - textWidth / 2) + 25, Screen.height / 3, textWidth, 20), myString);
        }
        if(isDead)
        {
            GUI.Label(new Rect((Screen.width / 2 - textWidth / 2) + 25, Screen.height / 3, textWidth, 20), uDead);
        }
    }
    void FixedUpdate()
    {
        var state = GetState();
        if (state.Equals(States.Normal))
        {
            //Force Change State to Normal grounded && depth < -5.200365f)
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.Normal, StateTransition.Overwrite);

            //calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            //jump
            if (canJump && Input.GetButton("Jump"))
            {

                print("Jumping");
                myRigidbody.AddForce((transform.up) * JumpForce, ForceMode.Impulse);

            }

        }
        if (state.Equals(States.DebugMode))
        {
            //Force Change State to Normal grounded && depth < -5.200365f)
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.Normal, StateTransition.Overwrite);

            //calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            //jump
            if (Input.GetButton("Jump"))
            {

                print("Jumping");
                myRigidbody.AddForce(2 * (transform.up) * JumpForce, ForceMode.Impulse);

            }

            if (Input.GetButton("Descend"))
            {

                print("Sinking");
                myRigidbody.AddForce(2* ((-transform.up) * JumpForce), ForceMode.Impulse);
            }

        }
        if (grounded && depth > -5.200365f)
        {
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.Normal, StateTransition.Safe);

            //calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            //velocityChange.y =0;
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, 0);

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        //floating
        if (state.Equals(States.Floating))
        {
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.Floating, StateTransition.Safe);
            //calculate how fast we should be moving

            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            //add constant upward force
            myRigidbody.AddForce((Vector3.up * gravityForce), ForceMode.Acceleration);
            //Sinking
            if (Input.GetButton("Descend") && isFloating)
            {

                print("Sinking");
                myRigidbody.AddForce(((-transform.up) * JumpForce), ForceMode.Impulse);
            }


        }
        //end floating
        if (state.Equals(States.Death))
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Application.LoadLevel(2);
        }
        if (state.Equals(States.UnderWater2))
        {
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.UnderWater, StateTransition.Safe);
            //calculate how fast we should be moving

            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            //Descend
            if (Input.GetButton("Descend"))
            {
                if (myRigidbody.velocity.y < 5.0f)
                {
                    print("descending");
                    //force mode.Impulse add an instant impulse using its mass
                    myRigidbody.AddForce((-transform.up) * swimmingForce, ForceMode.Impulse);
                }
            }
            //Swim

            if (Input.GetButton("Jump"))
            {
                if (myRigidbody.velocity.y < 5.0f)
                {
                    print("ascending");
                    //force mode.Impulse add an instant impulse using its mass
                    myRigidbody.AddForce( (((transform.up) * SpecialJumpForce)), ForceMode.Impulse);
                }
            }
            myRigidbody.AddForce(Vector3.up * waterLiftingForce, ForceMode.Impulse);
        }


        else if (state.Equals(States.UnderWater))
        {
            //TitanPlayerState.Instance.ChangeState(TitanPlayerState.States.UnderWater, StateTransition.Safe);
            //calculate how fast we should be moving

            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            //Apply a force that attempts to reach target velocity
            Vector3 velocity = myRigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //force mode. add velocity
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            //Descend
            if (Input.GetButton("Descend"))
            {
                if (myRigidbody.velocity.y < 5.0f)
                {
                    print("descending");
                    //force mode.Impulse add an instant impulse using its mass
                    myRigidbody.AddForce((-transform.up) * swimmingForce, ForceMode.Impulse);
                }
            }
            //Swim

            if (Input.GetButton("Jump") && canJump)
            {
                if (myRigidbody.velocity.y < 5.0f)
                {
                    print("ascending");
                    //force mode.Impulse add an instant impulse using its mass
                    myRigidbody.AddForce(((transform.up) * SpecialJumpForce), ForceMode.Impulse);
                }
            }
            myRigidbody.AddForce(Vector3.up * waterLiftingForce, ForceMode.Impulse);
        }

        grounded = false;
    }

    //Method to Get Depth
    void GetDepth()
    {
        depth = myTransform.position.y;
    }
    void SetDepthText()
    {
        depthText.text = depth.ToString("0.00") + "m";
    }
    //method to set isFalling|| depth > -5.1f
    void EnableDrag()
    {
        if (depth < 2.0f && depth > -7.2f)
        {
            myRigidbody.drag = .75f;
        }
        else
            myRigidbody.drag = 0;
    }
    void StateChanger()
    {
        //DebugMode();
        if (isDebugging == false)
        {
            skyPlaneBarrier.SetActive(true);
            myRigidbody.useGravity = true;
            if (playerHealth.currentHealth <= 0)
            {
                Debug.Log("state: Dead :( ");
                Instance.ChangeState(States.Death, StateTransition.Overwrite);
            }
            /**/
            // This will change the state.
            if (depth < -5.3f && !isFloating)
            {
                // Change state to underwater
                Instance.ChangeState(States.UnderWater, StateTransition.Safe);
            }

            if (!isFloating && depth < -5)
            {
                // Change state to underwaterdepth <= -250f &&
                Instance.ChangeState(States.UnderWater2, StateTransition.Safe);
            }

            if (isFloating)
            {
                // Change state to floating &
                Instance.ChangeState(States.Floating, StateTransition.Safe);
            }

            if (!isFloating & depth > -5.200365f)
            {
                Instance.ChangeState(States.NormalAboveWater, StateTransition.Safe);
            }

            if (grounded & depth < -5.200365f)
            {
                Instance.ChangeState(States.Normal, StateTransition.Overwrite);
            }
        }
        //Debugging Function
        else if (isDebugging == true)
        {
            skyPlaneBarrier.SetActive(false);
            Instance.ChangeState(States.DebugMode, StateTransition.Overwrite);
        }
        //End Debugging Function
    }
    void GetLocomotionInput()
    {
        // Defending
        if (Input.GetButton("Fire2") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            defend.SetActive(true);
            print("Using Defense");
           
        }
        else
        {
            defend.SetActive(false);
        }
        // end defending

        //Long Range Scanning
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            scan.SetActive(true);
            print("Using Scanning");
            
        }
        else
        {
            scan.SetActive(false);
        }
        // end Long Range Scanning

        //radar
        if (Input.GetButton("Fire3"))
        {
            nextFire = Time.time + fireRate;
            radar.SetActive(true);
            print("Using Radar");

        }
        else
        {
            radar.SetActive(false);
        }
        //end radar 

        //Will refactor post prototyping
        //float inputR = Mathf.Clamp(Input.GetAxis("Mouse X"), -1.0f, 1.0f);

        if (Input.GetButton("Rotate_Right"))
        {
            currentRotation++;
        }
        if (Input.GetButton("Rotate_Left"))
        {
            currentRotation--;
        }

        //currentRotation = Helper.ClampedAngle(currentRotation + (inputR * rotationSpeed));
        Quaternion rotationAngle = Quaternion.Euler(0.0f, currentRotation, 0.0f);
        myTransform.rotation = rotationAngle;
    }

    //Player States
    public void UpdatePlayerState(States pState)
    {
        var state = GetState();

        if (state.Equals(States.Floating))
        {
            Speed = NORMALSPEED;
            JumpForce = NORMALJUMPSPEED;
            waterLifting = true;
            canJump = true;
            myRigidbody.useGravity = true;
            //isFalling = false;
            //Debug.Log("state: Floating");
        }
        if (state.Equals(States.Normal))
        {
            Speed = NORMALSPEED;
            JumpForce = NORMALJUMPSPEED;
            waterLifting = true;
            myRigidbody.useGravity = true;
            //isFalling = true;
            //Debug.Log("state: Normal");
        }

        /*if (state.Equals(States.NormalAboveWater))
        {
            Speed = NORMALSPEED;
            JumpForce = 0;
            waterLifting = false;
            canJump = false;
            isFalling = true;
            Debug.Log("state: NormalAboveWater");
        }*/

        if (state.Equals(States.UnderWater))
        {
            Speed = UWSPEED;
            JumpForce = UWJUMPSPEED;
            waterLifting = true;
            canJump = true;
            myRigidbody.useGravity = true;
            //isFalling = false;
            // Debug.Log("state: UnderWater");
        }
        if (state.Equals(States.UnderWater2))
        {
            Speed = UW2SPEED;
            JumpForce = UW2JUMPSPEED;
            waterLifting = true;
            canJump = true;
            myRigidbody.useGravity = true;

            //isFalling = true;
            //Debug.Log("state: UnderWater2");
        }
        if (state.Equals(States.Death))
        {
            Speed = 0;
            JumpForce = 0;
            waterLifting = true;
            canJump = false;
            isDead = true;
            Debug.Log("state: Dead :( ");
            //myRigidbody.constraints = RigidbodyConstraints.FreezeAll | RigidbodyConstraints.FreezePosition;
        }
        if (state.Equals(States.DebugMode))
        {
            Speed = 65;
            JumpForce = 20;
            waterLifting = false;
            canJump = true;
            // isFalling = false;
            myRigidbody.useGravity = false;
            myRigidbody.drag = 0f;
            Debug.Log("state: DebugMode");
            //myRigidbody.constraints = RigidbodyConstraints.FreezeAll | RigidbodyConstraints.FreezePosition;
        }
    }

    /*********************************COlliders/Triggers**********************************/
    public void OnCollisionStay()
    {
        grounded = true;
    }

    //is Floating
    public void OnTriggerEnter(Collider other)
    {

        //if (other.CompareTag("Water"))
        //isFloating = true;
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
            isFloating = true;

    }
    //end floating
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
            isFloating = false;

    }

   
    /*
    public void DebugMode()
    {
        //start debug controls /** /

        if (Input.GetKeyDown(KeyCode.G) && !isDebugging)
        {
            //Debug.Log("Setting state to DebugMode");
            isDebugging = true;
        }

        else if (Input.GetKeyDown(KeyCode.G) && isDebugging)
        {
            isDebugging = false;
        }
        //End debug testing controls
    }*/
}

