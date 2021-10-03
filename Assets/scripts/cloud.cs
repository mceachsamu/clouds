using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    public cloud_spawner cloudSpawner;

    public float cloudMoveSpeed;

    private float transparency = 1.0f;

    public float fadeRate;
    
    private bool fading;
    private Vector3 resetPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        pos += cloudSpawner.cloudMoveDirection * cloudMoveSpeed;

        if (this.transform.position.x > cloudSpawner.rangeX/2.0f) {
            Vector3 newPos = pos;
            newPos.x = cloudSpawner.rangeX/-2.0f;
            resetPosition = newPos;
            fading = true;
        }
        if (this.transform.position.x < -cloudSpawner.rangeX/2.0f) {
            Vector3 newPos = pos;
            newPos.x = cloudSpawner.rangeX/2.0f;
            resetPosition = newPos;
            fading = true;
        }
        if (this.transform.position.z > cloudSpawner.rangeZ/2.0f) {
            Vector3 newPos = pos;
            newPos.z = cloudSpawner.rangeZ/-2.0f;
            resetPosition = newPos;
            fading = true;
        }
        if (this.transform.position.z < -cloudSpawner.rangeZ/2.0f) {
            Vector3 newPos = pos;
            newPos.z = cloudSpawner.rangeZ/2.0f;
            resetPosition = newPos;
            fading = true;
        }
        if (this.transform.position.y > cloudSpawner.rangeY/2.0f) {
            Vector3 newPos = pos;
            newPos.y = cloudSpawner.rangeY/-2.0f;
            resetPosition = newPos;
            fading = true;
        }
        if (this.transform.position.y < -cloudSpawner.rangeY/2.0f) {
            Vector3 newPos = pos;
            newPos.y = cloudSpawner.rangeY/2.0f;
            resetPosition = newPos;
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
            print(transparency);

            if (transparency <= 0.0f) {
                fading = false;
                print("here");
                this.transform.position = resetPosition;
            }
        }
    }

    private void fadeIn() {
        if (!fading && transparency < 1.0f) {
            this.GetComponent<Renderer>().material.SetFloat("_Transparency", transparency);
            transparency += fadeRate;
        }
    }
}
