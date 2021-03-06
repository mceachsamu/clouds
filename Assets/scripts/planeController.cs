using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeController : MonoBehaviour
{

    private float yTilt;

    public float yTiltRate;

    public float thrustMin;
    public float thrustMax;

    public float thrust;

    public float thrustIncrease;
    
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

    public float collisionForce;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        for (int i = 0; i < colliderPositions.Length; i++) {
            StartCoroutine(handleCollisionWithClouds(colliderPositions[i]));
            StartCoroutine(handleCollisionWithLand(colliderPositions[i]));
        }
    }

    void OnGUI() {
        this.GetComponent<Animator>().speed = thrust;
    }

    // Update is called once per frame
    void FixedUpdate()
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
        float tiltFactor = getTiltFactor();
        if (Input.GetKey("w")) {
            Vector3 direction = this.transform.forward * yTiltRate * - 1.0f * tiltFactor;
            rb.AddTorque(direction, ForceMode.VelocityChange);
        }
        if (Input.GetKey("s")) {
            Vector3 direction = this.transform.forward * yTiltRate * tiltFactor;
            rb.AddTorque(direction, ForceMode.VelocityChange);
        }
        if (Input.GetKey("a")) {
            Vector3 direction = this.getForward() * yTiltRate * tiltFactor;
            rb.AddTorque(direction, ForceMode.VelocityChange);
        }
        if (Input.GetKey("d")) {
            Vector3 direction = this.getForward() * yTiltRate * -1.0f * tiltFactor;
            rb.AddTorque(direction, ForceMode.VelocityChange);
        }

        if (Input.GetKey("p")) {
            if (thrust < thrustMax) {
                this.thrust += thrustIncrease;
            }
        }
        if (Input.GetKey("o")) {
            if (thrust > thrustMin) {
                this.thrust -= thrustIncrease;
            }
        }
    }

    public float getTiltFactor() {
        return Mathf.Clamp(this.GetComponent<Rigidbody>().velocity.magnitude, 0.0f, 10.0f)/10.0f;
    }

    public float getSpeed() {
        return this.GetComponent<Rigidbody>().velocity.magnitude;
    }

    private IEnumerator handleCollisionWithClouds(GameObject collider) {
        RaycastHit hit;

        while (true) {
            if ((Physics.Raycast(collider.transform.position, this.getForward(), out hit, collisionDistance)
            || Physics.Raycast(collider.transform.position, this.getForward() * -1.0f, out hit, collisionDistance))
            && hit.transform.gameObject.tag == "cloud")
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

            yield return new WaitForSeconds(0.01f);
        }
    }


    private IEnumerator handleCollisionWithLand(GameObject collider) {
        RaycastHit hit;

        while (true) {
            if ((Physics.Raycast(collider.transform.position, this.getForward(), out hit, collisionDistance)
            || Physics.Raycast(collider.transform.position, this.getForward() * -1.0f, out hit, collisionDistance))
            && hit.transform.gameObject.tag == "water")
            {
                // get the direction from the collider to the object
                Vector3 collisionDir = (collider.transform.position - hit.transform.position).normalized;
                this.rb.AddForceAtPosition(collisionDir * 1.0f * collisionForce, collider.transform.position);


            }

            yield return new WaitForSeconds(0.01f);
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

    public Vector3 getRight() {
        return this.transform.forward;
    }
}
