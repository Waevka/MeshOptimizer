using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {
    public GameObject model;
    public float xRotateSpeed;
    public float yRotateSpeed;
    public float zoomSpeed;
    public float panSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bool dirty = false;
        if (Input.GetButton("Fire1"))
        {
            float dx = xRotateSpeed * Input.GetAxis("Mouse Y");
            float dy = -yRotateSpeed * Input.GetAxis("Mouse X");
            model.transform.Rotate(dx, dy, 0, Space.World);
            dirty = true;
        }

        if (Input.GetButton("Fire3"))
        {
            float dx = panSpeed * Input.GetAxis("Mouse X");
            float dy = panSpeed * Input.GetAxis("Mouse Y");
            model.transform.Translate(dx, dy, 0, Space.World);
            dirty = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float dz = zoomSpeed * -Input.GetAxis("Mouse ScrollWheel");
            model.transform.Translate(0, 0, dz, Space.World);
            dirty = true;
        }

        if (dirty)
        {
            PoiManager.Instance.RecalculateBoundingBoxes();
        }
	}
}
