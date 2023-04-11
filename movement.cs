using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
using Random=UnityEngine.Random;
public class movement : Agent
{
    public float speed = 100;
    float rewardSum = 0;
    int counterEpisode = 0;
    Rigidbody rBody;
    public Transform myTargetTransform;
    public Transform myAgentTransform;
    public Transform myObstacleTransform;
    public Text textReward;
    public Text textEpisode;
    public Vector3 movement_ = new Vector3(1, 0, 0);
    public int movingObsRandom1;
    public int movingObsRandom2;
    public int stepsInEpisode = 0;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }


    void Update()
    {

        Vector3 movement_ = new Vector3(movingObsRandom1, 0, movingObsRandom2);
        if (myObstacleTransform.localPosition.x < 4 && myObstacleTransform.localPosition.x > -4 &&
            myObstacleTransform.localPosition.z < 4 && myObstacleTransform.localPosition.z > -4)
        {
        myObstacleTransform.Translate(movement_ * Time.deltaTime * 2, Space.World);
        }
    }

    
    public override void OnEpisodeBegin()
    {
        movingObsRandom1 = Random.Range(-2,2);
        movingObsRandom2 = Random.Range(-2,2);
        // Move the target to a new spot
        //for doubles
        float agentNewRandomX = Random.Range(-3,3);
        float agentNewRandomZ = Random.Range(-3,3);

        float targetNewRandomX = Random.Range(-3,3);
        float targetNewRandomZ = Random.Range(-3,3);

        float obstacleNewRandomX = Random.Range(-3,3);
        float obstacleNewRandomZ = Random.Range(-3,3);

        while ( (Math.Abs(agentNewRandomX - targetNewRandomX) < 2 || Math.Abs(agentNewRandomZ - targetNewRandomZ) < 2) ||
                (Math.Abs(agentNewRandomX - obstacleNewRandomX) < 2 || Math.Abs(agentNewRandomZ - obstacleNewRandomZ) < 2) ||
                (Math.Abs(targetNewRandomX - obstacleNewRandomX) < 2 || Math.Abs(targetNewRandomZ - obstacleNewRandomZ) < 2)
              )
        {
            agentNewRandomX = Random.Range(-3,3);
            agentNewRandomZ = Random.Range(-3,3);

            targetNewRandomX = Random.Range(-3,3);
            targetNewRandomZ = Random.Range(-3,3);

            obstacleNewRandomX = Random.Range(-3,3);
            obstacleNewRandomZ = Random.Range(-3,3);
        }

        myAgentTransform.localPosition = new Vector3(agentNewRandomX, 0.5f, agentNewRandomZ);
        myTargetTransform.localPosition = new Vector3(targetNewRandomX, 0.5f, targetNewRandomZ);
        myObstacleTransform.localPosition = new Vector3(obstacleNewRandomX, 0.5f, obstacleNewRandomZ);

    }


    public override void CollectObservations(VectorSensor sensor)
    {
        
        stepsInEpisode++;
        // Target and Agent positions
        sensor.AddObservation(myTargetTransform.localPosition);
        sensor.AddObservation(myAgentTransform.localPosition);
        sensor.AddObservation(myObstacleTransform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }


    public float forceMultiplier = 10;
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0] * 30.0f;
        controlSignal.z = actionBuffers.ContinuousActions[1] * 30.0f;
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, myTargetTransform.localPosition);

        // Reached target
        if (stepsInEpisode > 100)
        {
            stepsInEpisode = 0;
            EndEpisode();
        }
        
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            rewardSum += 1.0f;
            textReward.text = "Reward = " + rewardSum;
            EndEpisode();
            counterEpisode++;
            textEpisode.text = "Episode = " + counterEpisode;
        }

        float distanceToObstacle = Vector3.Distance(this.transform.localPosition, myObstacleTransform.localPosition);

        // Obstacle collision
        if (distanceToObstacle < 1.42f)
        {
            SetReward(-1.0f);
            rewardSum -= 1.0f;
            textReward.text = "Reward = " + rewardSum;
            EndEpisode();
            counterEpisode++;
            textEpisode.text = "Episode = " + counterEpisode;
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-2.0f);
            rewardSum -= 2.0f;
            textReward.text = "Reward = " + rewardSum;
            EndEpisode();
            counterEpisode++;
            textEpisode.text = "Episode = " + counterEpisode;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
