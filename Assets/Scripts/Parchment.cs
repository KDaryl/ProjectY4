using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used for the parchment that shows up for the end of a dungeon.
/// This class will let the user know if they failed/passed the dungeon. The level of the dungeon 
/// that they failed/passed. Along with the percentage of monsters killed and the time that they
/// had left to finish the dungeon.
/// </summary>
public class Parchment : MonoBehaviour {

    [SerializeField]
    List<SpriteRenderer> levelSprites;
    [SerializeField]
    List<SpriteRenderer> timeSprites;
    [SerializeField]
    List<Sprite> parchmentOutcomes;
    [SerializeField]
    SpriteRenderer parchment; //The passed or failed parchment to show
    [SerializeField]
    Animator animator; //Animator, for moving the parchment onto/off the screen

    //These are the green/red ticks we show on the oparchment if we completed the objectives
    [SerializeField]
    GameObject monsterTick;
    [SerializeField]
    GameObject timeTick;

    //Our tick sprites
    [SerializeField]
    Sprite greenTick;
    [SerializeField]
    Sprite redTick;

    [SerializeField]
    List<Sprite> numberSprite;

    [SerializeField]
    List<SpriteRenderer> monsterVals;

    [SerializeField]
    Animator transition;
    [SerializeField]
    SceneTransitioner sceneTransitioner;

    Vector3 monsterTickLocation;
    Vector3 timeTickLocation;

    List<string> endtimeStrs; //Our list of strings for the time left at the end of a dungeon

    // Use this for initialization
    void Start () {
        endtimeStrs = new List<string>();
        for (int i = 0; i < 3; i++)
            endtimeStrs.Add("");

        parchment.sprite = null;
        monsterTickLocation = new Vector3(-69, 20, 0);
        timeTickLocation = new Vector3(-69, -120, 0);
    }

    //Decides what type of parchement should be shown
    public void DecideParchmentType ()
    {
        //This method is called when the dungeon is finished (win or lose)
        Time.timeScale = 0;

        //If the dungeon was com,pleted, set green ticks for monster killed and time left
        if (GameLoop.completedDungeon)
        {
            parchment.sprite = parchmentOutcomes[0];
            monsterTick.GetComponent<SpriteRenderer>().sprite = greenTick;
            timeTick.GetComponent<SpriteRenderer>().sprite = greenTick;
            monsterTick.transform.localScale = new Vector3(6, 6, 1);
            timeTick.transform.localScale = new Vector3(6, 6, 1);
            monsterTick.transform.localPosition = monsterTickLocation;
            timeTick.transform.localPosition = timeTickLocation;
        }
        else //If the dungeon wasnt completed, set the red ticks for monster killed and time left
        {
            monsterTick.GetComponent<SpriteRenderer>().sprite = redTick;
            timeTick.GetComponent<SpriteRenderer>().sprite = redTick;
            monsterTick.transform.localScale = new Vector3(7, 7, 1);
            timeTick.transform.localScale = new Vector3(7, 7, 1);
            monsterTick.transform.localPosition = monsterTickLocation;
            timeTick.transform.localPosition = timeTickLocation;
            parchment.sprite = parchmentOutcomes[1];
        }

        //Gets the monster percent in string format
        string s = (100.0f * (1 - GameLoop.enemiesNeededTillComplete /  (float)GameLoop.totalEnemiesNeeded)).ToString("f0").PadLeft(3, '0');

        //Loop through each char of tghe string and convert it to an integer (index)
        for (int i = 0; i < s.Length; i++)
        { 
            //Assign the appropriate sprite for the letter
            monsterVals[i].sprite = numberSprite[(int)char.GetNumericValue(s[i])];
        }

        //Convert our time into strings for Seconds, Minutes, and Milliseconds
        endtimeStrs[0] = ((int)(GameLoop.timeLeft / 60)).ToString().PadLeft(2, '0'); //Min
        endtimeStrs[1] = ((int)(GameLoop.timeLeft - ((int)(GameLoop.timeLeft / 60) * 60))).ToString().PadLeft(2, '0'); //Secs
        endtimeStrs[2] = ((int)((GameLoop.timeLeft - (int)(GameLoop.timeLeft)) * 100)).ToString().PadLeft(2, '0'); //Milli

        //Set the sprites for the time left by converting the timeleft into char and then to integers 
        //to get an index for one of the number sprites and assing it to the sprite for the time.
        timeSprites[0].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[0][0])];
        timeSprites[1].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[0][1])];
        timeSprites[2].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[1][0])];
        timeSprites[3].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[1][1])];
        timeSprites[4].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[2][0])];
        timeSprites[5].sprite = numberSprite[(int)char.GetNumericValue(endtimeStrs[2][1])];

        //Set the level sprites
        string levelString = DungeonLevel.level.ToString().PadLeft(2, '0');
        levelSprites[0].sprite = numberSprite[(int)char.GetNumericValue(levelString[0])];
        levelSprites[1].sprite = numberSprite[(int)char.GetNumericValue(levelString[1])];
    }

    public void CloseParchment()
    {
        GameLoop.dungeonOver = false;
        transition.Play("SceneIn");
        //Asynchronisly load the camp scene
        sceneTransitioner.bgLoad = SceneManager.LoadSceneAsync("SampleScene");
        //Dont allow the scene to change automaticallys
        sceneTransitioner.bgLoad.allowSceneActivation = false;
    }

    public void CloseParchmentEnd()
    {
        //Tell our scene transitioner that the scene can now change
        sceneTransitioner.bgLoad.allowSceneActivation = false;
    }
}
