using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Daryl Keogh
/// Description: Script used to spawn the dungeon. This will be done randomly to keep the game feeling fresh
/// and to avoid the player seeing the same environments each time
/// </summary>
public class DungeonRandomiser : MonoBehaviour {

    [SerializeField]
    int maxTiles;
    [SerializeField]
    List<GameObject> environments;
    [SerializeField]
    GameObject sideCollider;
    [SerializeField]
    GameObject frontCollider;

    Vector3 lastSpawnLoc; //The last location we spawned an environment in
    public static int tilesSpawned; //The max amount of tiles that can spawn
    int minTiles; //The minimum amount of tiles that can spawn
    List<Vector3> lastSpawns;

    //Due to how the level is created, we must spawn our 
    //dungeon during the awake step of the scene to ensure 
    //everything is initialised correctly
    void Awake ()
    {
        var nextLocations = new List<Vector3> { new Vector3(0, 0, 7), new Vector3(4, 0, 0), new Vector3(0, 0, -7) };
        lastSpawns = new List<Vector3>();
        tilesSpawned = 1;
        lastSpawnLoc = new Vector3(0, .5f, 0);
        lastSpawns.Add(lastSpawnLoc);
        minTiles = Mathf.Clamp(1 + (DungeonLevel.level / 2), 0, maxTiles); //Get the min amount of tiles we must make
        int tileToSpawn = Random.Range(0, environments.Count); //Generate random number between 0 and the amount of environments

        //Create the first tile
        Instantiate(environments[tileToSpawn], lastSpawnLoc, environments[tileToSpawn].transform.rotation, transform);

        //Loop for min tiles to spawn all of our tiles
        for(int i = 0; i < minTiles && tilesSpawned < minTiles; i++)
        {
            bool spawned = false;

            while (spawned == false)
            {
                //get a random number between 0 and 3, this will be the position where the next item spawns
                int rand = Random.Range(0, 3);

                //If a tile is already at that position, skip and try again
                if (lastSpawns.Contains(lastSpawnLoc + nextLocations[rand]))
                    continue;

                //Spawn the item at the new location
                lastSpawnLoc += nextLocations[rand];
                lastSpawns.Add(lastSpawnLoc);
                tilesSpawned++;
                Instantiate(environments[tileToSpawn], lastSpawnLoc, environments[tileToSpawn].transform.rotation, transform);

                spawned = true;
            }
        }

        //Loop through the positions we spawned items at and spawn our world colliders
        for(int i = 0; i < tilesSpawned; i++)
        {
            //Spawn our edge colliders if there is no tiles around 
            if(lastSpawns.Contains(lastSpawns[i] + nextLocations[0]) == false) //If no tile to the left
            {
                Instantiate(sideCollider, lastSpawns[i] + new Vector3(0,.5f, 4), sideCollider.transform.rotation, transform);
            }
            if (lastSpawns.Contains(lastSpawns[i] + nextLocations[2]) == false) //If no tile to the right
            {
                Instantiate(sideCollider, lastSpawns[i] + new Vector3(0, .5f, -4), sideCollider.transform.rotation, transform);
            }
            if (lastSpawns.Contains(lastSpawns[i] + nextLocations[1]) == false) //If no tile infront
            {
                Instantiate(frontCollider, lastSpawns[i] + new Vector3(2.5f, .5f, 0), frontCollider.transform.rotation, transform);
            }
            if (lastSpawns.Contains(lastSpawns[i] - nextLocations[1]) == false) //If no tile behind
            {
                Instantiate(frontCollider, lastSpawns[i] + new Vector3(-2.5f, .5f, 0), frontCollider.transform.rotation, transform);
            }
        }
    }
}
