using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ResetObjectsUponRespawn : MonoBehaviour
{
    public List<GameObject> Objects;
    private List<IResetOnRespawn> ObjectsToReset;
    // Start is called before the first frame update
    void Start(){
        ObjectsToReset = new List<IResetOnRespawn>();
        FillList();
    }

    private void FillList(){
        if (Objects.Count>0)
        {
            foreach (GameObject gObject in Objects)
            {
                if (gObject.TryGetComponent<IResetOnRespawn>(out IResetOnRespawn res))
                {
                    Debug.Log("test");
                    ObjectsToReset.Add(res);
                }
            }
        }
    }

    public void ResetObjects(){
        if (ObjectsToReset.Count>0)
        {
            Debug.Log("Reset objects");
            foreach (IResetOnRespawn reset in ObjectsToReset)
            {
                reset.RespawnReset();
            }
        }
    }
}
