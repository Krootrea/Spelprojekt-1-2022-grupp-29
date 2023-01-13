using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class OnRespawn : MonoBehaviour
{
    public List<Checkpoint> checkpointObjects;

    private void LateUpdate(){
        if (SeveralCheckpointsActive())
        {
            ClearPastActiveCheckpoints();
        }
    }

    private void ClearPastActiveCheckpoints(){
        List<Checkpoint> activeCheckpointsToRemove = new List<Checkpoint>();
        foreach (Checkpoint checkpoint in checkpointObjects) {
            if (checkpoint.Active)
                activeCheckpointsToRemove.Add(checkpoint);
        }
        if (activeCheckpointsToRemove.Count > 1)
        {
            activeCheckpointsToRemove.Remove(activeCheckpointsToRemove[^1]);
            foreach (Checkpoint checkpoint in activeCheckpointsToRemove)
            {
                checkpointObjects.Remove(checkpoint);
            }
        }
    }

    private bool SeveralCheckpointsActive(){
        int checkpointsActive=0;
        foreach (Checkpoint checkpoint in checkpointObjects)
        {
            if (checkpoint.Active)
            {
                checkpointsActive++;
            }
        }
        if (checkpointsActive>1)
            return true;
        return false;
    }

    public void Respawn(){
        // HÄr ska vi då köra ResetObjectsUponRespawn (det ska ligga en "ResetObjectsUponRespawn.cs" på varje
        // checkpoint som då ska köras på den som är aktiv).
        if (SeveralCheckpointsActive())
            ClearPastActiveCheckpoints();
        else
        {
            foreach (Checkpoint cp in checkpointObjects)
            {
                cp.ResetObjects();
            }
        }
        
    }
}
