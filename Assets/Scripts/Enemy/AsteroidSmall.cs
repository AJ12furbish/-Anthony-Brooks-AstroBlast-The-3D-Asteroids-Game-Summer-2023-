using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSmall : MonoBehaviour
{
    //Instance Variables
    //Rotation Variables
    public float tumbleMin = 3.5f;
    public float tumbleMax = 6.5f;

    //Movement Variables
    public float moveSpeedMin = 2.5f;
    public float moveSpeedMax = 6.0f;
    private Vector3 moveDirection; //Randomly generation direction of motion
    private float speed; //The actual randomly generated speed

    //Components
    private Rigidbody body;

    //Gameplay Variables
    public GameObject explosion;
    public int maxHP = 20;
    private int hp;
    public int scoreValue = 150;

    private LevelManager manager;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        //Set up Asteroid "Tumbling" (Rotation speed)
        float tumbleSpeed = Random.Range(tumbleMin, tumbleMax);

        //Apply rotation to the Rigidbody
        body.angularVelocity = Random.onUnitSphere * tumbleSpeed;

        //Set up Asteroid "Movespeed"
        speed = Random.Range(moveSpeedMin, moveSpeedMax);
        moveDirection = Random.onUnitSphere;

        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;

        //Gameplay Stuff
        hp = maxHP;
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        manager.changeEnemyCount(1); //Add a new asteroid to the count

    } //end Start()

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            //Spawn the Explosion
            Instantiate(explosion, transform.position, transform.rotation);

            //Destroy the Asteroid
            Destroy(gameObject);

            //Update the number of enemies and score
            manager.changeScore(scoreValue);
            manager.changeEnemyCount(-1);
        }

    } //end Update()

    private void FixedUpdate()
    {
        //Manual, Non-Physics Movement
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        //Check for edges & screenwrap
        Boundary.checkBoundary(gameObject);

    } //end FixedUpdate()

    private void OnTriggerEnter(Collider other)
    {
        //Hit by a player weapon
        if (other.gameObject.tag.Equals("PlayerProjectile"))
        {
            //Debug.Log("Collision with " + other.gameObject.name + "!");

            //Take Damage
            hp -= other.gameObject.GetComponent<Weapons>().getDamage();

            GameObject sparkToSpawn = other.gameObject.GetComponent<Weapons>().getSpark();

            //Spawn the Spark
            GameObject theSpark = Instantiate(sparkToSpawn, other.transform.position, other.transform.rotation);
            theSpark.transform.Rotate(0, 180, 0);

            //Play hit effect
            GetComponent<AudioSource>().Play();

            //TODO: Damage Numbers?

            //Delete the laser
            Destroy(other.gameObject);
        }

        //Bumps into another Asteroid
        if (other.gameObject.tag.Equals("Asteroid"))
        {
            //Vector3 rotation by 180 on y
            moveDirection = Quaternion.Euler(0, 180, 0) * moveDirection;
        }
    }
}
