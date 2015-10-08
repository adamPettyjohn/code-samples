using UnityEngine;
using System.Collections;

//This spawns the players. There should be one of these
//in each scene. Requires one spawn point to be present
//in order to function correctly.

public class SpawnMaster : MonoBehaviour {
    //The spawn points
    private GameObject[] spawnPoints;

    //Attemps to spawn the passed in gameobject
    public void AttemptSpawn(GameObject player)
    {
        //Finds the other players
        GameObject[] ninjas = GameObject.FindGameObjectsWithTag("Ninja");

        //Sets the first spawn point to the default spawn
        GameObject currentSpawn = spawnPoints[0];
        float currentScore = 0f;

        //Iterates through the possible spawns. Uses an algorithm to check the
        //safest spawn. (currently just looks for the max distance away from enemies)
        foreach(GameObject possibleSpawn in spawnPoints)
        {
            float possibleScore = 0f;
            
            foreach(GameObject ninja in ninjas)
            {
                if (ninja.GetComponent<Health>() == null)
                {
                    possibleScore += 0f;
                }
                else
                {
                    //Make sure the player being looked at is not on the same team
                    if (ninja.GetComponent<Health>().currentTeam != player.GetComponent<Health>().currentTeam)
                    {
                        float addScore = Vector2.Distance(ninja.transform.position, possibleSpawn.transform.position);
                        possibleScore += addScore;
                    }
                }
            }

            if (possibleScore > currentScore)
            {
                currentSpawn = possibleSpawn;
                currentScore = possibleScore;
            }

        }

        player.GetComponent<Rigidbody2D>().position = currentSpawn.transform.position; //first attempt at fix

    }

	// Before anything happens, we look for the spawn points
	void Awake () {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
	}
}
