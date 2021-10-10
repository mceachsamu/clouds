using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeController : MonoBehaviour
{

    private float yTilt;

    public float yTiltRate;

    public float thrust;
    public float liftAmount;

    public float baseLift;

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

        Vector3 windDir = new Vector3(0.0f, 0.0f,  this.getForward().z).normalized;//this.getForward();
        Vector3 forward = this.getForward().normalized;

        float liftDir = this.getForward().y < 0.0f ? -1.0f : 1.0f;
        
        float lift = (1.0f - Vector3.Dot(windDir, forward) + baseLift) * liftDir * liftAmount;

        print(lift);
        
        // add our lift force
        rb.AddForce(this.transform.right * -1.0f * lift);

        Debug.DrawLine(this.transform.position, (this.transform.position + windDir * 600.0f), Color.yellow);

        // add forward thrust force
        rb.AddForce(this.getForward() * thrust);

        // add our turning torques
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
            Vector3 direction = this.getForward() * yTiltRate;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }
        if (Input.GetKey("d")) {
            Vector3 direction = this.getForward() * yTiltRate * -1.0f;
            Debug.DrawLine(this.transform.position, (this.transform.position + direction * 600.0f), Color.red);
            rb.AddTorque(direction);
        }

        for (int i = 0; i < colliderPositions.Length; i++) {
            handleCollision(colliderPositions[i]);
        }
    }

    private void handleCollision(GameObject collider) {
        RaycastHit hit;

        Debug.DrawLine(collider.transform.position, (collider.transform.position + this.getForward() * collisionDistance), Color.green);
        if (Physics.Raycast(collider.transform.position, this.getForward(), out hit, collisionDistance))
        {
            
            Debug.Log("Did Hit");
            colliderController collide = collider.GetComponent<colliderController>();
            collide.SetTransparencyFade(getCollisionParticleFade());
            cloud_spawner cloudControl = cloudController.GetComponent<cloud_spawner>();
            collide.setMoveSpeed(cloudControl.GetCloudMoveSpeed()/10.0f);
            collide.setMoveDirection(cloudControl.cloudMoveDirection);
            collide.baseColor = hit.transform.gameObject.GetComponent<cloud>().baseColor;
            collide.turnOn();
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
