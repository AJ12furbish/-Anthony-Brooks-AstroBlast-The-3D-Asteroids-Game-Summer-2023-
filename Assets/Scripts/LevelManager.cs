using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //Instance Variables
    //Prefabs
    public GameObject smallAsteroid;
    public GameObject bigAsteroid;
    public GameObject player;

    //UI Links
    public Text scoreText;
    public Text waveText;
    public Text timerText;
    public Text statusText;

    //Game Stats
    public int wave = 0;
    public int score = 0;
    public int lives = 2;
    private Quaternion lastPlayerRotation;
    private bool gameOver = false;

    //Wave Variables
    private bool waveLoading; //We use this to delay spawning a new wave
    private float wavePrepTimer;
    private int waveEnemiesLeft = 0;
    private float waveTimeLeft = 0;
    //Spawning Wave Arrays
    public int[] waveData_bigAsteroids = { 2, 3, 2, 4, 7 };
    public int[] waveData_smallAsteroids = { 0, 1, 10, 1, 0 };
    public float[] waveData_spawnDistance = { 0.2f, 0.2f, 0.25f, 0.3f, 0.35f };
    public float[] waveData_timeLimit = { 60f, 60f, 90f, 70f, 80f };

    private int DEBUGMODE = 0;

    // Start is called before the first frame update
    void Start()
    {
        wave = 0;
        setWaveText(wave);
        wavePrepTimer = 0.5f;
        waveLoading = true;

        //Link UI Elements
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        waveText = GameObject.Find("WaveText").GetComponent<Text>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        statusText = GameObject.Find("StatusText").GetComponent<Text>();
        statusText.enabled = false;
    }

    public void setLastPlayerRotation(Quaternion old_rotation)
    {
        lastPlayerRotation = old_rotation;
    }

    //Respawn Player
    public void spawnPlayer()
    {
        if (lives > 0)
        {
            //If there are lives left, spawn the player
            //Check for any asteroids in position 0,0,0
            Collider[] things = Physics.OverlapSphere(new Vector3(0, 0, 0), 4);
            foreach (Collider item in things)
            {
                //Move asteroids
                if (item.gameObject.tag.Equals("Asteroid"))
                {
                    string[] badTags = new string[] { "Player", "Asteroid" };
                    Vector3 newPosition = Boundary.getRandomPosition(0.4f, badTags, 1.2f);
                    item.gameObject.transform.position = newPosition;
                }

                //if you have any enemy lasers or such, delete them
                //  (Future stuff)
            } //end loop move asteroids

            if (lastPlayerRotation == null)
                lastPlayerRotation = Quaternion.identity;

            Instantiate(player, new Vector3(0, 0, 0), lastPlayerRotation);
            changeLives(-1);

        }
        else
        {
            //No lives left, game over
            gameOver = true;
            statusText.enabled = true;


        }
    } //end spawnPlayer

    // UI and Variable updating
    //
    public void changeScore(int inScore)
    {
        score += inScore;
        if (scoreText != null)
        {
            scoreText.text = "Lives: " + lives;
            scoreText.text += "\nScore: " + score;
        }
    }

    public void changeLives(int inLives)
    {
        lives += inLives;
        if (scoreText != null)
        {
            scoreText.text = "Lives: " + lives;
            scoreText.text += "\nScore: " + score;
        }
    }

    public void setWaveText(int inWave)
    {

        if (waveText != null)
        {
            waveText.text = "Wave: " + inWave;
        }
    }

    public void setWaveTimeText()
    {
        if (waveTimeLeft >= 10)
        {
            timerText.color = new Color(255f / 255f, 255f / 255f, 0 / 255f);
            timerText.text = "Time: " + Mathf.Ceil(waveTimeLeft);
        }
        else
        {
            timerText.color = new Color(255f / 255f, 150f / 255f, 0f / 255f);
            timerText.text = "Time: " + waveTimeLeft.ToString("#0.00");
            //timerText.text = string.Format("Time: {0:#0.00}", waveTimeLeft);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Wave Management

        //Decrease the timer that waits to spawn a wave
        if (wavePrepTimer > 0)
        {
            wavePrepTimer -= Time.deltaTime;
        }
        if (waveTimeLeft > 0)
        {
            waveTimeLeft -= Time.deltaTime;
            setWaveTimeText();

            if (DEBUGMODE >= 4)
                Debug.Log("Wave Time: " + waveTimeLeft);
        }


        //Detect if we are ready to say "you beat this wave" and start preparing for the next wave
        //END OF WAVE: DEFEATED ALL ENEMIES
        if (waveEnemiesLeft <= 0 && waveLoading == false && gameOver == false)
        {
            if (DEBUGMODE >= 1)
                Debug.Log("Cleared Wave: " + wave);
            waveLoading = true;
            wavePrepTimer = 3.0f; //3 second before next wave appears
            //Clear out the enemy count and time left
            waveTimeLeft = 0;
            waveEnemiesLeft = 0;
        }
        //END OF WAVE: TIME'S UP
        if (waveTimeLeft <= 0 && waveLoading == false && gameOver == false)
        {
            if (DEBUGMODE >= 1)
                Debug.Log("Failed Wave: " + wave);
            waveLoading = true;
            wavePrepTimer = 1.0f; //3 second before next wave appears
        }


        //Actually spawn the wave
        if (wavePrepTimer <= 0 && waveLoading == true && gameOver == false)
        {
            if (waveEnemiesLeft <= 0)//You "won" the wave by defeating the enemies
            {
                //Reset the Player to the middle
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                playerObject.transform.position = new Vector3(0, 0, 0);
            }

            wave++;
            setWaveText(wave);
            spawnWave(wave);
        }


        if ((Input.GetKeyDown("enter") || Input.GetKeyDown("return")) && gameOver == true)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        //Close the game
        if (Input.GetKeyDown("escape"))
            Application.Quit();

        //
        // DEBUG -> SPAWN AN ASTEROID
        //
        if (Input.GetKeyDown("r") && DEBUGMODE >= 2)
        {
            spawnObject(bigAsteroid, 1.2f, 0.2f, "Asteroid", "Player", "PlayerProjectile");
        }
    } //end Update()

    void spawnWave(int wave)
    {
        if (DEBUGMODE >= 1)
            Debug.Log("Spawning Wave: " + wave);

        int numBigAsteroidsToSpawn = 0;
        int numSmallAsteroidsToSpawn = 0;
        float edgeDistance = 0.0f;

        //To help convert the wave number into an array index (arrays start at 0, but waves start at 1)
        int waveIndex = wave - 1;

        //Load the wave data from the arrays
        if (waveIndex < waveData_bigAsteroids.Length)
        {
            //I have data already in the arrays for this wave
            numBigAsteroidsToSpawn = waveData_bigAsteroids[waveIndex];
            numSmallAsteroidsToSpawn = waveData_smallAsteroids[waveIndex];
            edgeDistance = waveData_spawnDistance[waveIndex];
            waveTimeLeft = waveData_timeLimit[waveIndex];
        }
        else
        {
            //I'm past the limits of my array... let's just generate values
            numBigAsteroidsToSpawn = 2 + wave / 2;
            numSmallAsteroidsToSpawn = 1 + (wave % 3);
            edgeDistance = 0.4f; //40% from the edge
            waveTimeLeft = 50f + wave * 10f;
        }

        //Actually spawn everything
        for (int i = 0; i < numBigAsteroidsToSpawn; i++)
        {
            spawnObject(bigAsteroid, 1.2f, edgeDistance);
        }
        for (int i = 0; i < numSmallAsteroidsToSpawn; i++)
        {
            spawnObject(smallAsteroid, 0.6f, edgeDistance);
        }

        //We have finished loading the wave
        waveLoading = false;
        if (DEBUGMODE >= 1)
            Debug.Log("Wave " + wave + " has spawned!");

    } //end spawnWave

    void spawnObject(GameObject thingToSpawn, float clearRadius, float edgeDistance, params string[] badTags)
    {
        //Restore badTags to the default 2 tags if it's passed as empty
        if (badTags == null || badTags.Length == 0)
            badTags = new string[] { "Player", "Asteroid" };

        if (DEBUGMODE >= 3)
        {
            string tags = "";
            for (int i = 0; i < badTags.Length; i++)
                tags += badTags[i] + " ";
            Debug.Log("Bad Tags: " + tags);
        }

        Vector3 position = Boundary.getRandomPosition(edgeDistance, badTags, clearRadius);
        Instantiate(thingToSpawn, position, Quaternion.identity);
    }

    //Method to change the enemy count
    public void changeEnemyCount(int amt)
    {
        waveEnemiesLeft += amt;
        if (DEBUGMODE >= 1)
            Debug.Log("Enemies: " + waveEnemiesLeft);
    }

    //DEBUGING METHOD
    private void OnDrawGizmos()
    {
        /*
        //Draw a green sphere
        Gizmos.color = Color.green;
        for (int i = 0; i < 100; i++)
        {
            string[] badTags = { "Player", "Asteroid" };

            //Get a random Position that's "safe"
            Vector3 randomPosition = Boundary.getRandomPosition(0.2f, badTags, 1.2f);

            //Draw the Gizmos
            Gizmos.DrawWireSphere(randomPosition, 0.5f);
        }
        */
    }
}
