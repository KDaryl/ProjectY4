using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used for an item drop in the game. If the player kills an enemy, there is a chance
/// an item will drop (which will be an object with this script attached to it). This script will set the items properties
/// such as the Damage of the item, the Critical Chance of the item, the Critical Damage, the Mana Cost reduction etc..
/// If the player picks up this item, the items stats are added to the players overall stats. These stats are recalculated 
/// everytime a new item is picked up
/// </summary>
public class LootDrop : MonoBehaviour {

    [SerializeField]
    bool isIntialiser; //Bool to hold if it is a loot drop, or a loot initialiser

    //Sprites for the item
    [SerializeField]
    List<Sprite> weaponSprites;
    [SerializeField]
    List<Sprite> chestSprites;
    [SerializeField]
    List<Sprite> legsSprites;
    [SerializeField]
    List<Sprite> feetSprites;

    //List of the colours for the tier of the item (Blue, Yellow, Purple)
    [SerializeField]
    List<Color> tierColorList;

    //The values of the items gotten
    [SerializeField]
    public List<int> statValues;

    //The type of item loot drop is
    public Item.ItemType type;

    public int tier; //The tier of the item (Blue, Yellow, Purple), the higher, the more stats

    //The sprite number, this is the index of the sprite list that the item will have
    //so if it is 3, set the item sprite to be the third sprite in the corresponding list
    public int spriteNum; 

    //Inventory
    Inventory inventory;

    [SerializeField]
    SpriteRenderer mySprite;
    [SerializeField]
    SpriteRenderer backDrop;
    [SerializeField]
    GameObject description;
    [SerializeField]
    List<GameObject> affixes;
    [SerializeField]
    List<Sprite> typeSprites; //These 
    [SerializeField]
    SpriteRenderer typeS; //The item type sprite (weapon, chest, legs, feet)
    [SerializeField]
    SpriteRenderer itemIcon; //The Icon that is shown for the item that drops
    [SerializeField]
    List<Sprite> affixSprites; //The affix sprites, these are our rendered text sprites (Mana Cost, Damage, Critical Damage etc..)
    [SerializeField]
    Animator animator;
    [SerializeField]
    Color upgradeCol; //The colour of the stats if they are an upgrade (green)
    [SerializeField]
    Color downgradeCol;//The colour of the stats if they are a downgrade (red)
    [SerializeField]
    Color noChangeCol; //The colour of the stats if they arethe same (white)

    //Time to show description
    public float showLootTime;
    float descTimer;

    //Bool that shows if we are showing the description or not
    bool showDesc;

    // Use this for initialization
    void Start ()
    {
        //if it is an initialiser, then set the static sprites for the Item Class
        if (isIntialiser)
        {
            if (Item.spritesSet == false)
            {
                Item.weaponSprites = weaponSprites;
                Item.chestSprites = chestSprites;
                Item.legsSprites = legsSprites;
                Item.feetSprites = feetSprites;
                Item.spritesSet = true;
            }
        }
        else
        {
            //If set the item sprite based on what tyope of item it is that has dropped
            switch (type)
            {
                case Item.ItemType.WEAPON:
                    mySprite.sprite = weaponSprites[spriteNum];
                    break;
                case Item.ItemType.CHEST:
                    mySprite.sprite = chestSprites[spriteNum];
                    break;
                case Item.ItemType.LEGS:
                    mySprite.sprite = legsSprites[spriteNum];
                    break;
                case Item.ItemType.FEET:
                    mySprite.sprite = feetSprites[spriteNum];
                    break;
            }

            //If the tier of the item is 0, then the colour will be Blue, if it is 1 then yellow, and 2 is purple
            backDrop.color = tierColorList[tier];

            //Cache the inventory game object
            inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

            //The timer for showing the description of the item (right click the item)
            descTimer = showLootTime;

            //Set the light colour based on the tier of loot
            GetComponent<Light>().color = tierColorList[tier];
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Minus timer for showing the description of an item
        if(showDesc)
        {
            descTimer -= Time.deltaTime;

            if(descTimer <= 0)
            {
                HideDescription();
            }
        }
    }

    public void ShowDescription()
    {
        animator.Play("In", 1);
        //description.SetActive(true);
        descTimer = showLootTime; //Set the timer
        showDesc = true; //Set show description to true   
        
        //Set all the affixes boxes
        for(int i = 0; i < affixes.Count; i++)
            affixes[i].SetActive(false);


        int affixesAdded = 0;
        //Loop through all stat values
        for(int i = 0; i < statValues.Count; i++)
        {
            if(statValues[i] != 0 && affixesAdded < 4)
            {
                affixes[affixesAdded].SetActive(true);

                Text affixText = affixes[affixesAdded].transform.GetChild(0).GetComponent<Text>();

                //Get the the current equipped item the playe rhas and set the colour of the stat
                //depending on if it is better/worse/no-change
                for(int k = 0; k < inventory.items.Count; k++)
                {
                    if(inventory.items[k].GetComponent<Item>().type == type)
                    {
                        float val = inventory.items[k].GetComponent<Item>().affixValues[i];

                        if (statValues[i] > val)
                            affixText.color = upgradeCol;
                        else if (statValues[i] < val)
                            affixText.color = downgradeCol;
                        else
                            affixText.color = noChangeCol;

                        break; //Break out
                    }
                }



                affixes[affixesAdded].GetComponent<SpriteRenderer>().sprite = affixSprites[i];

                if (i == 5)
                    affixText.text = statValues[i].ToString();
                else if (i == 2)
                    affixText.text = "- %" + statValues[i].ToString();
                else
                    affixText.text = "+ %" + statValues[i].ToString();

                affixesAdded++;
            }
        }

        typeS.sprite = typeSprites[(int)type];

        switch (type)
        {
            case Item.ItemType.WEAPON:
                itemIcon.sprite = Item.weaponSprites[spriteNum];
                break;
            case Item.ItemType.CHEST:
                itemIcon.sprite = Item.chestSprites[spriteNum];
                break;
            case Item.ItemType.LEGS:
                itemIcon.sprite = Item.legsSprites[spriteNum];
                break;
            case Item.ItemType.FEET:
                itemIcon.sprite = Item.feetSprites[spriteNum];
                break;
        }
    }

    public void HideDescription()
    {
        animator.Play("Out", 1);
        showDesc = false;
    }

    public void PickupItem()
    {
        inventory.PickupItem(gameObject);

        //Update the players stats to reflect the new gear
        inventory.UpdatePlayerStats();
    }
}
