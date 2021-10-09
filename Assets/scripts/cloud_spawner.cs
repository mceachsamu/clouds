using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud_spawner : MonoBehaviour
{

    public GameObject[] cloudPrefabs;
    public Material[] materials;

    public Vector3 minDirection;
    public Vector3 maxDirection;

    public float maxDistance;

    public float minDistance;

    public float minScale;

    public float maxScale;

    public float rangeX;
    public float rangeY;
    public float rangeZ;

    public int numClouds;

    public float moveSpeedMax;
    public float moveSpeedMin;
    public Vector3 cloudMoveDirection;

    public float fadeRate;

    public Vector3 center;

    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        instantiateClouds();
    }

    // Update is called once per frame
    void Update()
    {
        this.center = camera.transform.position;
    }

    public GameObject[] instantiateClouds() {
        for (int i = 0; i < numClouds; i++) {
            int prefabIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloud = Instantiate(cloudPrefabs[prefabIndex]);

            cloud.transform.position = getPosition();

            cloud.transform.localScale *= getScale();

            cloud.transform.forward = getRotation();

            int meterialIndex = Random.Range(0, materials.Length);

            cloud.GetComponent<Renderer>().material = materials[meterialIndex];

            cloud.AddComponent<cloud>();
            cloud.GetComponent<cloud>().cloudSpawner = this;
            cloud.GetComponent<cloud>().cloudMoveSpeed = GetCloudMoveSpeed();
            cloud.GetComponent<cloud>().fadeRate = fadeRate;
        }

        return null;
    }

    public float GetCloudMoveSpeed() {
        return Random.Range(moveSpeedMin, moveSpeedMax);
    }

    public Vector3 getRotation() {
        float x = Random.Range(minDirection.x, maxDirection.x);
        float y = Random.Range(minDirection.y, maxDirection.y);
        float z = Random.Range(minDirection.z, maxDirection.z);

        return new Vector3(x, y, z);
    }

    public float getScale() {
        return Random.Range(minScale, maxScale);
    }

    public Vector3 getPosition() {
        float xRange = Random.Range(center.x - rangeX/2.0f, center.x + rangeX/2.0f);
        float yRange = Random.Range(center.y - rangeY/2.0f, center.y + rangeY/2.0f);
        float zRange = Random.Range(center.z - rangeZ/2.0f, center.z + rangeZ/2.0f);

        return new Vector3(xRange,yRange,zRange);
    }

    public float getRangeX() {
        return this.rangeX;
    }

    public float getRangeY() {
        return this.rangeY;
    }

    public float getRangeZ() {
        return this.rangeZ;
    }

}
