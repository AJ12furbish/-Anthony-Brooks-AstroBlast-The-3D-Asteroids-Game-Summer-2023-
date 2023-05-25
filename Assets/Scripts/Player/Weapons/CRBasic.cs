using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRBasic : MonoBehaviour, Weapons
{
    //Instance Variables
    public float speed;
    public int damage;
    public int critChance = 5;
    public bool allowScreenWrap = false;


    public float existDuration = 1.5f; //How long the last will stay in the game for
    private float existTimer = 0.0f;
    public float currentCharge;
    
    private Rigidbody chargeRifle;
    public GameObject spark;

    // Start is called before the first frame update
    void Start()
    {
        chargeRifle = GetComponent<Rigidbody>();

        chargeRifle.velocity = transform.forward * speed;

        //Rotation wil be set by the player when spawned;

    }

    // Update is called once per frame
    void Update()
    {
        if(currentCharge < 0.5f)
        { damage = 5; speed = 10; }
        else if (currentCharge > 0.5f && currentCharge < 1.0f)
        { damage = 10; speed = 15; }
        else if (currentCharge > 1f && currentCharge < 1.5f)
        { damage = 15; speed = 20; }
        else
        { damage = 20; speed = 25; }

        chargeRifle.velocity = transform.forward * speed;

        if (allowScreenWrap)
            Boundary.checkBoundary(gameObject);

        //Increase the duration timer
        existTimer += Time.deltaTime;

        //Debug.Log("Laser Time: " + existTimer + " of " + existDuration);

        //Delete the laser if been alive too long
        //THIS MUST BE THE LAST THING IN UPDATE
        if (existTimer > existDuration)
        {
            Destroy(this.gameObject);
        }


    }

    public int getDamage()
    {
        bool isCritical;
        //Generate a random number from 1 to 100
        int rand = Random.Range(1, 100);

        //If that random number is above 100 - critChance
        if (rand >= 100 - critChance)
            isCritical = true;
        else
            isCritical = false;

        //Calculate Damage
        int actualDamage = damage;
        if (isCritical)
            actualDamage = (int)(actualDamage * 2.0);

        Color textColor = new Color(255, 150, 0); //Same orange as the laser
        Vector3 textSpawnLocation = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        //Create the damage popup
        DamagePopup.Create(textSpawnLocation, actualDamage, textColor, isCritical);


        return actualDamage;

    }

    public GameObject getSpark()
    {
        return spark;
    }
}
