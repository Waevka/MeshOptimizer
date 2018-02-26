using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentModelSelector : MonoBehaviour {
    [SerializeField]
    GameObject[] models;
    int currentModel = 0;
	// Use this for initialization
	void Start () {
        UpdateVisibleModel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNewModelIndex(float i)
    {
        currentModel = (int)i - 1;
        UpdateVisibleModel();
    }

    void UpdateVisibleModel()
    {
        for(int i = 0; i < models.Length; i++)
        {
            if(i == currentModel)
            {
                models[i].SetActive(true);
            } else
            {
                models[i].SetActive(false);
            }
        }
    }
}
