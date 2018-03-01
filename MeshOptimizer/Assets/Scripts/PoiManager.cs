using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiManager : MonoBehaviour {
    [SerializeField]
    private Color32 TopLevelPoiColor;
    [SerializeField]
    private Color32 MinLevelPoiColor;
    [SerializeField]
    private int TopLevel = 0;
    [SerializeField]
    private int MinLevel = 0;
    [SerializeField]
    private List<PointOfInterest> TopLevelPoiList;
    [SerializeField]
    private bool showLevelBoundaries = true;

    private static PoiManager _instance = null;
    public static PoiManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PoiManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Debug.Log(TopLevelPoiList.Count);
        _instance = this;
        TopLevelPoiList = new List<PointOfInterest>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public Color32 GetColorForGivenPoiLevel(int level)
    {
        Color32 colorToReturn = MinLevelPoiColor;
        if(MinLevel != 0)
        {
            float ratio = (float)level / (float)MinLevel;
            colorToReturn = Color32.Lerp(TopLevelPoiColor, MinLevelPoiColor, ratio);
            TopLevelPoiColor.a = 255;
            MinLevelPoiColor.a = 255;
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
        if (level > MinLevel)
        {
            MinLevel = level;
        }
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

    public bool GetLevelBoundariesVisibility()
    {
        return showLevelBoundaries;
    }

    public void SetLevelBoundariesVisibility(bool v)
    {
        showLevelBoundaries = v;
    }
}
