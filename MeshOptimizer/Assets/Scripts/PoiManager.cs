using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiManager : MonoBehaviour {
    [SerializeField]
    private Color TopLevelPoiColor;
    [SerializeField]
    private Color MinLevelPoiColor;
    [SerializeField]
    private int TopLevel = 0;
    [SerializeField]
    private int MinLevel;
    [SerializeField]
    private List<PointOfInterest> TopLevelPoiList;

    private static PoiManager _instance;
    public static PoiManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject g = new GameObject("PoiManager");
                g.AddComponent<PoiManager>();
                _instance = g.GetComponent<PoiManager>();

            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        TopLevelPoiList = new List<PointOfInterest>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Color GetColorForGivenPoiLevel(int level)
    {
        Color colorToReturn = MinLevelPoiColor;
        if(MinLevel != 0)
        {
            colorToReturn = Color.Lerp(TopLevelPoiColor, MinLevelPoiColor, level / MinLevel);
        }
        return colorToReturn;
    }

    public int GetTopLevel()
    {
        return TopLevel;
    }

    public void SetTopLevel(int level)
    {
        TopLevel = level;
    }

    public int GetMinLevel()
    {
        return MinLevel;
    }

    public void SetMinLevel(int level)
    {
        MinLevel = level;
    }

    public void RegisterTopLevelPoi(PointOfInterest poi)
    {
        TopLevelPoiList.Add(poi);
    }

    public void RecalculateBoundingBoxes()
    {
        foreach(PointOfInterest poi in TopLevelPoiList)
        {
            poi.CalculateBoundingBox(true);
        }
    }
}
