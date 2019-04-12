using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used when a player casts the skulld rop spell (spell number 1).
/// It will reset each skull that hits the ground and spawns an audio object to play the sound
/// of the skull falling from the sky
/// </summary>
public class SkullDrop : MonoBehaviour {
    [SerializeField]
    GameObject audioObject;
    [SerializeField]
    List<GameObject> skulls;

    int skullsDropped;

    private void Start()
    { 
        skullsDropped = -1; //No skulls have dropped yet
    }

    public void SpawnAudio()
    {
        skullsDropped++;

        //If we have dropped ht emax amount of skulls, reset our integer to 0
        if (skullsDropped == 10)
            skullsDropped = 0;

        //Reset our skull
        skulls[skullsDropped].GetComponent<Collider>().enabled = true;


        Instantiate(audioObject, transform.position, audioObject.transform.rotation);
    }
}
