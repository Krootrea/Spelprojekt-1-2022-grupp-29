using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
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
            if(light.Key.transform.gameObject.TryGetComponent<TokenInstance>(out TokenInstance t)
                && t.Collected){
                    lightsInScene.Remove(light.Key);
                    continue;
                }
            bool isCloseEnough = CheckIfTransformIsCloseEnough(light.Key.transform);
            if (light.Key.isActiveAndEnabled && !isCloseEnough) 
                light.Key.gameObject.SetActive(false);
            else if (!light.Key.isActiveAndEnabled && isCloseEnough)
                light.Key.gameObject.SetActive(true);
        }
    }

    private bool CheckIfTransformIsCloseEnough(Transform thingy){
        Vector3 thingyP = thingy.position;
        Vector3 playerP = player.transform.position;
        float leftBoardX = cameraEdges.getLeft(thingyP.z).x;
        float rightBoardX = cameraEdges.getRight(thingyP.z).x;
        float topBoardY = cameraEdges.getTop(thingyP.z).y;
        float bottomBoardY = cameraEdges.getBottom(thingyP.z).y;
        float space = 10f;
        
        bool closeEnough = thingyP.x < (rightBoardX + space) 
               && thingyP.x > (leftBoardX - space)
               && thingyP.y < (topBoardY + space)
               && thingyP.y > (bottomBoardY - space);
        return closeEnough;
    }
}
