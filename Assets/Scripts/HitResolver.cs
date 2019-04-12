using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: The HitLootResolver handles hits by both enemies and players. If an enemy dies during resolving a hit,
/// this class will then generate a random number, if that number is less than or equal to the item drop chance (set in the editor)
/// then this class will spawn a LootDrop at the enemies location with random stats (please loook at DropLoot method below)
/// </summary>
public class HitResolver : MonoBehaviour {

    [SerializeField]
    GameObject lootDrop; //The item that is spawned if loot is dropped on death of an enemy

    //Publics
    public GameObject player;
    public GameObject damageIndicator;

    //Privates
    Player playerScript;
    float enemyCritChance, enemyCritDamage;

    [SerializeField]
    float lootDropChance; //The chance for loot to drop

    [SerializeField]
    Color critColor, nonCritColor, enemyNonCritColor;

    [SerializeField]
    List<string> affixNames;
    [SerializeField]
    IntIntDictionary lowTierValues;
    [SerializeField]
    IntIntDictionary midTierValues;
    [SerializeField]
    IntIntDictionary highTierValues;

    List<IntIntDictionary> tierValueList;

    enum ItemTier
    {
        LOW,
        MID,
        HIGH
    }

    void Start()
    {
        //Add our valeus to a list for easy indexing
        tierValueList = new List<IntIntDictionary>();
        tierValueList.Add(lowTierValues);
        tierValueList.Add(midTierValues);
        tierValueList.Add(highTierValues);

        //Set the colour for our crit and non crit colours
        critColor = new Color(232, 224, 0);
        nonCritColor = new Color(255, 255, 255);

        //Cache the players script so we can access damage numbers and such
        playerScript = player.GetComponent<Player>();

        enemyCritChance = 5.0f; //5 percent chance to recieve crit from enemy
        enemyCritDamage = 1.1f; //Enemy deals 10 percent more damage with crits
    }

    //Resolves a hit on an enemy from the player
    public void resolveHitFromPlayer(GameObject enemy, string objectHitWith, string enemyType)
    {
        float objectDmg = objectHitWith == "Spell" ? playerScript.spellDamage : objectHitWith == "AiSpell" ? playerScript.companionDamage : playerScript.swordDamage;
        GameObject dmgNum = Instantiate(damageIndicator); //Create our damage number
        Text dmgText = dmgNum.transform.GetChild(0).GetComponent<Text>();
        float generatedNum = Random.Range(0, 100.0f); //Generate a numbe between 0 and 100
        bool isCrit = playerScript.playerCritChance >= generatedNum ? true : false; //If its a crit or not
        float damageIncrease = playerScript.hasDamageBuff ? (playerScript.damageBuffIncrease / 100.0f) : 1;
        //Multiply our damage by our crit multiplier if we generated a number in our crit range
        float damage = objectDmg * damageIncrease * (isCrit ? playerScript.playerCritDamage : 1);
        Vector3 dmgLocation = Vector3.zero; //Where to spawn the damage number
        dmgText.text = damage.ToString("0.0"); //Set the text of the damage indicator
        dmgText.color = isCrit ? critColor : nonCritColor; //Set the colour of it

        //If the enmy is a minotaur
        if(enemyType == "Minotaur")
        {
            if (enemy.GetComponent<MinotaurAgent>().isActiveAndEnabled)
            {
                //Take away health from the enemy
                enemy.GetComponent<MinotaurAgent>().health = Mathf.Clamp(enemy.GetComponent<MinotaurAgent>().health - damage, 0, enemy.GetComponent<MinotaurAgent>().maxHealth);

                if (enemy.GetComponent<MinotaurAgent>().health == 0)
                {
                    DropLoot(enemy.transform.position);
                }
            }
            else
            {
                //Take away health from the enemy
                enemy.GetComponent<MinotaurEnemy>().health = Mathf.Clamp(enemy.GetComponent<MinotaurEnemy>().health - damage, 0, enemy.GetComponent<MinotaurEnemy>().maxHealth);

                if (enemy.GetComponent<MinotaurEnemy>().health == 0)
                {
                    DropLoot(enemy.transform.position);
                }
            }

            //Set the damage spawn location
            dmgLocation = enemy.transform.position;
        }
        else if(enemyType == "Demon")
        {
            //Take away health from the enemy
            enemy.GetComponent<DemonEnemy>().health -= damage;
            //Set the damage spawn location
            dmgLocation = enemy.transform.position;
        }
        else if(true) //Else if a different enemy
        {

        }

        //Set the position of the damage number indicator
        dmgNum.transform.position = dmgLocation;
    }

    public void resolveHitFromEnemy(int _damage)
    {
        GameObject dmgNum = Instantiate(damageIndicator); //Create our damage number
        Text dmgText = dmgNum.transform.GetChild(0).GetComponent<Text>();
        float generatedNum = Random.Range(0, 100.0f); //Generate a number between 0 and 100
        bool isCrit = enemyCritChance >= generatedNum ? true : false; //If its a crit or not
        //Multiply our damage by our crit multiplier if we generated a number in our crit range
        //Then multiply our damage by the dungeon level damage modifier
        float damage = (_damage * DungeonLevel.DMGModifier) * (isCrit ? enemyCritDamage : 1);
        dmgText.text = damage.ToString("0.0"); //Set the text of the damage indicator
        dmgText.color = isCrit ? critColor : enemyNonCritColor; //Set the colour of it
        //Set the position of the damage number indicator
        dmgNum.transform.position = player.transform.position;

        //Take away damage from players health
        playerScript.health = Mathf.Clamp(playerScript.health - (int)damage, 0, playerScript.maxHealth);

        playerScript.UpdateHealthBar();
    }

    //Method to drop loot if an enemy has died
    void DropLoot(Vector3 pos)
    {
        //Generate random number between 0 and 100
        float val = Random.Range(0.0f, 100.0f);

        //If the genarated numbe riss below or equal to our loot drop chance
        //then generate a new piece of loot with randomised attributes
        if (val <= lootDropChance) //*** CHANEG TOP DROP CHANCE
        {
            GameObject loot = Instantiate(lootDrop, pos, lootDrop.transform.rotation);
            LootDrop attribs = loot.GetComponent<LootDrop>();

            //Get a random type
            Item.ItemType type = (Item.ItemType)Random.Range(0, 4);
            //Get the sprite number to be used for the item
            int spriteNum = Random.Range(0, 6);

            //Generate random float from 0 to 3
            float tierDecider = Random.Range(0, 3.0f);

            //Get the item tier
            ItemTier tier = ItemTier.LOW;

            //Set the tier of the item based on the tier decider value
            if (tierDecider >= DungeonLevel.highTierChance)
                tier = ItemTier.HIGH;
            else if (tierDecider >= DungeonLevel.midTierChance)
                tier = ItemTier.MID;
            else
                tier = ItemTier.LOW;


            int affixAmount = 0;

            //Set the affix amount based on the tier
            switch(tier)
            {
                case ItemTier.LOW:
                    affixAmount = 2;
                    break;
                case ItemTier.MID:
                    affixAmount = 3;
                    break;
                case ItemTier.HIGH:
                    affixAmount = 4;
                    break;
            }

            //Since weapon has adamage, then set the damage of the weapon and
            //minus the affixAmount
            if(type == Item.ItemType.WEAPON)
            {
                ///Get the min and max valeus for this stat at that tier
                int min = tierValueList[(int)tier][affixNames[5]].first;
                int max = tierValueList[(int)tier][affixNames[5]].second + 1;

                attribs.statValues[5] = Random.Range(min, max);
                affixAmount--; //Take away one from the affixAmount
            }

            //Add all of the affixes to the item
            for(int i = 0; i < affixAmount; i++)
            {
                bool affixAdded = false;

                //Loop while we havnt added an affix
                while(affixAdded == false)
                {
                    int valueIndex = Random.Range(0, 6);

                    //Ensures damage and movemenet speed are only on feet and weapon
                    if (type != Item.ItemType.WEAPON && valueIndex == 5)
                        continue;
                    if (type != Item.ItemType.FEET && valueIndex == 4)
                        continue;

                    //If we havnt added the item, add it
                    else if (attribs.statValues[valueIndex] == 0)
                    {
                        ///Get the min and max valeus for this stat at that tier
                        int min = tierValueList[(int)tier][affixNames[valueIndex]].first;
                        int max = tierValueList[(int)tier][affixNames[valueIndex]].second + 1;

                        //Generate random value between the min and max and add it to the item
                        attribs.statValues[valueIndex] = Random.Range(min, max);

                        //Set to true as we added the affix to the item
                        affixAdded = true;
                    }
                }
            }

            //Set the values for the loot
            attribs.spriteNum = spriteNum;
            attribs.type = type;
            attribs.tier = (int)tier;
        }
    }
}
