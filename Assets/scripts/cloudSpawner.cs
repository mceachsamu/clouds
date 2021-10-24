using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudSpawner : MonoBehaviour
{

    public GameObject[] cloudPrefabs;
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

    private GameObject[] clouds;

    private weatherController weatherControll;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // set the material properties for each cloud
        for (int i = 0; i < clouds.Length; i++) {
            this.setCloudMaterialProperties(clouds[i]);
        }
    }

    public void instantiateClouds() {
        clouds = new GameObject[numClouds];

        for (int i = 0; i < numClouds; i++) {
            int prefabIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloud = Instantiate(cloudPrefabs[prefabIndex]);

            cloud.transform.position = this.generateCloudPosition();
            cloud.transform.localScale *= this.generateCloudScale();
            cloud.transform.forward = this.generateCloudRotation();

            int colorIndex = Random.Range(0, weatherControll.getCurrentColors().Length);

            cloud.AddComponent<cloud>();
            cloud.GetComponent<cloud>().baseColor = weatherControll.getCurrentColors()[colorIndex];
            cloud.GetComponent<cloud>().colorIndex = colorIndex;
            
            cloud.GetComponent<cloud>().cloudSpawner = this;
            cloud.GetComponent<cloud>().cloudMoveSpeed = this.generateCloudMoveSpeed();
            cloud.GetComponent<cloud>().fadeRate = fadeRate;
            clouds[i] = cloud;

            setCloudMaterialProperties(cloud);
        }
    }

    private void setCloudMaterialProperties(GameObject cloud) {
        cloud cloudS = cloud.GetComponent<cloud>();
        int index = cloudS.colorIndex;
        colorSet currentColorSet = this.weatherControll.getCurrentColorSet();

        cloud.GetComponent<Renderer>().material.SetVector("_Color", currentColorSet.colors[index]);
        cloud.GetComponent<Renderer>().material.SetFloat("_Glossiness", currentColorSet.cloudGloss);
        cloud.GetComponent<Renderer>().material.SetFloat("_RimAmount", currentColorSet.cloudRim);
        cloud.GetComponent<Renderer>().material.SetVector("_RimColor", currentColorSet.rimColor);
        cloud.GetComponent<Renderer>().material.SetVector("_SpecularColor", currentColorSet.glossColor);
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightStrength",currentColorSet.backlightStrength);
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightPower", currentColorSet.backlightPower);
        cloud.GetComponent<Renderer>().material.SetVector("_BacklightColor", currentColorSet.backlightColor);
    }

    public float generateCloudMoveSpeed() {
        return Random.Range(moveSpeedMin, moveSpeedMax);
    }

    private Vector3 generateCloudRotation() {
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
