using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to play an audio source for the player casted spells
/// </summary>
public class SkullDropScript : MonoBehaviour {

    [SerializeField]
    AudioSource skullDrop;

	// Use this for initialization
	void Start () {
        //Sets the starting position of the audio source to begin at
        //30% in. So if the clip was 10 seconds long, this audio source would
        //begin playing at 3 seconds
        skullDrop.time = skullDrop.clip.length * .3f;
	}
}
