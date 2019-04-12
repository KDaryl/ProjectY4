using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Author: Daryl Keogh
/// Description: The CampLevelHandler is used to start the dungeon. It displays the current dungeon level of the dungeon
/// and will start the dungeon if the player clcisk the green dot or it will close the parchment if the player
/// presses the red X.
/// </summary>
public class CampLevelHandler : MonoBehaviour {

    [SerializeField]
    Animator animator;
    [SerializeField]
    Animator transition;
    [SerializeField]
    SpriteRenderer firstNum;
    [SerializeField]
    SpriteRenderer secondNum;

    [SerializeField]
    List<Sprite> numberSprites;

    [SerializeField]
    SceneTransitioner transitioner;

    public void StartAnimation()
    {
        //Set the sprite based on the dungeon level
        string levelStr = DungeonLevel.level.ToString();
        levelStr = levelStr.PadLeft(2, '0');

        firstNum.sprite = numberSprites[(int)char.GetNumericValue(levelStr[0])];
        secondNum.sprite = numberSprites[(int)char.GetNumericValue(levelStr[1])];

        animator.Play("DungeonStartAnim");
        Time.timeScale = 0;
    }

    public void EndAnimation()
    {
        Time.timeScale = 1;
        animator.Play("DungeonStartAnimOut");
    }

    public void StartDungeon()
    { 
        transition.Play("SceneIn");
        transitioner.bgLoad = SceneManager.LoadSceneAsync("DungeonScene", LoadSceneMode.Single);
        transitioner.bgLoad.allowSceneActivation = false;
    }
}
