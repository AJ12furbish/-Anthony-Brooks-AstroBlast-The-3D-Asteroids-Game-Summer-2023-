using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{

    //Instance Variables
    //Movement Variables
    public float moveSpeed = 12.0f; //Units per sec
    public float turnSpeed = 220.0f; //Degrees per sec
    [SerializeField] private bool useMouseControls = false;
    [SerializeField] private bool usePhysicsMovement = false;
    [SerializeField] private float maxSpeed = 10.0f;
    public GameObject explosion;
    private LevelManager manager;


    //Useful Variables
    private Rigidbody ship;
    private Light engineLight;
    private ParticleSystem engineParticles;
    private float lastParticleTime;
    private float particleRate = 1.0f / 30.0f; //Emit a particle every 1/30 of a second

    //Keyboard input
    private float inputV = 0.0f;
    private float inputH = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        //Link the Components to the variables
        ship = GetComponent<Rigidbody>();

        //engineLight = GameObject.Find("Engine Light").GetComponent<Light>();
        engineLight = GetComponentInChildren<Light>();
        engineParticles = GetComponentInChildren<ParticleSystem>();

        //Attach the GameController Variable
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

    } //end Start()

    // Update is called once per frame
    // Input gathering happens here
    void Update()
    {
        //Get Keyboard Input
        inputV = Input.GetAxis("Vertical");
        inputH = Input.GetAxis("Horizontal");


        //Debug.Log(inputV);
        //Turn off and on the Engine Light
        if (inputV != 0.0f)
        {
            //If we're pushing forward or backwards
            //If the light is off, turn it on
            if (engineLight.enabled == false)
                engineLight.enabled = true;

            //Limit the emission of particles to the variable particleRate
            float timeSinceLastParticle = Time.time - lastParticleTime;
            if (timeSinceLastParticle > particleRate)
            {
                engineParticles.Emit(1);
                lastParticleTime = Time.time;
            }

        }
        else
        {
            //If we're not pushing forward or backward
            if (engineLight.enabled == true && inputV <= 0.5f)
                engineLight.enabled = false;
        }

    } //end Update()

    //Called every 0.02s -- Physics engine updates
    private void FixedUpdate()
    {
        //*********
        // Movement
        //*********
        //Only move if the keys are being pressed.
        if (!usePhysicsMovement && inputV != 0.0f) //(Mathf.abs(inputV) > 0.01 )
        {
            Vector3 currentPosition = transform.position; //(x,y,z)
            Vector3 moveDirection = transform.forward * inputV * moveSpeed * Time.deltaTime;
            //                         (0,0,1)        * [-1,1] *  32       * 0.02

            ship.MovePosition(currentPosition + moveDirection);
            //inputV = 0; //Only if it "gets stuck" moving forward (due framerate issues)
        }
        else if (usePhysicsMovement && inputV > 0.0f)       //Pressing forward
        {
            //Make a force vector to apply to the ship.
            //Vector3 forwardForce = transform.forward * inputV * moveSpeed*50*ship.drag * Time.deltaTime;
            Vector3 forwardForce = transform.forward * inputV * moveSpeed * 50 * Time.deltaTime;

            //Convert from a world velocity to a local velocity
            Vector3 localVelocity = transform.InverseTransformDirection(ship.velocity);

            //Debug.Log(ship.velocity.magnitude); //Total direction speed
            //Debug.Log(localVelocity.z); //Total forward only speed

            if (localVelocity.z <= maxSpeed)
                ship.AddForce(forwardForce, ForceMode.Acceleration);
        }
        else if (usePhysicsMovement && inputV < 0.0f)       //Pressing back
        {
            //Make a force vector to apply to the ship.
            //Lower the moveSpeed multiplier to make reverse slower
            Vector3 forwardForce = transform.forward * inputV * moveSpeed * 50 * Time.deltaTime;

            //Convert from a world velocity to a local velocity
            Vector3 localVelocity = transform.InverseTransformDirection(ship.velocity);

            //Debug.Log(ship.velocity.magnitude); //Total direction speed
            //Debug.Log(localVelocity.z); //Total forward only speed

            if (localVelocity.z >= -maxSpeed * 0.5) //Caps reverse at -5 velocity
                ship.AddForce(forwardForce, ForceMode.Acceleration);
        }


        //*********
        // Rotation
        //*********
        if (!useMouseControls && inputH != 0.0f)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion rotationAmount = Quaternion.Euler(0f, inputH * turnSpeed * Time.deltaTime, 0f);

            //Rotate the ship
            ship.MoveRotation(currentRotation * rotationAmount);
            //inputH = 0;
        }
        else if (useMouseControls)
        {
            //Get Mouse Location
            Vector3 mousePos = Input.mousePosition;
            Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            //Compute the direction
            target.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            //Actually rotate, but with a smooth amount of rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);


            //Other useful commands
            int pixelWidth = Screen.width;
            int pixelHeight = Screen.height;
        } //end if Mouse Controls

        Boundary.checkBoundary(this.gameObject);

    }//end FixedUpdate

    private void OnTriggerEnter(Collider other)
    {
        //Check for an Asteroid
        if (other.gameObject.tag.Equals("Asteroid"))
        {
            //Save the last direction the player was facing
            manager.setLastPlayerRotation(ship.rotation);

            //Delete player and spawn explosion
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);

            //Respawn the player if possible
            manager.spawnPlayer();
        }

    } //end OnTriggerEnter()

}//end PlayerShip class
