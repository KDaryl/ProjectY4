using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: Our game loop is used fo rour dungeon to handle if the dungeon is over. If the player has completed or failed the dungeon.
/// It also determiens how many enemies the player must kill before the time runs out. The time the player has to finish the dungeon is also
/// determined here (Start method). The bars shown under the minimap are also updated here to show the percent of time and enemies left.
/// </summary>
public class GameLoop : MonoBehaviour {
    //Our static fields, other scripts will sue these variables 
    public static int enemiesAlive = 0; //Enemeis alive, this will be incremented by other scripts
    public static int enemiesNeededTillComplete;
    public static bool completedDungeon;
    public static float timeLeft;
    public static int totalEnemiesNeeded;

    //Amount of enemies alive till completion
    float timeToComplete;
    float timeLeftPercent;
    float enemiesLeftPercent;
    bool endEventsDone;
    static public bool dungeonOver;

    //We serialize our bar fields, this will be our bar that is on our UI to indicate
    //the amount of enemies we have left and the amount of time we have left
    [SerializeField]
    SpriteRenderer timeBar;
    [SerializeField]
    SpriteRenderer monsterBar;
    [SerializeField]
    Animator parchmentAnimator;
    [SerializeField]
    Player playerScript;

    // Use this for initialization
    void Start () {
        endEventsDone = false;
        completedDungeon = true;
        dungeonOver = false;

        //The player needs to kill 80 percent of the enemies to complete the dungeon
        enemiesNeededTillComplete = (int)(enemiesAlive * 0.8f);
        totalEnemiesNeeded = enemiesNeededTillComplete;

        //We have 4 minutes (240 seconds) + the amount of enemies multiplied by 2 seconds to complete the dungeon
        timeToComplete = 240 + (enemiesAlive * 2);
        timeLeft = timeToComplete; //Start the timer

        //Set our percentages to 0
        timeLeftPercent = 0;
        enemiesLeftPercent = 0;

        updateBars();
    }
	
	// Update is called once per frame
	void Update () {

        if (dungeonOver)
        {
            //If we havnt done our end game events, do them
            if (endEventsDone == false)
            {
                parchmentAnimator.Play("Dungeon End");
                endEventsDone = true;
                enemiesAlive = 0;
            }
        }
        else
        {

            timeLeft -= Time.deltaTime; //Minus time from our timer

            //If the player has killed enough enemies, go back
            if (enemiesNeededTillComplete <= 0)
            {
                dungeonOver = true;
                Debug.Log("Completed dungeon, increase dungeon level and teleport back");

                //Increase the dungeon level + difficulty
                DungeonLevel.LevelUpDungeon();
            }
            else if (timeLeft <= 0 || playerScript.health == 0) //If time ran out or the playe rhas no health
            {
                timeLeft = 0;
                completedDungeon = false;
                dungeonOver = true;
                Debug.Log("Failed dungeon");
            }
            else //If game hasnt ended yet
            {
                timeLeftPercent = Mathf.Clamp(1 - timeLeft / timeToComplete, 0, 1); //Get the percentage of time left
                enemiesLeftPercent = Mathf.Clamp(1 - enemiesNeededTillComplete / (float)totalEnemiesNeeded, 0, 1); //Get the percentage of enemies left

                //Update bars
                updateBars();
            }
        }
    }

    void updateBars()
    {
        //Updates the sizes of our bars
        timeBar.size = new Vector2(4.5f * timeLeftPercent, .25f);
        monsterBar.size = new Vector2(4.5f * enemiesLeftPercent, .25f);
    }
}
