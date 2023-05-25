using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBig : MonoBehaviour
{
    //Instance Variables
    //Rotation Variables
    public float tumbleMin = 1.5f;
    public float tumbleMax = 3.5f;

    //Movement Variables
    public float moveSpeedMin = 1.0f;
    public float moveSpeedMax = 3.0f;
    private Vector3 moveDirection; //Randomly generation direction of motion
    private float speed; //The actual randomly generated speed

    //Components
    private Rigidbody body;

    //Gameplay Variables
    public GameObject explosion;
    public GameObject smallAsteroidPrefab;
    public int numSmallAsteroids = 3;
    public int maxHP = 40;
    private int hp;
    public int scoreValue = 100;

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

            //Store the location to spawn little asteroids at
            Vector3 loc = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            //Destroy the Asteroid
            Destroy(gameObject);

            //Spawn small asteroid
            for (int i = 0; i < numSmallAsteroids; i++)
            {
                spawnSmallAsteroid(loc, 0.4f);
            }

            //Update the number of enemies and score
            manager.changeScore(scoreValue);
            manager.changeEnemyCount(-1);
        }

    } //end Update()

    public void spawnSmallAsteroid(Vector3 spawnLocationCenter, float spawnRadius)
    {
        //Assume the position is bad, until we see it's good
        bool goodPosition = false;
        int attempts = 0;

        Vector3 spawnOffsets = new Vector3();
        //float colliderRadius = smallAsteroidPrefab.GetComponent<CapsuleCollider>().height * .9f;

        while (goodPosition == false)
        {
            //Pick a random position within spawnRadius
            float randX = Random.Range(-spawnRadius, spawnRadius);
            float randZ = Random.Range(-spawnRadius, spawnRadius);

            spawnOffsets = new Vector3(randX, 0, randZ);

            Collider[] things = Physics.OverlapSphere(spawnLocationCenter + spawnOffsets, .5f); //Used to be colliderRadius

            //Assume the position is good, but check for bad
            goodPosition = true;

            //Go through the array looking for anything that this small Asteroid cannot overlap with
            foreach (Collider item in things)
            {
                if (item.gameObject.tag.Equals("Asteroid") || item.gameObject.tag.Equals("Player"))
                {
                    goodPosition = false;
                }
            }

            attempts++;
            if (attempts >= 10)
            {
                spawnRadius += 0.2f;
                attempts = 0;
            }
            if (spawnRadius >= 5.0f)
            {
                goodPosition = false;
                break;
            }
        } //end while()

        //Spawn if it's a good position and we didn't give up in the loop
        if (goodPosition)
            Instantiate(smallAsteroidPrefab, spawnLocationCenter + spawnOffsets, Quaternion.identity);

    } //end small Asteroid Spawn

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
