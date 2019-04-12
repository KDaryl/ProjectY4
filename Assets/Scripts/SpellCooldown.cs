using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This is ued to set the cooldown of spells on the UI bar. It will make the dark circle that appears on
/// the spells after use to decreas ein size once their cooldown reaches 0
/// </summary>
public class SpellCooldown : MonoBehaviour {

    [SerializeField]
    Transform mask;

    Vector3 initialMaskSize;

    //Static bool to hold if the spell is ready to be cast or not
    [SerializeField]
    public bool spellReady;
    [SerializeField]
    public bool isAoe;

    //Cooldown time of the spell
    [SerializeField]
    float cooldownTime;

    [SerializeField]
    public int manaCost;

    float initialCooldownTime;

	// Use this for initialization
	void Start () {
        initialCooldownTime = cooldownTime; //Set the initialCooldownTime
        initialMaskSize = mask.localScale; //Set the initial scale of the mask

        //If the spell is ready, set the scale to 0
        if(spellReady)
            mask.localScale = initialMaskSize * 0;
    }
	
	// Update is called once per frame
	void Update () {
        // If the spell is not ready
		if(spellReady == false)
        {
            cooldownTime -= Time.deltaTime; //Minus from our cooldown time

            if(cooldownTime <= 0)
            {
                spellReady = true; //Set spell ready
                cooldownTime = 0; //Set cooldown to 0
            }

            mask.localScale = initialMaskSize * (cooldownTime / initialCooldownTime);
        }
	}

    public void UseSpell()
    {
        spellReady = false; //Set spell ready to false
        cooldownTime = initialCooldownTime; //Reset timer
    }
}
