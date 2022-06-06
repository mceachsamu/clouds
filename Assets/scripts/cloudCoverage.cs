using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudCoverage : MonoBehaviour
{
    private weatherController WeatherController;

    private Color baseColor;

    public int colorIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        WeatherController = GameObject.FindGameObjectsWithTag("weatherController")[0].GetComponent<weatherController>();
    }

    // Update is called once per frame
    void Update()
    {
        this.setCloudMaterialProperties();
    }

    private void setCloudMaterialProperties() {
        int index = this.colorIndex;
        colorSet currentColorSet = WeatherController.getCurrentColorSet();

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
}
