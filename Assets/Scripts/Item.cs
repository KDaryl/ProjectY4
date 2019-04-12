using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used for the players inventory items (Weapon, Chest, Leg and Feet). The item will have stats on it
/// such as Damage, Critical Hit Chance, Critical Damage, Mana cost reduction etc. The stats of this item are added to the players
/// overall stats and will be shown in the inventory (by pressing Tab in game)
/// </summary>
public class Item : MonoBehaviour {

    //Make this public so we can set item types
    public enum ItemType
    {
        WEAPON,
        CHEST,
        LEGS,
        FEET
    }

    [SerializeField]
    public int spriteNum;

    //our affixes, we will set them on/off if we have affixes for them
    [SerializeField]
    List<GameObject> affixesDescriptions;

    //Sprites for the item
    [SerializeField]
    List<Sprite> affixSprites;
    static public List<Sprite> weaponSprites;
    static public List<Sprite> chestSprites;
    static public List<Sprite> legsSprites;
    static public List<Sprite> feetSprites;
    static public bool spritesSet = false;

    //The title object
    [SerializeField]
    SpriteRenderer itemTitle;
    [SerializeField]
    SpriteRenderer itemIcon;

    //Serialize the field for the type of item
    [SerializeField]
    public ItemType type;

    [SerializeField]
    List<Sprite> typeSprites;

    [SerializeField]
    public List<string> affixNames;
    [SerializeField]
    public List<bool> hasAffix;
    [SerializeField]
    public List<int> affixValues;

    //The amount of affixes the item has
    public int affixAmount = 0;
    //Our key val pair of what affixes we have
    Dictionary<string, bool> affixPairs;

    //Booleans for what affixes we have
    bool hasCritChance { get; set; }
    bool hasCritDamage { get; set; }
    bool hasManaCost { get; set; }
    bool hasSpellDuration { get; set; }
    bool hasMovementSpeed { get; set; }

    // Use this for initialization
    void Awake () {
        affixAmount = 0;
        affixPairs = new Dictionary<string, bool>();

        for (int i = 0; i < 4; i++)
        {
            affixesDescriptions[i].SetActive(false);
        }

        //Populate our affix for the item
        for (int i = 0; i < affixNames.Count; i++)
        {
            affixPairs.Add(affixNames[i], hasAffix[i]);

            //If the item has less than 4 affixes, and the affix is true
            //Then initialise it
            if(hasAffix[i] && affixAmount < 4)
            {
                //Initialise the affix description
                affixesDescriptions[affixAmount].SetActive(true);


                //Set the item sprite based on what the affix is
                int index = affixNames.IndexOf(affixNames[i]); //Get the index of the name of the affix
                affixesDescriptions[affixAmount].GetComponent<SpriteRenderer>().sprite = affixSprites[index];

                //Get the text of the affix (so we can change it
                Text affixTxt = affixesDescriptions[affixAmount].transform.GetChild(0).GetComponent<Text>();

                //Setting the text of the affix
                if(affixNames[i] == "Damage")
                {
                    affixTxt.text = affixValues[index].ToString();
                }
                else if(affixNames[i] == "Mana Cost")
                {
                    affixTxt.text = "- %" + affixValues[index].ToString();
                }
                else
                {
                    affixTxt.text = "+ %" + affixValues[index].ToString();
                }

                //Increase amount last
                affixAmount++;
            }
        }

        //Set booleans
        hasCritChance = hasAffix[0];
        hasCritDamage = hasAffix[1];
        hasManaCost = hasAffix[2];
        hasSpellDuration = hasAffix[3];
        hasMovementSpeed = hasAffix[4];

        //Set item tile sprite name using the type enum
        itemTitle.sprite = typeSprites[(int)type];
        
        switch(type)
        {
            case ItemType.WEAPON:
                itemIcon.sprite = weaponSprites[spriteNum];
                break;
            case ItemType.CHEST:
                itemIcon.sprite = chestSprites[spriteNum];
                break;
            case ItemType.LEGS:
                itemIcon.sprite = legsSprites[spriteNum];
                break;
            case ItemType.FEET:
                itemIcon.sprite = feetSprites[spriteNum];
                break;
        }

        //Update player stats
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>().UpdatePlayerStats();
    }

    public void UpdateSelf()
    {
        affixAmount = 0;

        for (int i = 0; i < 4; i++)
        {
            affixesDescriptions[i].SetActive(false);
        }

        for (int i = 0; i < affixNames.Count; i++)
        {
            //If the item has less than 4 affixes, and the affix is true
            //Then initialise it
            if (hasAffix[i] && affixAmount < 4)
            {
                //Initialise the affix description
                affixesDescriptions[affixAmount].SetActive(true);

                //Set the item sprite based on what the affix is
                int index = affixNames.IndexOf(affixNames[i]); //Get the index of the name of the affix
                affixesDescriptions[affixAmount].GetComponent<SpriteRenderer>().sprite = affixSprites[index];

                //Get the text of the affix (so we can change it
                Text affixTxt = affixesDescriptions[affixAmount].transform.GetChild(0).GetComponent<Text>();

                //Setting the text of the affix
                if (affixNames[i] == "Damage")
                {
                    affixTxt.text = affixValues[index].ToString();
                }
                else if (affixNames[i] == "Mana Cost")
                {
                    affixTxt.text = "- %" + affixValues[index].ToString();
                }
                else
                {
                    affixTxt.text = "+ %" + affixValues[index].ToString();
                }

                //Increase amount last
                affixAmount++;
            }
        }

        hasCritChance = hasAffix[0];
        hasCritDamage = hasAffix[1];
        hasManaCost = hasAffix[2];
        hasSpellDuration = hasAffix[3];
        hasMovementSpeed = hasAffix[4];

        //Set the sprite
        switch (type)
        {
            case ItemType.WEAPON:
                itemIcon.sprite = weaponSprites[spriteNum];
                break;
            case ItemType.CHEST:
                itemIcon.sprite = chestSprites[spriteNum];
                break;
            case ItemType.LEGS:
                itemIcon.sprite = legsSprites[spriteNum];
                break;
            case ItemType.FEET:
                itemIcon.sprite = feetSprites[spriteNum];
                break;
        }
    }

    public void UpdateItem(GameObject item)
    {
        LootDrop newItem = item.GetComponent<LootDrop>();
        affixAmount = 0;
        spriteNum = newItem.spriteNum;

        //reset our affix values and booleans
        for (int i = 0; i < hasAffix.Count; i++)
        {
            //If the new item has the affix
            if(newItem.statValues[i] != 0)
            {
                //Set the new value
                affixValues[i] = newItem.statValues[i];
                hasAffix[i] = true;
            }
            //Else if it doesnt reset the affix
            else
            {
                //Reset the value
                affixValues[i] = 0;
                hasAffix[i] = false;
            }
        }

        for (int i = 0; i < 4; i++)
            affixesDescriptions[i].SetActive(false);

        for (int i = 0; i < affixNames.Count; i++)
        {
            //If the item has less than 4 affixes, and the affix is true
            //Then initialise it
            if (hasAffix[i] && affixAmount < 4)
            {
                //Initialise the affix description
                affixesDescriptions[affixAmount].SetActive(true);

                //Set the item sprite based on what the affix is
                int index = affixNames.IndexOf(affixNames[i]); //Get the index of the name of the affix
                affixesDescriptions[affixAmount].GetComponent<SpriteRenderer>().sprite = affixSprites[index];

                //Get the text of the affix (so we can change it
                Text affixTxt = affixesDescriptions[affixAmount].transform.GetChild(0).GetComponent<Text>();

                //Setting the text of the affix
                if (affixNames[i] == "Damage")
                {
                    affixTxt.text = affixValues[index].ToString();
                }
                else if (affixNames[i] == "Mana Cost")
                {
                    affixTxt.text = "- %" + affixValues[index].ToString();
                }
                else
                {
                    affixTxt.text = "+ %" + affixValues[index].ToString();
                }

                //Increase amount last
                affixAmount++;
            }
        }

        hasCritChance = hasAffix[0];
        hasCritDamage = hasAffix[1];
        hasManaCost = hasAffix[2];
        hasSpellDuration = hasAffix[3];
        hasMovementSpeed = hasAffix[4];

        //Set the sprite
        switch (type)
        {
            case ItemType.WEAPON:
                itemIcon.sprite = weaponSprites[spriteNum];
                break;
            case ItemType.CHEST:
                itemIcon.sprite = chestSprites[spriteNum];
                break;
            case ItemType.LEGS:
                itemIcon.sprite = legsSprites[spriteNum];
                break;
            case ItemType.FEET:
                itemIcon.sprite = feetSprites[spriteNum];
                break;
        }
    }
}
