using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weatherController : MonoBehaviour
{

    public GameObject plane;

    public Material planeMaterial;

    public GameObject[] colorObjects;
    private colorSet[] colors;
    public Material skyboxMaterial;
    private colorSet currentColorSet;
    public float lightDirectionChangeRate;
    public GameObject directionalLight;

    public GameObject[] cloudSpawnerObjects;

    private cloudSpawner[] cloudSpawners;

    private GameObject[] land;

    public Vector3 center;

    // Start is called before the first frame update
    void Start()
    {  
        Application.targetFrameRate = 60;
        this.center = plane.transform.position;
        this.loadColors();
        this.loadCloudSpawners();
        for (int i = 0; i < cloudSpawners.Length; i++) {
            cloudSpawners[i].instantiateClouds();
        }

        //load in all the land objects
        land = GameObject.FindGameObjectsWithTag("land");
    }

    private void loadCloudSpawners() {
        cloudSpawners = new cloudSpawner[cloudSpawnerObjects.Length];
        for (int i = 0; i < cloudSpawnerObjects.Length; i++) {
            cloudSpawners[i] = cloudSpawnerObjects[i].GetComponent<cloudSpawner>();
            cloudSpawners[i].SetWeatherController(this);
        }
    }

    private void loadColors() {
        colors = new colorSet[colorObjects.Length];
        for (int i = 0; i < colorObjects.Length; i++) {
            colors[i] = colorObjects[i].GetComponent<colorSetMonoBehaviour>().getColorSet();
        }

        // set the initial color
        colors[0].currentMagnitude = 1.0f;
        currentColorSet = new colorSet();
        currentColorSet.SetNewColorSet(colors[2]);
    }

    // Update is called once per frame
    void Update()
    {
        this.currentColorSet.SetNewColorSet(this.calculateCloudSetSum());
        this.setSkyboxColor(getCurrentColorSet().skyColor);
        this.setPlaneColor();
        this.setLandColor();

        directionalLight.transform.Rotate(lightDirectionChangeRate * Time.deltaTime, 0.0f, 0.0f);
        float sunDown = (directionalLight.transform.forward.y) + 0.5f;
        if (sunDown < 0.0f) {
            sunDown = 0.0f;
        }

        float propDay = Mathf.Clamp(1.0f - sunDown, 0.0f, 1.0f);
        float propNight = Mathf.Clamp(sunDown, 0.0f, 1.0f);
        this.colors[0].currentMagnitude = propDay;
        this.colors[1].currentMagnitude = propNight;

        setSkyboxRotation();

        this.center = plane.transform.position;
    }

    private void setPlaneColor() {
        planeMaterial.SetVector("_Color", this.getCurrentColorSet().planeColor);
        planeMaterial.SetVector("_BacklightColor", this.getCurrentColorSet().planeBacklightColor);
    }

    private void setLandColor() {
        for (int i = 0; i < land.Length; i++) {
            land[i].GetComponent<Renderer>().material.SetColor("_Color", this.getCurrentColorSet().landColor);
        }
    }

    public Vector3 getCenter() {
        return plane.transform.position;
    }

    public Color[] getCurrentColors() {
        return getCurrentColorSet().colors;
    }

    public colorSet getCurrentColorSet() {
        return this.currentColorSet;
    }

    private void setSkyboxColor(Color c) {
        skyboxMaterial.SetColor("_Tint", getCurrentColorSet().skyColor);
    }

    private void setSkyboxRotation() {
        Vector3 rotation = directionalLight.transform.rotation.eulerAngles;

        skyboxMaterial.SetVector("_Rotation", rotation);
    }

    private colorSet calculateCloudSetSum() {
        colorSet result = new colorSet();
        
        // assume that we have atleast one set already, and every set has the same number of colors
        result.colors = new Color[this.colors[0].colors.Length];
        result.cloudMaterial = this.colors[0].cloudMaterial;

        for (int i = 0; i < colors.Length; i++) {
            colors[i].addToSetWithMagnitude(result);
        }

        return result;
    }
}
