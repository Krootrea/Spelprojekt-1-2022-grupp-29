using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHandler : MonoBehaviour
{
    private GameObject[] hasLightObjects;
    private Dictionary<Light2D, bool> lightsInScene;
    public float LightTurnOnAtDistance = 12.5f;
    private CameraEdges cameraEdges;

    private GameObject player;
    // Start is called before the first frame update
    void Start(){
        cameraEdges = Camera.main.GetComponent<CameraEdges>();
        lightsInScene = new Dictionary<Light2D, bool>();
        player = GameObject.FindGameObjectWithTag("Player");
        hasLightObjects = GameObject.FindGameObjectsWithTag("HasLight");
        foreach (GameObject obj in hasLightObjects) 
        {
            if (obj.TryGetComponent(out Light2D light2D))
                lightsInScene.Add(light2D, obj.activeSelf);
        }   
    }

    // Update is called once per frame
    void LateUpdate(){
        Vector3 playerPosition = player.transform.position;
        
        foreach (KeyValuePair<Light2D, bool> light in lightsInScene)
        {
            // if (light.Value)
            // {
                bool isCloseEnough = CheckIfLightIsCloseEnough(light.Key);
                if (light.Key.isActiveAndEnabled && !isCloseEnough) 
                    light.Key.gameObject.SetActive(false);
                else if (!light.Key.isActiveAndEnabled && isCloseEnough)
                    light.Key.gameObject.SetActive(true);
            
        }
    }

    private bool CheckIfLightIsCloseEnough(Light2D light2D){
        Vector3 lightP = light2D.transform.position;
        Vector3 playerP = player.transform.position;
        float leftBoardX = cameraEdges.getLeft(lightP.z).x;
        float rightBoardX = cameraEdges.getRight(lightP.z).x;
        float topBoardY = cameraEdges.getTop(lightP.z).y;
        float bottomBoardY = cameraEdges.getBottom(lightP.z).y;
        float space = 10f;
        
        bool closeEnough = lightP.x < (rightBoardX + space) 
               && lightP.x > (leftBoardX - space)
               && lightP.y < (topBoardY + space)
               && lightP.y > (bottomBoardY - space);
        return closeEnough;
    }
}
