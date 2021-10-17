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

    public GameObject[] colors;

    public Material skyboxMaterial;

    public GameObject camera;

    public GameObject plane;

    private GameObject[] clouds;

    private float change = 1.0f;
    public float changeRate;
    private GameObject previousColorSet;
    private GameObject currentColorSet;

    // Start is called before the first frame update
    void Start()
    {
        this.setColorSet(0);
        this.previousColorSet = this.currentColorSet;

        Application.targetFrameRate = 60;
        clouds = new GameObject[numClouds];
        instantiateClouds();
    }

    // Update is called once per frame
    void Update()
    {
        this.center = plane.transform.position;
        this.setSkyboxColor(getCurrentColorSet().skyColor);
        this.handleColorSetTransition();

        if (Input.GetKeyDown("1"))
        {
            this.setColorSet(0);
            this.startColorTransition();
        }
        if (Input.GetKeyDown("2"))
        {
            this.setColorSet(1);
            this.startColorTransition();
        }
        if (Input.GetKeyDown("3"))
        {
            this.setColorSet(2);
            this.startColorTransition();
        }
        if (Input.GetKeyDown("4"))
        {
            this.setColorSet(3);
            this.startColorTransition();
        }

        for (int i = 0; i < clouds.Length; i++) {
            this.setCloudMaterialProperties(clouds[i]);
        }
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
        Color target = transitionColor(getCurrentColorSet().colors[colorIndex], getPreviousColorSet().colors[colorIndex]);

        cloud.GetComponent<Renderer>().material.SetVector("_Color", target);
        cloud.GetComponent<Renderer>().material.SetFloat("_Glossiness", transitionFloat(getCurrentColorSet().cloudGloss, this.getPreviousColorSet().cloudGloss));
        cloud.GetComponent<Renderer>().material.SetFloat("_RimAmount", transitionFloat(getCurrentColorSet().cloudRim, this.getPreviousColorSet().cloudRim));
        cloud.GetComponent<Renderer>().material.SetVector("_RimColor", transitionColor(getCurrentColorSet().rimColor, this.getPreviousColorSet().rimColor));
        cloud.GetComponent<Renderer>().material.SetVector("_SpecularColor", transitionColor(getCurrentColorSet().glossColor, this.getPreviousColorSet().glossColor));
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightStrength",transitionFloat(getCurrentColorSet().backlightStrength, this.getPreviousColorSet().backlightStrength));
        cloud.GetComponent<Renderer>().material.SetFloat("_BackLightPower", transitionFloat(getCurrentColorSet().backlightPower, this.getPreviousColorSet().backlightPower));
        cloud.GetComponent<Renderer>().material.SetVector("_BacklightColor", transitionColor(getCurrentColorSet().backlightColor, this.getPreviousColorSet().backlightColor));
    }

    public void handleColorSetTransition() {
        if (this.change > 1.0f) {
            this.previousColorSet = this.currentColorSet;
            return;
        }

        this.change += changeRate;
    }

    public void startColorTransition() {
        this.change = 0.0f;
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
        
        return this.currentColorSet.GetComponent<colorSet>();
    }   
    
    private colorSet getPreviousColorSet() {
        
        return this.previousColorSet.GetComponent<colorSet>();
    }

    private void setColorSet(int i) {
        this.currentColorSet = colors[i];
    }

    private void setSkyboxColor(Color c) {
        skyboxMaterial.SetColor("_Tint", transitionColor(getCurrentColorSet().skyColor, getPreviousColorSet().skyColor));
    }

    private float transitionFloat(float f1, float f2) {
        float prop = change / 1.0f;

        float c = f2 * (1.0f - prop) + (prop) * f1;
        return c;
    }

    private Color transitionColor(Color c1, Color c2) {
        float prop = change / 1.0f;
        Color c = c2 * (1.0f - prop) + (prop) * c1;
        return c;
    }
}
