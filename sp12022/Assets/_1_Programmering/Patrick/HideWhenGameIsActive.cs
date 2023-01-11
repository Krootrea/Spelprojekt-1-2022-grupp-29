using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhenGameIsActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.SetActive(false);
    }
}
