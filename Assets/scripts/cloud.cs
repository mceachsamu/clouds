using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    public cloudSpawner cloudSpawner;

    public float cloudMoveSpeed;

    private static float startTransparency = 1.0f;

    private float transparency = startTransparency;

    public float fadeRate;

    public Color baseColor;
    
    private bool fading;
    private Vector3 resetPosition;
    public int colorIndex;

    private Collider collider;
    Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices, vertexVelocities;
    private float force = 30.0f;
    private float forceOffset = 0.1f;

    private float springForce = 1.0f;

    private float damping = 1.0f;

    private float distancePower = 2.0f;

    private bool colliding;

    private Vector3 collidingPosition;

    private Vector3 collidingDirection;

    private int counter = 0;

    private int maxDuration = 200;

    private Boolean isInteractive;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
        this.GetComponent<Renderer>().material.SetVector("_Color", baseColor);

        collider = this.GetComponent<Collider>();

        deformingMesh = this.GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length];

		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        pos += cloudSpawner.cloudMoveDirection * cloudMoveSpeed;

        if (this.transform.position.x > cloudSpawner.center.x + cloudSpawner.getRangeX()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }
        if (this.transform.position.x < cloudSpawner.center.x -cloudSpawner.getRangeX()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }
        if (this.transform.position.z > cloudSpawner.center.z + cloudSpawner.getRangeZ()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }
        if (this.transform.position.z < cloudSpawner.center.z - cloudSpawner.getRangeZ()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }
        if (this.transform.position.y > cloudSpawner.center.y + cloudSpawner.getRangeY()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }
        if (this.transform.position.y < cloudSpawner.center.y -cloudSpawner.getRangeY()/2.0f) {
            resetPosition = cloudSpawner.generateCloudPosition();
            fading = true;
        }

        this.transform.position = pos; 
        fadeOut();
        fadeIn();

        if (colliding) {
            counter++;
            if (isInteractive) {
                print("JERE");
                this.UpdateVertices();
            }
        }

        if (counter >= maxDuration) {
            colliding = false;
            counter = 0;
        }

        this.setCloudMaterialProperties();
    }

    public void resetCounter() {
        this.counter = 0;
    }

    private void setCloudMaterialProperties() {
        int index = this.colorIndex;
        colorSet currentColorSet = this.cloudSpawner.GetWeatherController().getCurrentColorSet();

        this.baseColor = currentColorSet.colors[index];
        this.GetComponent<Renderer>().material.SetVector("_Color", currentColorSet.colors[index]);
        this.GetComponent<Renderer>().material.SetFloat("_Glossiness", currentColorSet.cloudGloss);
        this.GetComponent<Renderer>().material.SetFloat("_RimAmount", currentColorSet.cloudRim);
        this.GetComponent<Renderer>().material.SetVector("_RimColor", currentColorSet.rimColor);
        this.GetComponent<Renderer>().material.SetVector("_SpecularColor", currentColorSet.glossColor);
        this.GetComponent<Renderer>().material.SetFloat("_BackLightStrength",currentColorSet.backlightStrength);
        this.GetComponent<Renderer>().material.SetFloat("_BackLightPower", currentColorSet.backlightPower);
        this.GetComponent<Renderer>().material.SetVector("_BacklightColor", currentColorSet.backlightColor);
    }

    private void fadeOut() {
        if (fading) {
            this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
            transparency -= fadeRate;

            if (transparency <= 0.0f) {
                fading = false;
                this.transform.position = resetPosition;
            }
        }
    }

    private void fadeIn() {
        if (!fading && transparency < startTransparency) {
            this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
            transparency += fadeRate;
        }
    }

    public void AddDeformingForce (Vector3 point, float force, int index) {
        AddForceToVertex(index, point, force);
	}

	void AddForceToVertex (int i, Vector3 point, float force) {
        Vector3 forceDirection = this.collidingDirection;
		Vector3 pointToVertex = transform.TransformPoint(displacedVertices[i]) - point;
        float dist = (1f + Mathf.Pow(pointToVertex.magnitude, distancePower));
        float attenuatedForce = force / dist;
        float velocity = attenuatedForce * Time.deltaTime;

        vertexVelocities[i] += forceDirection.normalized * velocity;
	}

    void UpdateVertex (int i) {
		Vector3 velocity = vertexVelocities[i];
        // removing spring force for now, need to consider how clouds move over time
        // Vector3 displacement = displacedVertices[i] - originalVertices[i];
		// velocity -= displacement * springForce * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;
		vertexVelocities[i] = velocity;
		displacedVertices[i] += transform.InverseTransformDirection(velocity * Time.deltaTime);
	}

    void HandleCollision (Collider collider, Vector3 position, float force, int index) {
        Vector3 closest = collidingPosition;

        //returns true if the point is inside the collider
        // float dist = (closest.y - position.y);
        // if (dist < distanceThreshold) {
        this.AddDeformingForce(closest, force, index);
        // }
    }

    private void UpdateVertices() {
        
        for (int i = 0; i < displacedVertices.Length; i++) {
            this.HandleCollision(collider, displacedVertices[i], force, i);
			this.UpdateVertex(i);
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
        this.GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    }

    public void SetColliding(Boolean c, Vector3 position, Vector3 direction) {
        this.colliding = c;
        this.collidingPosition = position;
        this.collidingDirection = direction;
    }

    public void SetInteractive(Boolean isInteractive) {
        this.isInteractive = isInteractive;
    }
}
