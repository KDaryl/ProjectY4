using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: This is the same class as MinotaurAgent but instead o using Machine Learning, it is hardcoded
/// with a Finite State Machine.
/// </summary>
public class MinotaurEnemy : MonoBehaviour {

    enum State
    {
        Idle,
        Seek,
        Attack
    }
    [SerializeField]
    Color fullColor, emptyColor;
    [SerializeField]
    Collider axeCollider;
    [SerializeField]
    EnemyHitScript EHS;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Collider bodyCollider;
    [SerializeField]
    AudioSource gruntSound;
    [SerializeField]
    AudioSource swingSound;

    public float attackDistance, moveSpeed;
    public Animator animator;
    public SpriteRenderer healthBarFill;
    HitResolver hitResolver; //For resolving hits

    Transform tf;
    GameObject player;
    Vector3 velocity;
    State currentState;
    float timeTillAttack, attackSpeed;
    bool deathTriggered;

    public float maxHealth;
    float fillPercentage;
    public float health { get; set; } //Need to access health
    const float maxHealthBarSize = 4.5f;

    // Use this for initialization
    void Start()
    {
        //Get the player game object, we search for it here as we can then load enemies dynamically
        player = GameObject.FindGameObjectWithTag("Player");
        hitResolver = GameObject.FindGameObjectWithTag("HitResolver").GetComponent<HitResolver>();
        tf = GetComponent<Transform>(); //Cache the transform of the object so we can move it
        attackSpeed = 2;
        timeTillAttack = 2;
        currentState = State.Idle;
        //Set max health to 200 multiplier by the dungeon level modifier
        maxHealth = 200 * DungeonLevel.HPModifier;
        health = maxHealth; //Set current health to max health
        deathTriggered = false;
        fillPercentage = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        //If health is 0 then set our trigger
        if (health <= 0)
        {
            //If the death triggered event hasnt happened yet, then trigger it
            if(deathTriggered == false)
            {
                if (velocity.z > 0)
                    animator.Play("DeadL");
                else
                    animator.Play("DeadR");

                animator.Play("Away", 1);
                deathTriggered = true;
                GameLoop.enemiesNeededTillComplete--;
                EHS.alive = false;
                rb.useGravity = false;
                bodyCollider.isTrigger = true;
            }
        }
        else
        {
            //If the current state is attack
            if (currentState == State.Attack)
            {

                //If the player has goen outside of the attack range, seek to him
                if (Vector3.Distance(tf.position, player.transform.position) > attackDistance)
                {
                    currentState = State.Seek;
                }
                else
                {
                    //If we can attack then attack
                    timeTillAttack += Time.deltaTime;

                    if (timeTillAttack >= attackSpeed)
                    {
                        animator.SetTrigger("Attack");
                        axeCollider.enabled = true;
                        timeTillAttack = 0;
                        swingSound.Play();
                    }
                }
            }
        }

        healthBarFill.color = Color.Lerp(fullColor, emptyColor, 1 - fillPercentage);
    }

    void FixedUpdate()
    {
        //Only move if alive
        if(health > 0)
        {
            //Reset animator variables
            animator.SetBool("MovingLeft", false);
            animator.SetBool("MovingRight", false);

            //Reset velocity
            velocity = Vector3.zero;

            if(currentState == State.Seek)
            {
                Seek(player.transform.position);

                //If the distance from the playe ris less than the attack range, then attack the player
                if(Vector3.Distance(tf.position, player.transform.position) < attackDistance)
                {
                    //Set the state to attack
                    currentState = State.Attack;
                }
            }

            //Add velocity to position
            tf.position += velocity;
        }
    }

    void Seek(Vector3 position)
    {
        velocity = (position - tf.position).normalized * moveSpeed * Time.fixedDeltaTime;

        //If moving left
        if(velocity.z > 0)
        {
            animator.SetBool("MovingLeft", true);
        }
        //If moving right
        else if(velocity.z < 0)
        {
            animator.SetBool("MovingRight", true);
        }
    }

    public void updateHealthBar()
    {
        //Clamp health to between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);
        fillPercentage = health / maxHealth;
        //Get the new width of the health bar
        float newWidth = fillPercentage * 4.5f;
        healthBarFill.size = new Vector2(newWidth, .5f);
    }

    void OnTriggerEnter(Collider col)
    {
        //Only check for collision triggers if we are alive
        if (health > 0)
        {
            //If the Minotaur has reached the players attack radius, then seek to the player
            if (col.CompareTag("Player Attack Radius"))
            {
                //Set the state
                currentState = State.Seek;
                animator.Play("Near", 1);

                int playGrunt = Random.Range(0, 101);

                //Chance of playing the monster grunt when playe ris nearby
                if (playGrunt <= 50)
                    gruntSound.Play();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (health > 0)
        {
            if (col.CompareTag("Player Attack Radius"))
            {
                //Set the state to idle
                currentState = State.Idle;
                animator.Play("Away", 1);
            }
        }
    }

    public void DestroyMinotaur()
    {
        Destroy(gameObject);
    }
}
