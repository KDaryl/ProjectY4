using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to destroy an object with this script attached to it after a certain amount of time.
/// The amount of time they have to live can be set at runtime or in the editor.
/// </summary>
public class DestroyTimer : MonoBehaviour {

    [SerializeField]
    public float timeToLive;

	
	// Update is called once per frame
	void Update () {

        timeToLive -= Time.deltaTime; //Minus dt from timer

        //If time ris up, delete the gameobejct this script is attached to
        if (timeToLive <= 0)
            Destroy(this.gameObject);
	}
}
