using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to show the dungeon parchment in the camp. When the player
/// enters the portal, it will play the animation setup to show the player the current dungeon
/// they are on and allow them to start the dungeon or close the parchment.
/// </summary>
public class DungeonTeleporter : MonoBehaviour {

    [SerializeField]
    CampLevelHandler handler;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player Pickup Radius"))
        {
            handler.StartAnimation();
        }
    }
}
