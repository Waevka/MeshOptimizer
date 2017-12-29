using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {
    public GameObject model;
    public float xRotateSpeed;
    public float yRotateSpeed;
    public float zoomSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            float dx = xRotateSpeed * Input.GetAxis("Mouse Y");
            float dy = -yRotateSpeed * Input.GetAxis("Mouse X");
            model.transform.Rotate(dx, dy, 0);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float dz = zoomSpeed * Input.GetAxis("Mouse ScrollWheel");
            model.transform.Translate(0, 0, dz);
        }
	}
}
