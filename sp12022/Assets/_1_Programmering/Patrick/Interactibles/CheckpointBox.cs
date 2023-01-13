using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBox : MonoBehaviour
{
    private GameObject Closed, Open;
    // Start is called before the first frame update
    void Start(){
        Closed = transform.Find("Closed").gameObject;
        Open = transform.Find("Open").gameObject;
    }

    public void OpenBox(){
        Closed.SetActive(false);
        Open.SetActive(true);
    }

    public void CloseBox(){
        Closed.SetActive(true);
        Open.SetActive(false);
        
    }
}
