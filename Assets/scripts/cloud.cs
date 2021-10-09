using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    public cloud_spawner cloudSpawner;

    public float cloudMoveSpeed;

    private static float startTransparency = 1.0f;

    private float transparency = startTransparency;

    public float fadeRate;
    
    private bool fading;
    private Vector3 resetPosition;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency); 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        pos += cloudSpawner.cloudMoveDirection * cloudMoveSpeed;

        if (this.transform.position.x > cloudSpawner.center.x + cloudSpawner.getRangeX()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }
        if (this.transform.position.x < cloudSpawner.center.x -cloudSpawner.getRangeX()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }
        if (this.transform.position.z > cloudSpawner.center.z + cloudSpawner.getRangeZ()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }
        if (this.transform.position.z < cloudSpawner.center.z - cloudSpawner.getRangeZ()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }
        if (this.transform.position.y > cloudSpawner.center.y + cloudSpawner.getRangeY()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }
        if (this.transform.position.y < cloudSpawner.center.y -cloudSpawner.getRangeY()/2.0f) {
            resetPosition = cloudSpawner.getPosition();
            fading = true;
        }

        this.transform.position = pos; 
        fadeOut();
        fadeIn();
    }

    private void fadeOut() {
        if (fading) {
            this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
            transparency -= fadeRate;

            if (transparency <= 0.0f) {
                fading = false;
                this.transform.position = resetPosition;
            }
        }
    }

    private void fadeIn() {
        if (!fading && transparency < startTransparency) {
            this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
            transparency += fadeRate;
        }
    }
}
