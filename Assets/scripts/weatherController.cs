using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weatherController : MonoBehaviour
{

    public GameObject plane;

    public Material planeMaterial;

    public GameObject[] colorObjects;
    public Material skyboxMaterial;
    public GameObject directionalLight;

    private colorSet currentColorSet;
    private colorSet[] colors;

    private cloudSpawner[] cloudSpawners;

    private GameObject[] land;

    public int startColor;

    public ColorChangeMode mode;
    public int nightColor;
    public float lightDirectionChangeRate;

    private bool colorIsChanging = false;
    private float currentColorProportion = 0.0f;
    private int currentColorIndex;
    private int goalColorSetIndex;
    public float colorChangeRate;
    

    public Vector3 center;

    public enum ColorChangeMode {
        DayCycle,
        InputCycle
    }

    // Start is called before the first frame update
    void Start()
    {  
        Application.targetFrameRate = 60;
        this.center = plane.transform.position;
        currentColorIndex = startColor;

        this.loadColors();
        this.loadCloudSpawners();
        for (int i = 0; i < cloudSpawners.Length; i++) {
            cloudSpawners[i].instantiateClouds();
        }

        //load in all the land objects
        land = GameObject.FindGameObjectsWithTag("land");
    }

    // Update is called once per frame
    void Update()
    {
        this.currentColorSet.SetNewColorSet(this.calculateCloudSetSum());
        this.setSkyboxColor(getCurrentColorSet().skyColor);
        this.setPlaneColor(this.getCurrentColorSet().planeColor, this.getCurrentColorSet().planeBacklightColor);
        this.setLandColor(this.getCurrentColorSet().landColor);

        switch (mode) {
            case ColorChangeMode.DayCycle:
                // change the colors over the day
                this.DayCycle();
                break;
            case ColorChangeMode.InputCycle:
                this.InputCycle();
                break;
        }

        this.center = plane.transform.position;
    }

    private void loadCloudSpawners() {
        GameObject[] cloudSpawnerObjects = GameObject.FindGameObjectsWithTag("cloud-spawner");
        
        cloudSpawners = new cloudSpawner[cloudSpawnerObjects.Length];
        for (int i = 0; i < cloudSpawnerObjects.Length; i++) {
            cloudSpawners[i] = cloudSpawnerObjects[i].GetComponent<cloudSpawner>();
            cloudSpawners[i].SetWeatherController(this);
        }
    }

    // loads colors from the monobehaviour scripts into non mono behaviour
    // the reason we want to do this is so that we can still modify the colors
    // in the gameobject window.
    private void loadColors() {
        colors = new colorSet[colorObjects.Length];
        for (int i = 0; i < colorObjects.Length; i++) {
            colors[i] = colorObjects[i].GetComponent<colorSetMonoBehaviour>().getColorSet();
        }

        // set the initial color
        colors[startColor].currentMagnitude = 1.0f;
        currentColorSet = new colorSet();
        currentColorSet.SetNewColorSet(colors[startColor]);
    }

    // InputCycle changes the colors based on user inputs
    private void InputCycle() {
        //map the indexes of the colors to input keys - this wont work if we have more than 9 color sets
        if (!colorIsChanging) {
            for (int i = 0; i < this.colors.Length; i++) {
                if (Input.GetKeyDown("" + (i + 1))) {
                    colorIsChanging = true;
                    
                    goalColorSetIndex = i;
                }
            }
        }

        if (colorIsChanging) {
            if (currentColorProportion < 1.0f) {
                currentColorProportion += colorChangeRate;
            }
            if (currentColorProportion >= 1.0f) {
                currentColorIndex = goalColorSetIndex;
                currentColorProportion = 0.0f;
                colorIsChanging = false;
            }
        }


        float propCurrent = Mathf.Clamp(1.0f - currentColorProportion, 0.0f, 1.0f);
        float propNext = Mathf.Clamp(currentColorProportion, 0.0f, 1.0f);
        
        this.colors[goalColorSetIndex].currentMagnitude = propNext;
        this.colors[currentColorIndex].currentMagnitude = propCurrent;

        print(currentColorIndex);
    }

    // DayCycle changes the colors between two values over a day
    private void DayCycle() {
        directionalLight.transform.Rotate(lightDirectionChangeRate * Time.deltaTime, 0.0f, 0.0f);
        float sunDown = (directionalLight.transform.forward.y);
        if (sunDown < 0.0f) {
            sunDown = 0.0f;
        }

        float propDay = Mathf.Clamp(1.0f - sunDown, 0.0f, 1.0f);
        float propNight = Mathf.Clamp(sunDown, 0.0f, 1.0f);
        
        this.colors[startColor].currentMagnitude = propDay;
        this.colors[nightColor].currentMagnitude = propNight;

        this.setSkyboxRotation();
    }

    private void setPlaneColor(Color planeColor, Color planeBacklightColor) {
        planeMaterial.SetVector("_Color", planeColor);
        planeMaterial.SetVector("_BacklightColor", planeBacklightColor);
    }

    private void setLandColor(Color landColor) {
        for (int i = 0; i < land.Length; i++) {
            land[i].GetComponent<Renderer>().material.SetColor("_Color", landColor);
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
