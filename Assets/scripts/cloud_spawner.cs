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

    public GameObject[] colorObjects;
    public colorSet[] colors;

    public Material skyboxMaterial;

    public GameObject camera;

    public GameObject plane;

    private GameObject[] clouds;

    private colorSet currentColorSet;

    public GameObject directionalLight;

    public float lightDirectionChangeRate;

    // Start is called before the first frame update
    void Start()
    {
        this.loadColors();
        Application.targetFrameRate = 60;
        clouds = new GameObject[numClouds];
        instantiateClouds();
    }

    private void loadColors() {
        colors = new colorSet[colorObjects.Length];
        for (int i = 0; i < colorObjects.Length; i++) {
            colors[i] = colorObjects[i].GetComponent<colorSetMonoBehaviour>().getColorSet();
        }

        // set the initial color
        colors[2].currentMagnitude = 1.0f;
        currentColorSet = new colorSet();
        currentColorSet.SetNewColorSet(colors[2]);
    }

    // Update is called once per frame
    void Update()
    {
        this.center = plane.transform.position;

        this.currentColorSet.SetNewColorSet(this.calculateCloudSetSum());

        this.setSkyboxColor(getCurrentColorSet().skyColor);

        for (int i = 0; i < clouds.Length; i++) {
            this.setCloudMaterialProperties(clouds[i]);
        }

        directionalLight.transform.Rotate(0.0f, lightDirectionChangeRate * Time.deltaTime, 0.0f);
        float sunDown = (directionalLight.transform.forward.y);
        if (sunDown < 0.0f) {
            sunDown = 0.0f;
        } 

        float propDay = 1.0f - sunDown;
        float propNight = sunDown;
        this.colors[2].currentMagnitude = propDay;
        this.colors[1].currentMagnitude = propNight;
    }

    public GameObject[] instantiateClouds() {
        for (int i = 0; i < numClouds; i++) {
            int prefabIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloud = Instantiate(cloudPrefabs[prefabIndex]);

            cloud.transform.position = getPosition();
            cloud.transform.localScale *= getScale();
            cloud.transform.forward = getRotation();

            int colorIndex = Random.Range(0, getCurrentColors().Length);

            cloud.AddComponent<cloud>();
            cloud.GetComponent<cloud>().baseColor = getCurrentColors()[colorIndex];
            cloud.GetComponent<cloud>().colorIndex = colorIndex;
            
            cloud.GetComponent<cloud>().cloudSpawner = this;
            cloud.GetComponent<cloud>().cloudMoveSpeed = GetCloudMoveSpeed();
            cloud.GetComponent<cloud>().fadeRate = fadeRate;
            clouds[i] = cloud;

            setCloudMaterialProperties(cloud);
        }

        return null;
    }

    private void setCloudMaterialProperties(GameObject cloud) {
        int colorIndex = cloud.GetComponent<cloud>().colorIndex;
        Color target = getCurrentColorSet().colors[colorIndex];

        cloud.GetComponent<Renderer>().material.SetVector("_Color", target);
        cloud.GetComponent<Renderer>().material.SetFloat("_Glossiness", getCurrentColorSet().cloudGloss);
        cloud.GetComponent<Renderer>().material.SetFloat("_RimAmount", getCurrentColorSet().cloudRim);
        cloud.GetComponent<Renderer>().material.SetVector("_RimColor", getCurrentColorSet().rimColor);
        cloud.GetComponent<Renderer>().material.SetVector("_SpecularColor", getCurrentColorSet().glossColor);
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightStrength",getCurrentColorSet().backlightStrength);
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightPower", getCurrentColorSet().backlightPower);
        cloud.GetComponent<Renderer>().material.SetVector("_BacklightColor", getCurrentColorSet().backlightColor);
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

    private Color[] getCurrentColors() {
        return getCurrentColorSet().colors;
    }

    private colorSet getCurrentColorSet() {
        
        return this.currentColorSet;
    }   

    private void setSkyboxColor(Color c) {
        skyboxMaterial.SetColor("_Tint", getCurrentColorSet().skyColor);
    }

    private colorSet calculateCloudSetSum() {
        colorSet result = new colorSet();
        
        // assume that we have atleast one set already, and every set has the same number of colors
        result.colors = new Color[this.colors[0].colors.Length];
        result.cloudMaterial = this.colors[0].cloudMaterial;

        for (int i = 0; i < colors.Length; i++) {
            colors[i].addToSetWithMagnitude(result);
        }

        print(this.colors[0].colors[0]);

        return result;
    }
}
