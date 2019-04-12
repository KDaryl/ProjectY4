using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Daryl Keogh
/// Description: Script used to spawn monsters in an environment by calculating the spawn chances of a monster
/// - and generating a number and deciding which one to spawn
/// </summary>
public class EnemySpawnerScript : MonoBehaviour {

    //These will be the spawn locations our monsters can spawn at
    [SerializeField]
    List<Transform> spawnLocs;

    //This will be our list of enemies the script can spawn, 
    //This list is populated in the editor for an ennvironment prefab
    [SerializeField]
    List<GameObject> enemies;

    //This will be our lsit of spawn chances, we make these integers
    //These will be our range, so we peroduce a random number from 0 to 100 and
    //If this number is in the range of the drop chance for that enemy, then spawn it
    [SerializeField]
    List<int> spawnChances;

    //Here we will get a value from 0 to 1 for each enemy spawn chance
    List<float> normalisedChances;

    // Use this for initialization
    void Awake () {

        //Check if the lists are setup correctly, if not output a message
        if(spawnChances.Count != enemies.Count)
        {
            Debug.Log("You've given more enemies than drop chances, have a look at the gameobject");
        }
        else if(spawnLocs.Count == 0)
        {
            Debug.Log("You've not assigned any locations for enemies to spawn");
        }
        //Else spawn the enemies
        else
        {
            normalisedChances = new List<float>();

            int sum = 0;
            for(int i = 0; i < enemies.Count; i++)
            {
                sum += spawnChances[i];
            }

            for(int i = 0; i < enemies.Count; i++)
            {
                normalisedChances.Add(spawnChances[i] / (float)sum);
            }

            //Loop through our spawn locations and spawn an enemy for each one
            for(int i = 0; i < spawnLocs.Count; i++)
            {
                float val = Random.value;
                bool spawned = false;

                for (int k = 0; k < enemies.Count; k++)
                {
                    if(val >= normalisedChances[k])
                    {
                        //Spawn an enemy
                        GameObject enemy = Instantiate(enemies[k], spawnLocs[i].position, enemies[k].transform.rotation);
                        //enemy.transform.position = spawnLocs[i].position;
                        GameLoop.enemiesAlive++;
                        spawned = true; //Set to true
                        break; //Break from loop as we have spawned
                    }
                }

                //If we didnt spawn a monster, choose one at random
                if(spawned == false)
                {
                    GameLoop.enemiesAlive++;
                    int rand = Random.Range(0, enemies.Count);
                    GameObject enemy = Instantiate(enemies[rand], spawnLocs[i].position, enemies[rand].transform.rotation);
                }
            }
        }
	}
}
