using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: The Ai companion handles the movement and enemies in range. It will move to the player if out of range,
/// attack enemies in range of the AI companion. 
/// </summary>
public class AICompanion : MonoBehaviour {

    [SerializeField]
    GameObject spell;
    [SerializeField]
    float attackSpeed;
    [SerializeField]
    float followDistance;
    [SerializeField]
    AudioSource spellCast;

    public GameObject player;
    public float moveSpeed;

    enum State
    {
        Follow,
        Idle,
        Attack
    }

    //Set current state to follow
    State currentState = State.Idle;
    Vector3 velocity;
    Transform tf; //Cache the AIs transform
    Rigidbody rb;
    float attackRate;

    //The enemy we want to attack
    GameObject attackingEnemy;

    //variables for handling enemies in range
    List<GameObject> enemiesInRange;

	void Start () {
        enemiesInRange = new List<GameObject>();
        velocity = new Vector3(0, 0, 0);
        tf = GetComponent<Transform>();
        attackRate = attackSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void Update () {
		
        if(currentState == State.Attack)
        {
            attackRate -= Time.deltaTime;

            //Checking if we need to update our list
            if(attackingEnemy == null && enemiesInRange.Count > 0)
            {
                //Remove all gameobjects in the list that are null
                enemiesInRange.RemoveAll(GameObject => GameObject == null);

                if (enemiesInRange.Count > 0)
                    attackingEnemy = enemiesInRange[0];
            }
            //If its time to attack, spawn a spell from the AI
            if (attackRate <= 0 && attackingEnemy != null)
            {
                Vector3 pos = transform.position;
                GameObject newSpell = Instantiate(spell, new Vector3(pos.x, pos.y - .03f, pos.z), spell.transform.rotation);
                newSpell.GetComponent<SpellMover>().SpellLookAt(attackingEnemy);
                attackRate = attackSpeed;
                spellCast.time = spellCast.clip.length * .2f;
                spellCast.Play();
            }
            //If we arent attacking an enemy and there are none in range, follow the player
            else if (attackingEnemy == null && enemiesInRange.Count == 0)
            {
                currentState = State.Follow;
            }
            else //Else get an enemy to attack
            {
                //Target the top of the list enemy
                attackingEnemy = enemiesInRange[0];
            }
        }

	}

    void FixedUpdate()
    {
        //Reset the AI's velocity
        velocity = Vector3.zero;

        //If we are to follow the player
        if(currentState == State.Follow)
        {
            //Seek to the players position
            Seek(player.transform.position);

            if(Vector3.Distance(transform.position, player.transform.position) < followDistance)
            {
                if (enemiesInRange.Count > 0)
                    currentState = State.Attack;
                else
                    currentState = State.Idle;
            }
        }

        //Add velocity to the AI's position
        tf.position += velocity;
    }

    //Seeks to a position
    void Seek(Vector3 position)
    {
        velocity = (position - tf.position).normalized * moveSpeed * Time.fixedDeltaTime;
    }
    

    void OnTriggerExit(Collider col)
    {
        //If the AI companion has left the players follow radius, set the AI's state to follow
        if(col.CompareTag("Player") && currentState != State.Attack)
        {
            currentState = State.Follow; //If out of range of player, seek to him
        }
        if (col.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(col.gameObject); //Remove that enemy from the list
            attackingEnemy = null; //Reset the enemy we are attacking
            currentState = State.Follow; //If out of range of player, seek to him
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if (enemiesInRange.Count == 0)
                currentState = State.Idle;
            else
                currentState = State.Attack;
        }

        if(col.CompareTag("Enemy"))
        {
            attackingEnemy = null; //Reset the enemy we are attacking
            currentState = State.Attack;
            enemiesInRange.Add(col.gameObject); //Add the enemy to our enemies in range list
        }
    }
}
