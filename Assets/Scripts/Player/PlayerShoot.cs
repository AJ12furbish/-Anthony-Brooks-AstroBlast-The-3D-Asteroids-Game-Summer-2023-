using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    //Instance Variables
    //Gameplay Variables
    private int currentWeapon = 0;

    //Rate of Fire (time inbetween Attacks)
    public float attackCooldown = 0.2f; //Fire 5 times a second
    private float attackTimer = 0.0f;
    public float currentCharge = 0.0f;

    //50dps

    //PREFAB LINKS
    //**You need to set these in the Unity Editor**
    public GameObject laser; //The laser prefab to spawn
    public GameObject chargeRifle;

    //Component Variable
    public GameObject shotSpawnPoint; //The hardpoint the laser is going to appear from
    private AudioSource sound;


    // Start is called before the first frame update
    void Start()
    {
        //Ways to link the "Laser Hardpoint" child object to the variable
        //Can crash if you respawn the player
        //shotSpawnPoint = GameObject.Find("Laser Hardpoint");

        sound = shotSpawnPoint.GetComponent<AudioSource>();

    } //end Start()

    // Update is called once per frame
    void Update()
    {
        //Cooldown the player's attack
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        //Fire the laser
        //Input.GetButton("Fire1")    //return true continuously while the Fire1 button is held
        //Input.GetButtonDown("Fire1")//return true ONCE the first frame Fire1 is down
        //Input.GetButtonUp("Fire1")  //return true ONCE the frame Fire1 is released

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (currentWeapon == 1)
                currentWeapon = 0;
            else
                currentWeapon = 1;
        }


        //How a laser will work: GetButton("Fire1") -> attack by holding down the mouse
        //                       GetButtonUp("Fire1") -> attack by tapping the mouse
        if (currentWeapon == 0)
        {
            if (Input.GetButton("Fire1") && attackTimer <= 0.0f)
            {
                //Spawn the laser
                GameObject tempLaser = Instantiate(laser, shotSpawnPoint.transform.position, shotSpawnPoint.transform.rotation);

                //Set the laser's damage
                //GetComponent<> can get a script and you can call methods or change variables


                //Set the attackTimer cooldown
                attackTimer = attackCooldown;

                //Play the sound effect
                sound.Play();
            }
        }
        else
        {
            

            if (Input.GetButton("Fire1") && attackTimer <= 0.0f)
            {
                currentCharge += Time.deltaTime;

            }

            if (Input.GetButtonUp("Fire1") && attackTimer <= 0.0f)
            {
                //Spawn the laser
                GameObject tempCR = Instantiate(chargeRifle, shotSpawnPoint.transform.position, shotSpawnPoint.transform.rotation);

                //Set the laser's damage
                //GetComponent<> can get a script and you can call methods or change variables
                tempCR.GetComponent<CRBasic>().currentCharge = this.currentCharge;
                this.currentCharge = 0;

                //Set the attackTimer cooldown
                attackTimer = attackCooldown;

                //Play the sound effect
                sound.Play();
            }
        }
    } //end Update()
}
