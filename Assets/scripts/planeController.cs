using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeController : MonoBehaviour
{

    private float yTilt;

    public float yTiltRate;

    public float thrust;

    private Rigidbody rb;

    public GameObject[] colliderPositions;
    
    public float collisionDistance;

    public GameObject cloudController;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // add thrust force
        rb.AddForce(this.transform.up * thrust);
        if (Input.GetKey("w")) {
            Vector3 direction = this.transform.forward * yTiltRate * - 1.0f;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }
        if (Input.GetKey("s")) {
            Vector3 direction = this.transform.forward * yTiltRate;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }
        if (Input.GetKey("a")) {
            Vector3 direction = this.transform.up * yTiltRate;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }
        if (Input.GetKey("d")) {
            Vector3 direction = this.transform.up * yTiltRate * -1.0f;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }

        for (int i = 0; i < colliderPositions.Length; i++) {
            handleCollision(colliderPositions[i]);
        }
    }

    private void handleCollision(GameObject collider) {
        RaycastHit hit;

        

        Debug.DrawLine(collider.transform.position, (collider.transform.position + this.transform.up * collisionDistance), Color.green);
        if (Physics.Raycast(collider.transform.position, this.transform.up, collisionDistance))
        {
            Debug.Log("Did Hit");
            colliderController collide = collider.GetComponent<colliderController>();
            cloud_spawner cloudControl = cloudController.GetComponent<cloud_spawner>();
            collide.setMoveSpeed(cloudControl.GetCloudMoveSpeed()/10.0f);
            collide.setMoveDirection(cloudControl.cloudMoveDirection);

            collide.turnOn();
        }
    }
}
