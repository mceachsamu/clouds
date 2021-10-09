using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeController : MonoBehaviour
{

    private float yTilt;

    public float yTiltRate;

    public float thrust;

    private Rigidbody rb;

    public GameObject centerOfMass;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // rb.centerOfMass = centerOfMass.transform.position;
        // add thrust force
        rb.AddForce(this.transform.up * thrust);
        // print(this.transform.up);

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
    }
}
