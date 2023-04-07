using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
public class movement : Agent
{
    public float speed = 100;
    float rewardSum = 0;
    Rigidbody rBody;
    public Transform myTargetTransform;
    public Transform myAgentTransform;
    public Text textReward;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }


    void Update()
    {

    }

    
    public override void OnEpisodeBegin()
    {
        // Move the target to a new spot
        myAgentTransform.localPosition = new Vector3(Random.value * 8 - 4,
                                            0.5f,
                                            Random.value * 8 - 4);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(myTargetTransform.localPosition);
        sensor.AddObservation(myAgentTransform.localPosition);

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
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            rewardSum += 1.0f;
            textReward.text = "Reward" + rewardSum;
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            SetReward(-2.0f);
            rewardSum -= 2.0f;
            textReward.text = "Reward" + rewardSum;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
