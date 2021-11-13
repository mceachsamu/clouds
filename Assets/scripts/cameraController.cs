using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject plane;

    public float offsetZ;
    public float offsetY;

    private Vector3 targetPosition;
    private Vector3 targetRotation;
    
    public float moveToPositionRate;
    public float moveToRotationRate;

    public float collisionDistance;

    private Boolean toggleFog = false;



    private Boolean toggleOn;

    private Boolean toggleOff;

    public float maxOpacity;

    private float currentOpacity = 0.0f;

    public float opacityChange;


    // Start is called before the first frame update
    void Start()
    {
        this.targetPosition = plane.transform.position;
        this.targetRotation = plane.GetComponent<planeController>().getForward();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.setTargetLocation();
        this.transform.position = this.targetPosition;

        Vector3 planeForward = this.targetRotation;//plane.GetComponent<planeController>().getForward();
        this.transform.forward = planeForward;
        
        Vector3 planeUp = plane.GetComponent<planeController>().getUp();
        Vector3 pos = this.transform.position - planeForward * offsetZ + planeUp * offsetY;
        this.transform.position = pos;

        Vector3 cameraForward = new Vector3(planeForward.x, planeForward.y, this.transform.forward.z);
        this.transform.forward = cameraForward;
    }

    public void setTargetLocation() {
        Vector3 diff = plane.transform.position - targetPosition;

        this.targetPosition = targetPosition + diff * moveToPositionRate * diff.magnitude;

        Vector3 dirDiff = plane.GetComponent<planeController>().getForward() - targetRotation;
        this.targetRotation = targetRotation + dirDiff * moveToRotationRate;
    }
}
