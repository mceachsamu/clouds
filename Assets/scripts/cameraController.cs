using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject plane;

    public float offsetZ;
    public float offsetY;

    // Start is called before the first frame update
    void Start()
    {
        // offset = this.transform.position - plane.tranform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startDir = this.transform.forward;
        this.transform.position = plane.transform.position;

        Vector3 planeForward = plane.GetComponent<planeController>().getForward();
        this.transform.forward = planeForward;
        
        Vector3 planeUp = plane.GetComponent<planeController>().getUp();
        Vector3 pos = this.transform.position - planeForward * offsetZ + planeUp * offsetY;
        this.transform.position = pos;

        Vector3 cameraForward = new Vector3(planeForward.x, planeForward.y, this.transform.forward.z);
        this.transform.forward = cameraForward;
    }
}
