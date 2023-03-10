using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraEdges : MonoBehaviour
{
    private Camera cam;

    private void Awake(){
        cam = Camera.main;
    }

    // edges
    public Vector3 getTop(float zValue){
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        return this.transform.TransformPoint(new Vector3(0, y, distance));
    }

    private float DistanceZ(float zValue){
        float distance = zValue - cam.transform.position.z;
        return Mathf.Abs(distance);
    }

    public Vector3 getBottom(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        return this.transform.TransformPoint(new Vector3(0, -y, distance));
    }
    public Vector3 getLeft(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * this.cam.aspect;
        return this.transform.TransformPoint(new Vector3(-x, 0, distance));
    }
    public Vector3 getRight(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * cam.aspect;
        return this.transform.TransformPoint(new Vector3(x, 0, distance));
    }


// corners
    public Vector3 getTopLeft(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * cam.aspect;
        return this.transform.TransformPoint(new Vector3(-x, y, distance));
    }
    public Vector3 getTopRight(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * cam.aspect;
        return this.transform.TransformPoint(new Vector3(x, y, distance));
    }
    public Vector3 getBottomLeft(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * cam.aspect;
        return this.transform.TransformPoint(new Vector3(-x, -y, distance));
    }
    public Vector3 getBottomRight(float zValue)
    {
        float distance = DistanceZ(zValue);
        float y = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
        float x = y * cam.aspect;
        return this.transform.TransformPoint(new Vector3(x, -y, distance));
    }
}
