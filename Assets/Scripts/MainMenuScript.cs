using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to handle the main menu of the game. It rotates the camera slightly
/// to get rid of the static-ness of the main menu and can switch to the camp scene of the game when the
/// Start Game button is pressed.
/// </summary>
public class MainMenuScript : MonoBehaviour {

    [SerializeField]
    GameObject transition;
    [SerializeField]
    GameObject camera;

    private void Update()
    {
        camera.transform.Rotate(new Vector3(0, -1* Time.deltaTime, 0));
    }

    public void StartGame()
    {
        transition.GetComponent<SceneTransitioner>().bgLoad = SceneManager.LoadSceneAsync("SampleScene");
        transition.GetComponent<SceneTransitioner>().bgLoad.allowSceneActivation = false;
        transition.GetComponent<Animator>().Play("SceneIn");
    }

    public void EndGame()
    {
        Application.Quit();// Close the game
    }
}
