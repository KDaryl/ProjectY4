using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: SpellCast is used to handle casting spells by the player.
/// When the player pressed 1-4 on the keyboard (the keys assigned to the spells), 
/// this class will attempt to cast the spell.
/// </summary>
public class SpellCast : MonoBehaviour {

    bool spawnedCompanion;
    GameObject companion;

    [SerializeField]
    GameObject companionPrefab;

    [SerializeField]
    Player playerScript;
    [SerializeField]
    public int manaPerSec;

    [SerializeField]
    int healthPerSecond;

    float manaHpSecTimer;


    [SerializeField]
    public int healAmount;

    [SerializeField]
    List<SpellCooldown> spells; //Our list of spells (this holds the mana cost and such for the spell)

    [SerializeField]
    GameObject aoeSkullSpell; 

    //The buttons used to cast the spells
    List<KeyCode> spellBtns;

    Transform spellLocation;

    // Bit shift the index of the layer (9) to get a bit mask
    int layerMask = 1 << 9;

    // Use this for initialization
    void Start () {
        spellBtns = new List<KeyCode>();

        //Add buttons 1 to 4
        spellBtns.Add(KeyCode.Alpha1);
        spellBtns.Add(KeyCode.Alpha2);
        spellBtns.Add(KeyCode.Alpha3);
        spellBtns.Add(KeyCode.Alpha4);

        spawnedCompanion = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Loop through our buttons
        for(int i = 0; i < spellBtns.Count; i++)
        {
            //Check if the key is down
            if(Input.GetKeyDown(spellBtns[i]))
            {
                //Check if the spell thats assigned to this key is ready to be cast
                if(spells[i].spellReady)
                {
                    //Get the mana cost by multiplying by our mana cost percentage
                    //This is lowered based on gear
                    int manaCost = (int)Mathf.Clamp(spells[i].manaCost * playerScript.manaCost, 0, Mathf.Infinity);

                    if (playerScript.mana >= manaCost)
                    {
                        bool spellSuccess = true;
                        //If the spell is an AOE spell
                        if (spells[i].isAoe)
                        {
                            //Raycast from mouse to game and spawn the spell
                            RaycastHit hit;
                            Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(Input.mousePosition));
                            // Does the ray intersect any objects
                            if (Physics.Raycast(ray, out hit, 400.0f, layerMask))
                            {
                                //Spawn spell
                                GameObject s = Instantiate(aoeSkullSpell);

                                s.transform.position = new Vector3(hit.point.x, 1, hit.point.z);

                                //Set the time for the spell to be active by 4 seconds multiplied by the spell duration
                                //multiplier Of the player
                                s.GetComponent<DestroyTimer>().timeToLive = 4 * playerScript.spellDuration;

                            }
                            else
                            {
                                spellSuccess = false; //Didnt hit a target, so make hit sucess as false
                            }
                        }
                        else
                        {
                            if(i == 1)
                            {
                                playerScript.StartTemporaryDamageBuff(); //Set temp damage buff on player
                            }
                            //if 3 was pressed, spawn the players Ai companion
                            else if(i == 2)
                            {
                                if (spawnedCompanion)
                                {
                                    Destroy(companion);
                                }
                                else
                                    spawnedCompanion = true;

                                //Spawn companion
                                companion = Instantiate(companionPrefab, transform.position, companionPrefab.transform.rotation);
                            }
                            else if(i == 3) //Else if 4 was pressed, heal the player
                            {
                                //Increase the players health
                                playerScript.health = Mathf.Clamp(playerScript.health + healAmount, 0, playerScript.maxHealth);
                                playerScript.UpdateHealthBar();
                            }
                        }

                        //If the spells worked, take away from the players mana, set teh spell as used and updat ethe players mana bar
                        if (spellSuccess)
                        {
                            //Take away from mana
                            playerScript.mana = Mathf.Clamp(playerScript.mana - manaCost, 0, playerScript.maxMana);
                            spells[i].UseSpell(); //Set the spell as used
                            playerScript.UpdateManaBar();
                        }
                    }
                }
            }
        }

        manaHpSecTimer -= Time.deltaTime;

        //If our time rhas reached 0, add to our health and mana bar
        if (manaHpSecTimer <= 0)
        {
            playerScript.mana = Mathf.Clamp(playerScript.mana + manaPerSec, 0, playerScript.maxMana);
            playerScript.health = Mathf.Clamp(playerScript.health + healthPerSecond, 0, playerScript.maxHealth);

            //Update our health and mana bars
            playerScript.UpdateManaBar();
            playerScript.UpdateHealthBar();

            //Reset the timer
            manaHpSecTimer = 1;
        }

    }
}
