using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is attached to enemies to handle when they are struct by enemies. If the enemy is hit by a players sword,
/// they will call the HandleHitFromPlayer(..) method in the hitResolver script and pass in any necessary details to resolve
/// the collision.
/// </summary>
public class EnemyHitScript : MonoBehaviour {
    
    [SerializeField]
    string enemyType = "";
    [SerializeField]
    MinotaurEnemy script;
    [SerializeField]
    MinotaurAgent maScript;

    HitResolver hitResolver; //For resolving our hits
    bool canTakeDamage;
    float timeTillSwordHit;

    public bool alive;

    private void Start()
    {
        alive = true;
        if (enemyType == "")
        {
            Debug.Log("You never gave an enemytype, we wont be able to resolve collisions");
        }

        timeTillSwordHit = Player.swordAttackRate;
        canTakeDamage = true;
        hitResolver = GameObject.FindGameObjectWithTag("HitResolver").GetComponent<HitResolver>();
    }

    private void Update()
    {
        //If the enemy has been hit by a sword, start our time rand make the enemy not take
        //any more sword damage till the player can attack again
        if(canTakeDamage == false)
        {
            timeTillSwordHit -= Time.deltaTime; //Take away time goen sinc elast frame

            if(timeTillSwordHit <= 0)
            {
                canTakeDamage = true;
                timeTillSwordHit = Player.swordAttackRate;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (alive)
        {
            if (col.CompareTag("Player Sword") && canTakeDamage)
            {
                canTakeDamage = false;
                //Reslove the hit by calling our hit resolver
                hitResolver.resolveHitFromPlayer(gameObject, "Sword", enemyType);
            }
            else if (col.CompareTag("Spell"))
            {
                hitResolver.resolveHitFromPlayer(gameObject, "AiSpell", enemyType);
                col.gameObject.transform.parent.gameObject.GetComponent<SpellMover>().DestroySpell(transform.position);
            }
            else if (col.CompareTag("SkullSpell"))
            {
                hitResolver.resolveHitFromPlayer(gameObject, "Spell", enemyType);
                col.gameObject.GetComponent<Collider>().enabled = false; //Set spell to not active
            }
            if (maScript.isActiveAndEnabled == false)
                script.updateHealthBar();
            else
                maScript.updateHealthBar();
        }
    }
}
