using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorSet
{
    public Color[] colors;

    public Color skyColor;

    public Material cloudMaterial;

    public float cloudGloss;

    public float cloudRim;

    public Color rimColor;
    public Color glossColor;
    public float backlightStrength;
    public float backlightPower;
    public Color backlightColor;

    public float currentMagnitude = 0.0f;

    public Color planeColor;
    public Color planeBacklightColor;

    public Color landColor;

    public void addToSetWithMagnitude(colorSet addTo) {

        for (int i = 0; i < colors.Length; i++) {
            addTo.colors[i] += this.colors[i] * currentMagnitude;
        }

        addTo.skyColor += this.skyColor * currentMagnitude;
        addTo.cloudGloss += this.cloudGloss * currentMagnitude;
        addTo.cloudRim += this.cloudRim * currentMagnitude;
        addTo.glossColor += this.glossColor * currentMagnitude;
        addTo.backlightStrength += this.backlightStrength * currentMagnitude;
        addTo.backlightPower += this.backlightPower * currentMagnitude;
        addTo.backlightColor += this.backlightColor * currentMagnitude;
        addTo.planeColor += this.planeColor * currentMagnitude;
        addTo.planeBacklightColor += this.planeBacklightColor * currentMagnitude;
        addTo.landColor += this.landColor * currentMagnitude;
    }

    public void SetNewColorSet(colorSet c) {
        this.colors = new Color[c.colors.Length];
        for (int i = 0; i < this.colors.Length; i++) {
            this.colors[i] = c.colors[i];
        }

        this.skyColor = c.skyColor;
        this.cloudMaterial = c.cloudMaterial;
        this.cloudGloss = c.cloudGloss;
        this.cloudRim = c.cloudRim;
        this.glossColor = c.glossColor;
        this.backlightStrength = c.backlightStrength;
        this.backlightPower = c.backlightPower;
        this.backlightColor = c.backlightColor;
        this.planeColor = c.planeColor;
        this.planeBacklightColor = c.planeBacklightColor;
        this.landColor = c.landColor;
    }
}
