using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorSetMonoBehaviour : MonoBehaviour
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

    public colorSet getColorSet() {
        colorSet c = new colorSet();
        c.colors = new Color[this.colors.Length];
        for (int i = 0; i < this.colors.Length; i++) {
            c.colors[i] = this.colors[i];
        }

        c.skyColor = this.skyColor;
        c.cloudMaterial = this.cloudMaterial;
        c.cloudGloss = this.cloudGloss;
        c.cloudRim = this.cloudRim;
        c.glossColor = this.glossColor;
        c.backlightStrength = this.backlightStrength;
        c.backlightPower = this.backlightPower;
        c.backlightColor = this.backlightColor;

        return c;
    }
}
