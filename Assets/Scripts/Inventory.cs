using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: The inventory is used to display all of the items the player currently has equipped along with their
/// stats. The inventory can be opened using Tab
/// </summary>
public class Inventory : MonoBehaviour {

    [SerializeField]
    public List<GameObject> items;

    [SerializeField]
    List<Text> statTexts;

    [SerializeField]
    GameObject player;

    //Hold a reference to the player script
    //This is so we can change the values of the player
    //Movement speed, sword damage etc
    Player playerScript;

    [SerializeField]
    GameObject inventoryBook;

	// Use this for initialization
	void Start () {
        //Ignore collisions between loot and players/enemies
        Physics.IgnoreLayerCollision(4, 0, true);

        playerScript = player.GetComponent<Player>();

        //Making our items persist through scenes so the player
        //Keep shis stats and items
        if(PersitantScript.itemsSet == false)
        {
            for (int i = 0; i < items.Count; i++)
            {
                PersitantScript.persistantItems.Add(new Item());
                Item original = items[i].GetComponent<Item>();
                Item newItem = PersitantScript.persistantItems[i];

                //Set the values of the new item
                newItem.affixAmount = original.affixAmount;
                newItem.affixNames = original.affixNames;
                newItem.affixValues = original.affixValues;
                newItem.hasAffix = original.hasAffix;
                newItem.spriteNum = original.spriteNum;
                newItem.type = original.type;
            }           

            PersitantScript.itemsSet = true;
        }
        else
        {
            for (int i = 0; i < PersitantScript.persistantItems.Count; i++)
            {
                Item item = items[i].GetComponent<Item>();
                Item newItem = PersitantScript.persistantItems[i];

                //Set the values of the item from our saved item
                item.affixAmount = newItem.affixAmount;
                item.affixNames = newItem.affixNames;
                item.affixValues = newItem.affixValues;
                item.hasAffix = newItem.hasAffix;
                item.spriteNum = newItem.spriteNum;
                item.type = newItem.type;
                item.UpdateSelf();
            }

        }

        //Update player stats
        UpdatePlayerStats();
    }

    public void UpdateInventory()
    {
        PersitantScript.persistantItems.Clear();

        for (int i = 0; i < items.Count; i++)
            PersitantScript.persistantItems.Add(items[i].GetComponent<Item>());
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Tab) && GameLoop.dungeonOver == false)
        {
            inventoryBook.SetActive(!inventoryBook.active);
        }
        //If the dungeon ends and the inventory is opened, close it
        else if (GameLoop.dungeonOver && inventoryBook.active)
        {
            inventoryBook.SetActive(false);
        }
	}

    //Method used to update one of the items
    public void PickupItem(GameObject item)
    {
        GameObject itemToUpdate = null;

        //Find oput which item we have to update due to our 
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].GetComponent<Item>().type == item.GetComponent<LootDrop>().type)
            {
                itemToUpdate = items[i];
                break;
            }
        }

        //Update the item in the inventory
        itemToUpdate.GetComponent<Item>().UpdateItem(item);

        //At the end destroy the item
        Destroy(item);

        //Update the inventory to store the new item
        UpdateInventory();
    }
    
    public void UpdatePlayerStats()
    {
        float CC = 0;
        float CD = 0;
        float MC = 0;
        float SP = 0;
        float MS = 0;
        float DMG = 0;

        //Loop through inventory items and apply the affixes
        for(int i = 0; i < items.Count; i++)
        {
            Item item = items[i].GetComponent<Item>();

            //Add the values of the item to our temporary variables
            //Make sure to clamp to min and max values
            CC = Mathf.Clamp(CC + item.affixValues[0], 0, 100);
            CD = Mathf.Clamp(CD + item.affixValues[1], 0, Mathf.Infinity);
            MC = Mathf.Clamp(MC + item.affixValues[2], 0, 100);
            SP = Mathf.Clamp(SP + item.affixValues[3], 0, 300);
            MS = Mathf.Clamp(MS + item.affixValues[4], 0, 100);
            DMG = Mathf.Clamp(DMG + item.affixValues[5], 0, Mathf.Infinity);
        }

        //Update all stats text
        for (int i = 0; i < statTexts.Count; i++)
        {
            if (i == 0)
                statTexts[i].text = "%" + CC.ToString();
            else if(i == 1)
                statTexts[i].text = "%" + CD.ToString();
            else if(i == 2)
                statTexts[i].text = "- %" + MC.ToString();
            else if (i == 3)
                statTexts[i].text = "%" + SP.ToString();
            else if(i == 4)
                statTexts[i].text = "%" + MS.ToString();
            else
                statTexts[i].text = DMG.ToString();
        }

        MS = 1 + (MS / 100.0f); //Get MS percentage
        MC = 1 - (MC / 100.0f); //Get MC percentage
        SP = 1 + (SP / 100.0f); //Get SP percentage
        CD = 1 + (CD / 100.0f);

        //Debug.Log("Player Stats " + CC.ToString() + " " + CD.ToString() + " " + MC.ToString() + " " + SP.ToString() + " " + MS.ToString() + " " + DMG.ToString());

        //Setting the attributes of the player
        playerScript.movementSpeed = MS;
        playerScript.manaCost = MC;
        playerScript.spellDuration = SP;
        playerScript.playerCritChance = CC;
        playerScript.playerCritDamage = CD;
        playerScript.swordDamage = DMG;
        playerScript.spellDamage = (int)(DMG * .75f); //Spells do 75% of weapon damage
        playerScript.companionDamage = (int)(DMG * .35f); //Ai companion does 35% of weapon damage
    }
}
