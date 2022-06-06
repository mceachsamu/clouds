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

    public GameObject firstPersonPosition;

    private Boolean toggleFog = false;



    private Boolean toggleOn;

    private Boolean toggleOff;

    public float maxOpacity;

    private float currentOpacity = 0.0f;

    public float opacityChange;

    public int cameraSwitch = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        this.targetPosition = plane.transform.position;
        

        this.transform.forward = plane.GetComponent<planeController>().getForward();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FirstPersonCam();
        if (Input.GetKey("1")) {
            cameraSwitch = 1;
        }
        if (Input.GetKey("2")) {
            cameraSwitch = 0;
        }

        if (cameraSwitch == 0) {
            FirstPersonCam();
        } else {
            ThirdPersonCam();
        }
    }

    public void FirstPersonCam() {
        this.transform.parent = plane.transform;
        // this.setTargetLocation();
        this.transform.position = firstPersonPosition.transform.position;
        // this.transform.position = plane.transform.position;
        
    }

    public void ThirdPersonCam() {
        this.setTargetLocation();
        this.transform.position = this.targetPosition;

        Vector3 planeForward = this.targetRotation;//plane.GetComponent<planeController>().getForward();
        this.transform.forward = plane.GetComponent<planeController>().getForward();
        
        Vector3 planeUp = plane.GetComponent<planeController>().getUp();
        Vector3 pos = this.transform.position - planeForward * offsetZ + planeUp * offsetY;
        this.transform.position = pos;
    }

    public void setTargetLocation() {
        Vector3 diff = plane.transform.position - targetPosition;

        this.targetPosition = targetPosition + diff * moveToPositionRate * diff.magnitude;

        Vector3 dirDiff = plane.GetComponent<planeController>().getForward() - targetRotation;
        this.targetRotation = targetRotation + dirDiff * moveToRotationRate;
    }
}
