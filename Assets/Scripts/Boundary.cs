using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Boundary
{
    public static void checkBoundary(GameObject thing)
    {
        //DEBUG - Print Object's relative position on the screen
        int DEBUG = 0;
        if (DEBUG == 1)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(thing.transform.position);
            Debug.Log("(" + pos.x + ", " + pos.y + ", " + pos.z + ")");
        }

        //Dynamically calculate the edges of the screen
        Vector3 origin = new Vector3(0, 0, 0);
        Vector3 topRight = new Vector3(1.0f, 1.0f, 0);

        float xMin = Camera.main.ViewportToWorldPoint(origin).x;
        float xMax = Camera.main.ViewportToWorldPoint(topRight).x;
        float zMin = Camera.main.ViewportToWorldPoint(origin).z;
        float zMax = Camera.main.ViewportToWorldPoint(topRight).z;

        float xWidth = xMax - xMin;
        float zHeight = zMax - zMin;
        if (DEBUG == 2)
            Debug.Log("xMin: " + xMin + " xMax: " + xMax + " zMin: " + zMin + " zMax: " + zMax);

        //Get the "thing"'s location
        float x = thing.transform.position.x;
        float y = thing.transform.position.y;
        float z = thing.transform.position.z;

        //Check for Edge of Screen (and then Wrap to other side)
        //Exit on Left
        if (x <= xMin)
            thing.transform.position += new Vector3(xWidth, 0f, 0f);

        //Exit on Right
        if (x >= xMax)
            thing.transform.position -= new Vector3(xWidth, 0f, 0f);

        //Exit on Bottom
        if (z <= zMin)
            thing.transform.position += new Vector3(0f, 0f, zHeight);

        //Exit on Top
        if (z >= zMax)
            thing.transform.position -= new Vector3(0f, 0f, zHeight);

    } //end checkBoundary()

    //********
    //Calculate Edge of Screen Position
    //********
    public static float getXMin()
    {
        float xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).x;
        return xMin;
    }
    public static float getXMax()
    {
        float xMax = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f)).x;
        return xMax;
    }
    public static float getZMin()
    {
        float zMin = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).z;
        return zMin;
    }
    public static float getZMax()
    {
        float zMax = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f)).z;
        return zMax;
    }

    //********
    //Detect Edge of Screen
    //********
    public static bool IsOffLeftEdge(Vector3 position, float amount)
    {
        if (position.x < getXMin() - amount)
            return true;
        else
            return false;
    }

    public static bool IsOffRightEdge(Vector3 position, float amount)
    {
        if (position.x > getXMax() + amount)
            return true;
        else
            return false;
    }

    public static bool IsOffBottomEdge(Vector3 position, float amount)
    {
        if (position.z < getZMin() - amount)
            return true;
        else
            return false;
    }

    public static bool IsOffTopEdge(Vector3 position, float amount)
    {
        if (position.z > getZMax() + amount)
            return true;
        else
            return false;
    }

    public static Vector3 getRandomPosition(float percentFromEdge)
    {
        //pick a random edge
        int edge = Random.Range(0, 4); //Picks 0 - 3
        float randX = 0.0f;
        float randZ = 0.0f;

        if (edge == 0) //top edge
        {
            randX = Random.Range(getXMin(), getXMax());
            randZ = getZMax() - Random.Range(0, percentFromEdge * getZMax());
        }
        else if (edge == 1) //bottom edge
        {
            randX = Random.Range(getXMin(), getXMax());
            randZ = getZMin() - Random.Range(0, percentFromEdge * getZMin());
        }
        else if (edge == 2) //right edge
        {
            randX = getXMax() - Random.Range(0, percentFromEdge * getXMax());
            randZ = Random.Range(getZMin(), getZMax());
        }
        else if (edge == 3) //left edge
        {
            randX = getXMin() - Random.Range(0, percentFromEdge * getXMin());
            randZ = Random.Range(getZMin(), getZMax());
        }

        //Vector3 spawnPosition = new Vector3 (randX, 0, randZ);

        return new Vector3(randX, 0, randZ);
    } //end getRandomPosition()

    public static Vector3 getRandomPosition(float percentFromEdge, string[] tags, float clearRadius)
    {
        bool good = false;
        int attempts = 0;
        Vector3 spawnPosition = getRandomPosition(percentFromEdge);

        while (good == false) //While we don't have a good location
        {
            //Get a random position
            spawnPosition = getRandomPosition(percentFromEdge);

            //Get a list of things within the clearRadius of that spawnPosition
            Collider[] things = Physics.OverlapSphere(spawnPosition, clearRadius);

            //Assume it's a good position
            good = true;

            //Loop through all of the physics things
            foreach (Collider item in things)
            {
                //Check each of the tags in the string array
                foreach (string badTag in tags)
                {
                    if (item.gameObject.tag.Equals(badTag))
                        good = false;
                }
            } //end foreach outer

            if (good == false)
            {
                attempts++;
                if (attempts > 20) //if we tried 20 positions in this "random donut rectangle"
                    percentFromEdge += 0.05f;  //Increase our search size
            }

        }//end while good == false

        //We're outside the loop, that means the location is good
        return spawnPosition;

    }//end get Safe Location

}//end class Boundary
