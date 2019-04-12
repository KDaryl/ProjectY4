using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class is very similar to the minotaur enemy and will be used in the future when new enemies can be added.
/// </summary>
public class DemonEnemy : MonoBehaviour
{

    enum State
    {
        Idle,
        Seek,
        Attack
    }
    [SerializeField]
    Color fullColor, emptyColor;
    [SerializeField]
    Collider handCollider;


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

    float maxHealth, fillPercentage;
    public float health { get; set; } //Need to access health
    const float maxHealthBarSize = 4.5f;

    // Use this for initialization
    void Start()
    {
        //Get the player game object, we search for it here as we can then load enemies dynamically
        player = GameObject.FindGameObjectWithTag("Player");
        hitResolver = GameObject.FindGameObjectWithTag("HitResolver").GetComponent<HitResolver>();
        tf = GetComponent<Transform>(); //Cache the transform of the object so we can move it
        attackSpeed = 1.5f;
        timeTillAttack = 1.5f;
        currentState = State.Idle;
        health = 100;
        maxHealth = 100;
        deathTriggered = false;
        fillPercentage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //If health is 0 then set our trigger
        if (health == 0)
        {
            //If the death triggered event hasnt happened yet, then trigger it
            if (deathTriggered == false)
            {
                animator.SetTrigger("Dead");
                deathTriggered = true;
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
                        handCollider.enabled = true;
                        timeTillAttack = 0;
                    }
                }
            }

            //If moving left
            if (velocity.z > 0)
            {
                animator.SetBool("Facing Left", false);
            }
            //If moving right
            else if (velocity.z < 0)
            {
                animator.SetBool("Facing Left", true);
            }
        }

        healthBarFill.color = Color.Lerp(fullColor, emptyColor, 1 - fillPercentage);
    }

    void FixedUpdate()
    {
        //Reset animator variables

        //Only move if alive
        if (health > 0)
        {
            //Reset velocity
            velocity = Vector3.zero;

            if (currentState == State.Seek)
            {
                Seek(player.transform.position);

                //If the distance from the playe ris less than the attack range, then attack the player
                if (Vector3.Distance(tf.position, player.transform.position) < attackDistance)
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

    }

    void updateHealthBar()
    {
        //Clamp health to between 0 and maxHealth
        health = clamp(health, 0, maxHealth);
        fillPercentage = health / maxHealth;
        //Get the new width of the health bar
        float newWidth = fillPercentage * 4.5f;
        healthBarFill.size = new Vector2(newWidth, .5f);
    }

    float clamp(float val, float min, float max)
    {
        if (val < min)
        {
            val = min;
        }
        else if (val > max)
        {
            val = max;
        }
        return val;
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
                animator.SetTrigger("NearPlayer");
            }

            else if (col.CompareTag("Player Sword") && tag == "Enemy")
            {

                //Reslove the hit by calling our hit resolver
                hitResolver.resolveHitFromPlayer(gameObject, "Sword", "Demon");
                col.enabled = false; // Disable the swords collider
            }
            updateHealthBar();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player Attack Radius"))
        {
            //Set the state to idle
            currentState = State.Idle;
            animator.SetTrigger("AwayPlayer");
        }
    }
}
