using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudSpawner : MonoBehaviour
{

    public GameObject[] cloudPrefabs;
    public Vector3 minDirection;
    public Vector3 maxDirection;

    public DirectionType cloudDirectionType;

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

    private GameObject[] clouds;

    private weatherController weatherControll;

    public Texture2D DistributionHorizontal;

    public enum DirectionType {
        linear,
        radial
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.center = weatherControll.center;
    }

    public void instantiateClouds() {
        clouds = new GameObject[numClouds];

        for (int i = 0; i < numClouds; i++) {
            int prefabIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloud = Instantiate(cloudPrefabs[prefabIndex]);

            cloud.transform.position = this.generateCloudPosition();
            cloud.transform.localScale *= this.generateCloudScale();
            cloud.transform.right = this.generateCloudRotation(cloud.transform.position);

            int colorIndex = Random.Range(0, weatherControll.getCurrentColors().Length);

            cloud.GetComponent<cloud>().baseColor = weatherControll.getCurrentColors()[colorIndex];
            cloud.GetComponent<cloud>().colorIndex = colorIndex;
            
            cloud.GetComponent<cloud>().cloudSpawner = this;
            cloud.GetComponent<cloud>().cloudMoveSpeed = this.generateCloudMoveSpeed();
            cloud.GetComponent<cloud>().fadeRate = fadeRate;
            clouds[i] = cloud;
        }
    }

    public weatherController GetWeatherController() {
        return this.weatherControll;
    }

    public float generateCloudMoveSpeed() {
        return Random.Range(moveSpeedMin, moveSpeedMax);
    }

    private Vector3 generateCloudRotation(Vector3 pos) {
        if (cloudDirectionType == DirectionType.linear) {
            return getLinearRotation();
        }

        return getRadialRotation(pos);
    }

    private Vector3 getRadialRotation(Vector3 pos) {
        GameObject plane = GameObject.FindGameObjectsWithTag("Plane")[0];
        Vector3 planePos = new Vector3(plane.transform.position.x, pos.y, plane.transform.position.z);

        Vector3 directionToPlane = (planePos - pos).normalized;
        Vector3 radialDirection = new Vector3(directionToPlane.x, directionToPlane.y, directionToPlane.z);

        return radialDirection;
    }

    private Vector3 getLinearRotation() {
        float x = Random.Range(minDirection.x, maxDirection.x);
        float y = Random.Range(minDirection.y, maxDirection.y);
        float z = Random.Range(minDirection.z, maxDirection.z);

        return new Vector3(x, y, z);
    }

    private float generateCloudScale() {
        return Random.Range(minScale, maxScale);
    }

    public Vector3 generateCloudPosition() {
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

    public GameObject[] GetClouds() {
        return clouds;
    }

    public void SetWeatherController(weatherController weatherControll) {
        this.weatherControll = weatherControll;
    }
}
