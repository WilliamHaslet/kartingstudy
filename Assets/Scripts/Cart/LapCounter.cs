using System;
using System.Collections.Generic;
using UnityEngine;

public class LapCounter : MonoBehaviour
{

    [SerializeField] private Layer checkPointLayer;
    [SerializeField] private TrackData trackData;

    private List<int> passedCheckpoints = new List<int>();
    private Action lapCompleteCallback;
    private int nextCheckpoint;
    private int lastCheckpoint;

    private void Start()
    {

        nextCheckpoint = 0;

        lastCheckpoint = trackData.checkpoints.Count - 1;

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == checkPointLayer)
        {

            lastCheckpoint = trackData.checkpoints.IndexOf(other.transform);

            nextCheckpoint = lastCheckpoint + 1;

            if (nextCheckpoint >= trackData.checkpoints.Count)
            {

                nextCheckpoint = 0;

            }

            AddCheckpoint(lastCheckpoint);

        }

    }

    private void AddCheckpoint(int index)
    {

        if (index == 0 && passedCheckpoints.Count >= trackData.minimumCheckpointCount)
        {

            // Null check for for test ai. Remove later?
            if (lapCompleteCallback != null)
            {

                lapCompleteCallback.Invoke();

            }

            passedCheckpoints.Clear();

        }
        
        if (!passedCheckpoints.Contains(index))
        {

            passedCheckpoints.Add(index);

        }

    }

    public Vector3 GetCurrentDirection()
    {

        return (trackData.checkpoints[nextCheckpoint].position - trackData.checkpoints[lastCheckpoint].position).normalized;

    }

    public Transform GetNextCheckPoint()
    {

        return trackData.checkpoints[nextCheckpoint];

    }
    
    public Transform GetLastCheckPoint()
    {

        return trackData.checkpoints[lastCheckpoint];

    }

    public void AddLapCallback(Action callback)
    {

        lapCompleteCallback += callback;

    }

    public float GetLapCompletionAtNext()
    {

        return nextCheckpoint / (float)trackData.checkpoints.Count;

    }
    
    public float GetLapCompletionAtLast()
    {

        return lastCheckpoint / (float)trackData.checkpoints.Count;

    }

}
