using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to make sure our items picked up throughout the game are not destroyed when switching between scenes.
/// So if the player goes into a dungeon and finds a weapon that has much better stats than they currently hacve and piccks it up, when 
/// the scene changes, they will still have that weapon equipped.
/// </summary>
public class PersitantScript : MonoBehaviour {

    //Boolean to know if our items have been set or not
    //this will be change din our inventory
    public static bool itemsSet = false;

    //We need to hold our copy of our items
    public static List<Item> persistantItems = new List<Item>();

	// Use this for initialization
	void Awake () {
        //Doesnt destroy the object that this script is attached to
        DontDestroyOnLoad(this.gameObject);
	}
}
