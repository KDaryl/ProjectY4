using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Daryl Keogh
/// Description: Our Pause menu is simply used to play animations when pause is pressed. It will stop the game from updating
/// and will have methods that can be called in the animator and buttons atatched to the pause menu to move to other scenes.
/// You can also end the current dungeon you are in if you wish, along with going to the main menu or simply continue playing.
/// </summary>
public class PauseMenu : MonoBehaviour {

    [SerializeField]
    GameObject transition;

    public static Animator animator;

    public static bool gamePaused = false;

    bool exitDung;

    private void Start()
    {
        exitDung = false;
        animator = GetComponent<Animator>();
    }

    //Pauses the game
    public static void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0;
        animator.Play("Pause#");
    }

    //Unpauses the game
    public void Continue()
    {
        animator.Play("PauseOut");
    }

    //This ends the current dungeon and returns to camp
    public void ExitDungeon()
    {
        if (SceneManager.GetActiveScene().name == "DungeonScene")
        {

            GameLoop.timeLeft = 0;
            animator.Play("PauseOut");
            exitDung = true;
        }
    }

    //This is called to go to the main menu scene and begins the scene transition
    public void MainMenu()
    {
        animator.Play("PauseOut");
        transition.GetComponent<SceneTransitioner>().bgLoad = SceneManager.LoadSceneAsync("MainMenu");
        transition.GetComponent<SceneTransitioner>().bgLoad.allowSceneActivation = false;
        transition.GetComponent<Animator>().Play("SceneIn");
    }

    //Called in our PauseOut animation to reset the time scale of the game so it can continue updating again
    public void EndPause()
    {
        if(exitDung == false)
            Time.timeScale = 1;
        gamePaused = false;
    }
}
