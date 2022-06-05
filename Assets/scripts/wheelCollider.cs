using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelCollider : MonoBehaviour
{

    public float collisionDistance = 10.0f;
    public float bounceMultiplier = 0.5f;
    public float landSlowDownRate = 0.9f;
    public float minLift = 0.5f;
    public GameObject plane;

    // Start is called before the first frame update
    void Start()
    {
        plane = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCollidingWithWater()) {
            Rigidbody planeRb =  plane.GetComponent<Rigidbody>();
            Vector3 velocity = planeRb.velocity;

            // add counter force to make plane bounce on water surface
            // planeRb.AddForce();
            
            Vector3 forceDir = new Vector3(0.0f, -1.0f * (velocity.y - minLift) * bounceMultiplier, 0.0f);
            planeRb.AddForce(forceDir);
            
            planeRb.velocity *= landSlowDownRate;
        }
    }

    private bool isCollidingWithWater() {
        RaycastHit hit;
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.right * collisionDistance, Color.red);
        
        if (Physics.Raycast(this.transform.position, this.transform.right, out hit, collisionDistance))
        {
            if (hit.transform.gameObject.tag == "water") {
                return true;
            }
        }
        
        return false;
    }
}
