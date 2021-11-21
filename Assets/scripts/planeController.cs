using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeController : MonoBehaviour
{

    private float yTilt;

    public float yTiltRate;

    public float thrust;
    public float liftAmount;
    public float liftPull;

    public float baseLift;

    public float stabilisation;

    private Rigidbody rb;

    public GameObject[] colliderPositions;
    
    public float collisionDistance;

    public GameObject cloudController;

    public float minParticleFade;
    public float maxParticleFade;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 windDir = new Vector3(0.0f, 0.0f, this.getForward().z).normalized;
        Vector3 forward = this.getForward().normalized;
        Vector3 wing = -1.0f * this.transform.right.normalized;
        
        Vector3 reflect = this.transform.worldToLocalMatrix * (wing * 2.0f * Vector3.Dot(wing, windDir) - windDir);
        reflect.y *= -1.0f;
        reflect.x *= -1.0f;
        
        reflect = this.transform.localToWorldMatrix * reflect;
        reflect.x = 0.0f;
        reflect.z = 0.0f;

        Vector3 pullY = new Vector3(forward.x, 0.0f, forward.z);
        rb.AddForce(pullY * Mathf.Abs(rb.velocity.y) * liftPull);
        
        Vector3 pullX = new Vector3(0.0f, forward.y, 0.0f);
        rb.AddForce(pullX * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) * liftPull);
        
        float liftV = rb.velocity.magnitude > 1.0f ? 1.0f : rb.velocity.magnitude;
        // add our lift force
        rb.AddForce(reflect.normalized * liftAmount * liftV);

        // add forward thrust force
        rb.AddForce(this.getForward() * thrust);
        
        // stabilize the wings
        float stabilise = this.stabilisation * this.transform.forward.y;
        rb.AddTorque(new Vector3(0.0f, 0.0f, stabilise));

        // add our turning torques  
        if (Input.GetKey("w")) {
            Vector3 direction = this.transform.forward * yTiltRate * - 1.0f;
            rb.AddTorque(direction);
        }
        if (Input.GetKey("s")) {
            Vector3 direction = this.transform.forward * yTiltRate;
            rb.AddTorque(direction);
        }
        if (Input.GetKey("a")) {
            Vector3 direction = this.getForward() * yTiltRate;
            rb.AddTorque(direction);
        }
        if (Input.GetKey("d")) {
            Vector3 direction = this.getForward() * yTiltRate * -1.0f;
            rb.AddTorque(direction);
        }

        for (int i = 0; i < colliderPositions.Length; i++) {
            handleCollision(colliderPositions[i]);
        }
    }

    public float getSpeed() {
        return this.GetComponent<Rigidbody>().velocity.magnitude;
    }

    private void handleCollision(GameObject collider) {
        RaycastHit hit;

        if (Physics.Raycast(collider.transform.position, this.getForward(), out hit, collisionDistance))
        {
            colliderController collide = collider.GetComponent<colliderController>();
            collide.SetTransparencyFade(getCollisionParticleFade());
            cloudSpawner cloudControl = cloudController.GetComponent<cloudSpawner>();
            cloud cloud = hit.transform.gameObject.GetComponent<cloud>();
            collide.baseColor = cloud.baseColor;
            collide.turnOn();
            cloud.SetColliding(true, collide.transform.position, this.GetComponent<Rigidbody>().velocity);
            cloud.resetCounter();
        }
    }

    private float getCollisionParticleFade() {
        return Random.Range(minParticleFade, maxParticleFade);
    }

    public Vector3 getForward() {
        return this.transform.up;
    }
    
    public Vector3 getUp() {
        return this.transform.right;
    }
}
