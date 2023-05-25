using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootCR : MonoBehaviour
{
    //Instance Variables
    public int damage = 5;
    //Rate of Fire
    public float attackCooldown = 0.5f; //2 time per second
    private float attackTimer = 0.0f;

    //Component Variable
    public GameObject chargeRifle;

    public GameObject shotSpawnPoint;
    private AudioSource sound;



    // Start is called before the first frame update
    void Start()
    {
        sound = shotSpawnPoint.GetComponent<AudioSource>();
    }

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

        //How a CR will work: GetButton("Fire1") -> attack by holding down the mouse
        //                       GetButtonUp("Fire1") -> attack by tapping the mouse
        if (Input.GetButtonUp("Fire1") && attackTimer <= 0.0f)
        {
            //Spawn the chargeRifle
            GameObject tempCR = Instantiate(chargeRifle, shotSpawnPoint.transform.position, shotSpawnPoint.transform.rotation);

            //Set the CR's damage
            //GetComponent<> can get a script and you can call methods or change variables

            //Set the attackTimer cooldown
            attackTimer = attackCooldown;

            //Play the sound effect
            sound.Play();

        }
    }
}
