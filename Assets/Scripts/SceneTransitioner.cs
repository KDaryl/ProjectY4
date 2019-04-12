using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used for swapping between classes and for also beginning a dungeon.
/// It has an animtor attached that will play a transition (black circle closes in, and gets bigger, opens up).
/// This is used to remove the instabnt scene change that occurs when scenes are loaded.
/// </summary>
public class SceneTransitioner : MonoBehaviour {

    [SerializeField]
    bool sceneIn;
    [SerializeField]
    Animator animator;

    //Used for loading the async operation
    public AsyncOperation bgLoad;

    void Start()
    {
        if(sceneIn)
        {
            animator.Play("SceneOut");
        }
    }

    public void LoadDungeon()
    {
        bgLoad.allowSceneActivation = true;
        Time.timeScale = 1;
    }
}
