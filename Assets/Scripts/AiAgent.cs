using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class was used in an attempt to train the AI companion using Machine Learning. This method turned out to be a failure
/// and is not used in the final build of the game but i will leave the code here to show previous attempts at training the AI.
/// </summary>
public class AiAgent : Agent {


    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    bool inRange;
    public List<Vector3> spawnLocations;
    public Vector3 aiSpawnLoc;
    public Transform player;
    public override void AgentReset()
    {
        //Move ai to one of the 4 random locations
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        transform.position = aiSpawnLoc;
        player.position = spawnLocations[Random.Range(0, 4)];
        inRange = false;
    }

    public override void CollectObservations()
    {
        AddVectorObs(player.position);
        AddVectorObs(transform.position);
    }

    public float speed = .7f;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = -vectorAction[1];

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.position,
                                                  player.position);

        float dot = Vector3.Dot(controlSignal.normalized, player.position.normalized);
        if(dot < -0.75)
        {
            AddReward(-.0001f);
            //Done();
        }

        // Reached target
        if (distanceToTarget < .75f)
        {
            AddReward(1.0f);
            Done();
        }
        else 
        {
            transform.position += controlSignal.normalized * speed * Time.fixedDeltaTime;
        }

        // Fell off platform
        if (this.transform.position.y < -1)
        {
            AddReward(-0.1f);
            Done();
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            inRange = true;
        }
    }
}
