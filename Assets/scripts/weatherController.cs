using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weatherController : MonoBehaviour
{

    public GameObject plane;

    public GameObject[] colorObjects;
    private colorSet[] colors;
    public Material skyboxMaterial;
    private colorSet currentColorSet;
    public float lightDirectionChangeRate;
    public GameObject directionalLight;

    public GameObject[] cloudSpawnerObjects;

    private cloudSpawner[] cloudSpawners;

    private Vector3 center;

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
        colors[2].currentMagnitude = 1.0f;
        currentColorSet = new colorSet();
        currentColorSet.SetNewColorSet(colors[2]);
    }

    // Update is called once per frame
    void Update()
    {
        this.currentColorSet.SetNewColorSet(this.calculateCloudSetSum());
        this.setSkyboxColor(getCurrentColorSet().skyColor);

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
