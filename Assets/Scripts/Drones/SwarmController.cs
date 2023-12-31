﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SwarmController : MonoBehaviour
{
    public float step = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetNearestTransportDroneToPosition(Vector3 target)
    {
        var currentMinimalDistance = float.PositiveInfinity;
        Transform nearestDrone = null;
        foreach (Transform droneContainer in transform)
        {
            var drone = droneContainer.Find("Drone");
            var transportStateMachine = droneContainer.GetComponentInChildren<TransportStateMachine>();
            if (transportStateMachine.CanTransport())
            {
                var distance = Vector3.Distance(drone.position, target);
                if (distance < currentMinimalDistance)
                {
                    currentMinimalDistance = distance;
                    nearestDrone = droneContainer;
                }
            }
        }
        return nearestDrone;
    }

    public List<Transform> GetDronesFromLeftToRightOnXAxis()
    {
        SortedDictionary<float, Transform> dict = new SortedDictionary<float, Transform>();
        foreach (Transform drone in transform)
        {
            dict.Add(drone.Find("Drone").position.x, drone);
        }
        return dict.Values.ToList();
    }

    public void RunAllAutoPilots()
    {
        List<AutoPilot> drones = new List<AutoPilot>();
        for (int i = 0; i < transform.childCount; i++)
        {
            drones.Add(transform.GetChild(i).Find("Drone").GetComponentInChildren<AutoPilot>());
        }
        float delay = 0;
        foreach (var pilot in drones)
        {
            if (!pilot.running)
            {
                pilot.delayedStartDuration = delay;
            }
            delay = delay + step;
        }
        foreach (var pilot in drones)
        {
            pilot.ExecuteAllSteps();
        }
    }

    public void UseCrazyflieConnectionForAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.SetToCrazyflieConnection();
        }
    }

    public void UseSimulatorForAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var simulator = drone.GetComponentInChildren<DroneSimulator>();
            simulator.UseSimulator();
        }
    }

    public void StartAllDrones()
    {
        foreach(Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneStart();
        }
    }

    public void LandAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneLand();
        }
    }

    public void ActivateSwarm()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.Activate);
        }
    }

    public void ActivateSwarmInDelayedSequence()
    {
        float countdown = 0;
        foreach (Transform drone in GetDronesFromLeftToRightOnXAxis())
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Countdown = countdown;
            transportStateMachine.Fire(TransportStateMachine.Trigger.Activate);
            countdown += 0.3f;
        }
    }

    public void WanderAlone()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.WanderAlone);
        }
    }

    public void WanderWithSwarm()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.WanderWithSwarm);
        }
    }

    public void EncircleHuman()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.EncircleHuman);
        }
    }

    public void SwitchToUpperFence()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.SwitchToUpperFence);
        }
    }

    public void SwitchToLowerFence()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.SwitchToLowerFence);
        }
    }

    public void GoHome()
    {
        foreach (Transform drone in transform)
        {
            var transportStateMachine = drone.GetComponentInChildren<TransportStateMachine>();
            transportStateMachine.Fire(TransportStateMachine.Trigger.GoHome);
        }
    }

    public void StopWandering()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<LeaderController>();
            leader.leadingDroneActive = false;
            leader.isWanderingActive = false;
            leader.isPursuitActive = false;
        }
    }

    public void LetsExactlyMoveHome()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneMoveHome();
        }
    }


}

[CustomEditor(typeof(SwarmController))]
public class SwarmControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SwarmController s = (SwarmController)target;

        if (GUILayout.Button("Use Crazyflie Connection"))
        {
            s.UseCrazyflieConnectionForAllDrones();
        }

        if (GUILayout.Button("Use Simulation"))
        {
            s.UseSimulatorForAllDrones();
        }

        if (GUILayout.Button("Start"))
        {
            s.StartAllDrones();
        }

        if (GUILayout.Button("Land"))
        {
            s.LandAllDrones();
        }

        if (GUILayout.Button("Activate"))
        {
            s.ActivateSwarm();
        }

        if (GUILayout.Button("Activate In Sequence"))
        {
            s.ActivateSwarmInDelayedSequence();
        }

        if (GUILayout.Button("Wander Alone"))
        {
            s.WanderAlone();
        }

        if (GUILayout.Button("Wander With Swarm"))
        {
            s.WanderWithSwarm();
        }

        if (GUILayout.Button("Upper Fence"))
        {
            s.SwitchToUpperFence();
        }

        if (GUILayout.Button("Lower Fence"))
        {
            s.SwitchToLowerFence();
        }

        if (GUILayout.Button("Go Home"))
        {
            s.GoHome();
        }

        if (GUILayout.Button("Stop Wandering"))
        {
            s.StopWandering();
        }

        if (GUILayout.Button("Move Home Exactly"))
        {
            s.LetsExactlyMoveHome();
        }


    }
}
