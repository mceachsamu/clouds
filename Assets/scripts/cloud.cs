using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    public cloud_spawner cloudSpawner;

    public float cloudMoveSpeed;

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
            pos.x = cloudSpawner.rangeX/-2.0f;
        }
        if (this.transform.position.x < -cloudSpawner.rangeX/2.0f) {
            pos.x = cloudSpawner.rangeX/2.0f;
        }
        if (this.transform.position.z > cloudSpawner.rangeZ/2.0f) {
            pos.z = cloudSpawner.rangeZ/-2.0f;
        }
        if (this.transform.position.z < -cloudSpawner.rangeZ/2.0f) {
            pos.z = cloudSpawner.rangeZ/2.0f;
        }
        if (this.transform.position.y > cloudSpawner.rangeY/2.0f) {
            pos.y = cloudSpawner.rangeY/-2.0f;
        }
        if (this.transform.position.y < -cloudSpawner.rangeY/2.0f) {
            pos.y = cloudSpawner.rangeY/2.0f;
        }

        this.transform.position = pos; 
    }
}
