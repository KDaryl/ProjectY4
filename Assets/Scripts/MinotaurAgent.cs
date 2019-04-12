using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used to teach our AI how to perform actions. Each setup is different for
/// how each action was trained. I have commented out but left the code for each action so you can
/// look at how the reward function (void AgentAction(..)) is setup for each one. The code that is uncommented
/// is the one which combines all the brains together to create a finite state machine for swapping between
/// brains to perform those actions.
/// </summary>

//Seeking Player
/*public class MinotaurAgent : Agent
{
    [SerializeField]
    List<Transform> spawnLocations;
    [SerializeField]
    GameObject player;
    public float speed = .7f;

    void Start()
    {
        if (player == null)
        {
            //Get the player object
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void AgentReset()
    {
        transform.position = spawnLocations[Random.Range(0, spawnLocations.Count)].position;
    }

    public override void CollectObservations()
    {
        AddVectorObs(player.transform.position); //Look after players position
        AddVectorObs(transform.position); //Look at the enemies position
    }

 
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = -vectorAction[1];

        //Get distance to player
        float distanceToTarget = Vector3.Distance(transform.position,
                                        player.transform.position);

        AddReward(-.0001f);

        //If enemy reached player give reward and set done
        if(distanceToTarget < 0.375f)
        {
            Done();
            AddReward(1); 
        }
        else //Else move
        {
            transform.position += controlSignal.normalized * speed * Time.fixedDeltaTime;
        }
    }

    void OnTriggerExit(Collider col)
    {
        //If we have exited the players attack radius then minus a reward from the enemy
        if(col.CompareTag("Player Attack Radius"))
        {
            Done();
            AddReward(-1);
        }
    }
}*/

//Fleeing player (Low health)
/*public class MinotaurAgent : Agent
{
    [SerializeField]
    List<Transform> spawnLocations;
    GameObject player;
    public float speed = .7f;

    void Start()
    {
        //Get the player object
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void AgentReset()
    {
        transform.position = spawnLocations[Random.Range(0, spawnLocations.Count)].position;
    }

    public override void CollectObservations()
    {
        AddVectorObs(player.transform.position); //Look after players position
        AddVectorObs(transform.position); //Look at the enemies position
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = -vectorAction[1];

        //Get distance to player
        float distanceToTarget = Vector3.Distance(transform.position,
                                        player.transform.position);

        //Add time penalty to flee
        AddReward(-.001f);

        //Move enemy
        transform.position += controlSignal.normalized * speed * Time.fixedDeltaTime;
    }

    void OnTriggerExit(Collider col)
    {
        //If we have exited the players attack radius then minus a reward from the enemy
        if (col.CompareTag("Player Attack Radius"))
        {
            Done();
            AddReward(1);
        }
    }
}
*/

//Attacking player
/*public class MinotaurAgent : Agent
{
    [SerializeField]
    GameObject player;
    public float attackCooldown = .3f;
    float attackTimer;
    bool attacked;

    private void Start()
    {
        attacked = false;
        attackTimer = 2; //Hit every 2 seconds
    }

    private void Update()
    {
        if(attacked)
        {
            attackTimer -= Time.deltaTime;

            if(attackTimer <= 0)
            {
                attacked = false;
                attackTimer = 2;
            }
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(player.transform.position); //Look after players position
        AddVectorObs(transform.position); //Look at the enemies position
        AddVectorObs(attackTimer); //Make our brain look after our attackTimer so it knows when to attack
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 1
        float attack = vectorAction[0]; //Get the value for the action

        //If the value from the brain is 1 and the attack is not on cooldown
        if (attack == 1 && attackTimer == 2)
        {
            attacked = true;
            AddReward(1);
            Done();
        }
        else if(attack == 1 && attackTimer != 2)
        {
            AddReward(-.1f);
        }
        //If the ai does nothing while cooldown for attack, then reward it for waiting
        else if(attack == 0 && attackTimer != 2)
        {
            AddReward(1);
        }
        //Else punish
        else
            AddReward(-.001f);


    }
}*/

//Mixing Actions together
public class MinotaurAgent : Agent
{
    enum State
    {
        Idle,
        Seek,
        Flee,
        Attack
    }
    [SerializeField]
    Color fullColor, emptyColor;
    [SerializeField]
    Collider axeCollider;
    [SerializeField]
    EnemyHitScript EHS;
    [SerializeField]
    Collider bodyCollider;
    [SerializeField]
    AudioSource gruntSound;
    [SerializeField]
    AudioSource swingSound;

    public Animator animator;
    public SpriteRenderer healthBarFill;
    HitResolver hitResolver; //For resolving hits


    [SerializeField]
    List<string> brainNames;
    [SerializeField]
    List<Brain> brains;

    Dictionary<string, Brain> brainDict; //Our map of brains

    GameObject player;

    [SerializeField]
    float speed = .35f;

    State currentState;
    State newState;
    float attackTimer;
    bool attacked;
    bool deathTriggered;

    public float maxHealth;
    float fillPercentage;
    public float health { get; set; } //Need to access health
    const float maxHealthBarSize = 4.5f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        brainDict = new Dictionary<string, Brain>(); //Create dictionary
        //Setup our map
        for(int i = 0; i < brainNames.Count; i++)
        {
            brainDict.Add(brainNames[i], brains[i]);
        }
        attackTimer = 2;
        currentState = State.Idle; //SetActionMask current state to Idle
        newState = currentState;

        //Get the player object
        player = GameObject.FindGameObjectWithTag("Player");
        attacked = false;
        hitResolver = GameObject.FindGameObjectWithTag("HitResolver").GetComponent<HitResolver>();
        //Set max health to 200 multiplier by the dungeon level modifier
        maxHealth = 200 * DungeonLevel.HPModifier;
        health = maxHealth; //Set current health to max health
        deathTriggered = false;
        fillPercentage = 1;
    }

    public override void AgentReset()
    {
        
    }

    //This is where will give brain svariables to look out for
    public override void CollectObservations()
    {
        AddVectorObs(player.transform.position); //Look after players position
        AddVectorObs(transform.position); //Look at the enemies position

        //If we are using the attack brain, le tit look at the attackTimer
        if (currentState == State.Attack)
        {
            AddVectorObs(attackTimer);
        }
    }

    private void Update()
    {
        //If health is 0 then set our trigger
        if (health <= 0)
        {
            //If the death triggered event hasnt happened yet, then trigger it
            if (deathTriggered == false)
            {
                //Play death animation based on what way we are facing
                if (rb.velocity.z > 0)
                    animator.Play("DeadL");
                else
                    animator.Play("DeadR");

                //Play the way animation (fade out health bar)
                animator.Play("Away", 1);

                deathTriggered = true;
                GameLoop.enemiesNeededTillComplete--;
                EHS.alive = false;
                rb.useGravity = false;
                bodyCollider.isTrigger = true;
            }
        }
        //If we arre in the attack brain and we attacked, count down our timer
        else if (currentState == State.Attack)
        {
            if (attacked)
            {
                attackTimer -= Time.deltaTime;

                if (attackTimer <= 0)
                {
                    attacked = false;
                    attackTimer = 2;
                }
            }
        }
    }

    void FixedUpdate()
    {
        //If the brain should be changed and the ai is still alive, swap
        //the brain based on the new state
        if(newState != currentState && health > 0)
        {
            switch(newState)
            {
                case State.Idle:
                    GiveBrain(brainDict["Idle"]); //Idle as player has left our radius (cant see him anymore)
                    break;

                case State.Seek:
                    GiveBrain(brainDict["Seek"]); //Seek to player now that he is in range
                    break;

                case State.Flee:
                    GiveBrain(brainDict["Flee"]);
                    break;
                case State.Attack:
                    GiveBrain(brainDict["Attack"]);
                    break;
            }

            //Set the current state to the new state
            currentState = newState;
        }
    }

    //This is our agents reward function, but since this version of the agent is not in training
    //we will not be punishing/rewarding the ai for their actions, only processing them
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (health > 0)
        {
            if (currentState == State.Attack)
            {
                //if the ai puts out a 1 to attack, set our booleans and play our animation
                if (vectorAction[0] == 1 && attacked == false)
                {
                    animator.SetTrigger("Attack");
                    axeCollider.enabled = true;
                    swingSound.Play();
                    attacked = true;
                }
            }
            //I fthe enemy is fleeing or seeking, move the ais
            else if (currentState == State.Seek || currentState == State.Flee)
            {
                // Actions, size = 2
                Vector3 controlSignal = Vector3.zero;
                controlSignal.x = vectorAction[0];
                controlSignal.z = -vectorAction[1];

                //Get distance to player
                float distanceToTarget = Vector3.Distance(transform.position,
                                                player.transform.position);

                //If the ai is seeking and we reached the player, switch our new state to attack 
                //FixedUpdate() will then swap the brain
                if (currentState == State.Seek)
                {
                    if (distanceToTarget <= 0.35)
                    {
                        newState = State.Attack;
                    }
                }

                //Move enemy
                transform.position += controlSignal.normalized * speed * Time.fixedDeltaTime;

                //Reset animator variables
                animator.SetBool("MovingLeft", false);
                animator.SetBool("MovingRight", false);

                //Play our moving animation based on what way the ai is moving
                if (rb.velocity.z < 0)
                    animator.SetBool("MovingLeft", true);
                else
                    animator.SetBool("MovingRight", true);
            }
        }
    }

    //Method used to update the size of the health bar of the enemy
    public void updateHealthBar()
    {
        //Clamp health to between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);
        fillPercentage = health / maxHealth;
        //Get the new width of the health bar
        float newWidth = fillPercentage * 4.5f;
        healthBarFill.size = new Vector2(newWidth, .5f);
    }

    //Exiting player attack radius
    void OnTriggerExit(Collider col)
    {
        if (health > 0)
        {
            //If we have exited the players attack radius then minus a reward from the enemy
            if (col.CompareTag("Player Attack Radius"))
            {
                newState = State.Idle;
                animator.Play("Away", 1);
            }
        }
    }
    
    //Entering player attack radius
    void OnTriggerEnter(Collider col)
    {
        if (health > 0)
        {
            //If we have exited the players attack radius then minus a reward from the enemy
            if (col.CompareTag("Player Attack Radius"))
            {
                newState = State.Seek;

                animator.Play("Near", 1);

                int playGrunt = Random.Range(0, 101);

                //Chance of playing the monster grunt when playe ris nearby
                if (playGrunt <= 50)
                    gruntSound.Play();
            }
        }
    }
}
