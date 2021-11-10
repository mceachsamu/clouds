using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject plane;

    public GameObject fog;

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

        this.handleCollision();
    }

    public void setTargetLocation() {
        Vector3 diff = plane.transform.position - targetPosition;

        this.targetPosition = targetPosition + diff * moveToPositionRate * diff.magnitude;

        Vector3 dirDiff = plane.GetComponent<planeController>().getForward() - targetRotation;
        this.targetRotation = targetRotation + dirDiff * moveToRotationRate;
    }

    private void handleCollision() {
        RaycastHit hit;
        RaycastHit hit2;

        // GameObject cloud = new GameObject();

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, collisionDistance)) {
            print("FUCK");
            toggleOn = true;
        } else {
            toggleOn = false;
        }

        if (Physics.Raycast(this.transform.position, this.transform.forward * -1.0f, out hit2, collisionDistance)) {
            print("SHIT");
            toggleOff = true;
        } else {
            toggleOff = false;
        }

        if (toggleOn && toggleOff && hit.transform.gameObject.GetInstanceID() == hit2.transform.gameObject.GetInstanceID()) {
            cloud c = hit.transform.gameObject.GetComponent<cloud>();
            print("HERE");
            if (c != null) {
                print("here");
                this.fog.GetComponent<Renderer>().material.SetColor("_Color", c.baseColor);
                this.fog.GetComponent<Renderer>().material.SetFloat("_Transparency", 0.9f);
            }
        } else {
            this.fog.GetComponent<Renderer>().material.SetFloat("_Transparency", 0.0f);
        }
    }
}
