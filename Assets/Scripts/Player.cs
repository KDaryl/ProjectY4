using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used to manage the player. From here we will move the player, attack
/// and hold the players stats in the game (Critical Chance, Critical Damage, Damage, Health, Mana etc..). 
/// We also handle picking up loot from here with the Mouse Clicks and showing loot descriptions.
/// </summary>
public class Player : MonoBehaviour {
    [SerializeField]
    HitResolver hitResolver;

    [SerializeField]
    SpriteRenderer healthBar;
    [SerializeField]
    SpriteRenderer manaBar;

    [SerializeField]
    public int health;
    [SerializeField]
    public int maxHealth;
    [SerializeField]
    public int mana;
    [SerializeField]
    public int maxMana;

    public float moveSpeed;
    public Animator animator;
    public bool hasDamageBuff;
    public float damageBuffTimer;
    public int damageBuffIncrease; //The amount of damage (percent) extra the player deals when buffed
    float damageTimerLeft;

    //Our player attribute variables (these are percentages)[
    public float playerCritChance, playerCritDamage;
    public float manaCost;
    public float movementSpeed;
    public float spellDuration;
    public float swordDamage;
    public float spellDamage;
    public float companionDamage;

    Transform trans; //Transform of the player object 
    bool moveL, moveR, moveU, moveD, attacking;
    Vector3 directL, directR, directU, directD;
    Vector3 mDirect; //Movement direction

    public static float swordAttackRate, timeForSwordAttack;

    public Collider swordCollider;
    public

    int lootRayMask;

    //The item we have clicked to show the description of
    public static LootDrop itemClicked = null;

    //Bools for managing animations
    bool pickedUpItem;
    bool lastMovedLeft;

    [SerializeField]
    AudioSource meleeSound;

	void Start () {
        lastMovedLeft = false;
        hasDamageBuff = false;
        damageBuffTimer = 15; // initially starts at 15 seconds (we can increase this with items)
        damageBuffIncrease = 115; //Damage buf fincreases damage dealth with sword by 15 percent

        trans = GetComponent<Transform>(); //Cache the Transform
        //Create the vectors for our movement, we must define teh directions they move in as it is isometric
        directL = new Vector3(0, 0, 1);
        directR = new Vector3(0, 0, -1);
        directU = new Vector3(1, 0, 0);
        directD = new Vector3(-1, 0, 0);

        mDirect = new Vector3(0,0,0);

        swordAttackRate = .5f;
        timeForSwordAttack = 1;

        lootRayMask = 1 << 4; //Layer mask for the loot!
    }
	
	void Update () {
        lastMovedLeft = moveL ? true : false;
        //Set our movement variables depending on the input if we arent attacking
        moveL = !attacking ? Input.GetKey(KeyCode.A) : false;
        moveR = !attacking ? Input.GetKey(KeyCode.D) : false;
        moveU = !attacking ? Input.GetKey(KeyCode.W) : false;
        moveD = !attacking ? Input.GetKey(KeyCode.S) : false;
        attacking = false; //Set attacking to false

        if (timeForSwordAttack < swordAttackRate)
        {
            timeForSwordAttack += Time.deltaTime;
        }

        //Handling damage buff increase
        if(hasDamageBuff)
        {
            damageTimerLeft -= Time.deltaTime;

            if(damageTimerLeft <= 0)
            {
                hasDamageBuff = false;
            }
        }

        //Check if left click was pressed once
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            // Does the ray intersect any loot objects
            if (Physics.Raycast(ray, out hit, 200.0f, lootRayMask))
            {
                pickedUpItem = true;
                //If it does, then pickup and equip that item
                hit.collider.gameObject.GetComponent<LootDrop>().PickupItem();
            }
        }

        //If left click iss held
        if (Input.GetMouseButton(0) && pickedUpItem == false)
        {
            //Else If we can attack, then attack
            if (timeForSwordAttack >= swordAttackRate)
            {
                timeForSwordAttack = 0;
                animator.SetTrigger("Sword Attack");
                swordCollider.enabled = true; //Enable the sword collider
                meleeSound.Play(); //Play melee sound
            }
            attacking = true;
        }

        //Check if right mouse button was clicked
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            // Does the ray intersect any loot objects
            if (Physics.Raycast(ray, out hit, 200.0f, lootRayMask))
            {
                if (itemClicked != null)
                    itemClicked.HideDescription();

                //Set the item clicked
                itemClicked = hit.collider.gameObject.GetComponent<LootDrop>();

                //Show the description of the new item
                itemClicked.ShowDescription();                   
            }
        }

        //If the player unclicks the left mopuse btuton, set our bool to false
        if(Input.GetMouseButtonUp(0))
        {
            pickedUpItem = false;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.PauseGame();
        }
    }

    void FixedUpdate(){
        //Set our animator booleans
        animator.SetBool("Attacking", attacking);
        mDirect = Vector3.zero;

        //Move player using movement variables
        if (moveU)
            mDirect += directU;
        if (moveD)
            mDirect += directD;
        if (moveR)
            mDirect += directR;
        if (moveL)
            mDirect += directL;

        //Move our player if he isnt attacking
        if(!attacking)
            trans.position += mDirect.normalized * (moveSpeed * movementSpeed) * Time.deltaTime;

        //Play the animation that suits our action taken
        PlayActionAnimation();
    }

    //In this method we play our move animations
    //transitions are setup so we dont need to handle every edge case
    //such as (moving left and down), (moving right and down)
    void PlayActionAnimation()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("LastMovedLeft", lastMovedLeft);

        //If not moving or attacking, set our idel boolean in our animator
        if (!attacking 
            && (!moveL && !moveR && !moveU && !moveD)
            || (moveL && moveR && !moveU && !moveD)
            || (!moveL && !moveR && moveU && moveD))
        {
            animator.SetBool("Idle", true);
        }

        //If moving left
        else if (moveL && !moveR && !moveU && !moveD)
            animator.Play("Move Left 0");
        //If moving right
        else if (!moveL && moveR && !moveU && !moveD)
            animator.Play("Move Right 0");
        //If moving up
        else if (!moveL && !moveR && moveU && !moveD)
            animator.Play("Move Up 0");
        //If moving down
        else if (!moveL && !moveR && !moveU && moveD)
            animator.Play("Move Down 0");
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Melee Weapon"))
        {
            hitResolver.resolveHitFromEnemy(5); //Resolve hit from enemy

            Debug.Log("Hit by melee weapon");
            col.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    public void StartTemporaryDamageBuff()
    {
        hasDamageBuff = true;
        damageTimerLeft = damageBuffTimer;
    }

    public void UpdateHealthBar()
    {
        healthBar.size = new Vector2(4.5f * (health / (float)maxHealth), .5f);
    }

    public void UpdateManaBar()
    {
        manaBar.size = new Vector2(4.5f * (mana / (float)maxMana), .5f);
    }
}
