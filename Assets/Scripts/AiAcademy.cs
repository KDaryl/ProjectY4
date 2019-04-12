using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
/// <summary>
/// Author: Daryl Keogh
/// Description: The AI Academy is used as a sort of entry point for Unity ML Agents and must be present for it to work correctly.
/// This script also stops Unity ML Agents from messing with the time scale and framerate cap on the game when using trained models.
/// </summary>
public class AiAcademy : Academy {

    [SerializeField]
    bool isTraining = false;

    void Start()
    {
        //If we are not training AI, then make sure our game is full screened and the framerate capture is set to 0 (no cap)
        if (!isTraining)
        {
            Time.captureFramerate = 0;
            Screen.fullScreen = true;
        }
    }
}
