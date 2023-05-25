using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkCleanup : MonoBehaviour
{
    public float existDuration = 1.2f; //Stay alive for x seconds
    private float existTimer = 0.0f;


    // Update is called once per frame
    void Update()
    {
        existTimer += Time.deltaTime;
        if (existTimer > existDuration)
            Destroy(gameObject);
    }
}
