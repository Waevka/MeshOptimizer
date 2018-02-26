using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour {

    [SerializeField]
    private PointOfInterest ParentPoi = null;
    [SerializeField]
    private List<PointOfInterest> ChildrenPoi = null;
    [SerializeField]
    private int PoiLevel = 0;
    private bool Initialized = false; //TODO
    public bool IsRootPOI = false;
    public float MinimumCameraDistance = 20.0f;
    public float MaximumCameraDistanceDelta = 3.5f;
    public bool active = false;

    public GameObject DebugMarkerPrefab;

    [SerializeField]
    private Vector3 BoundingBoxCenter;
    [SerializeField]
    public Bounds BoundingBox;
    private GameObject DebugMarker = null;

    [SerializeField]
    private MeshRenderer MeshRendererPoi;
    [SerializeField]
    private Color PoiColor;

    private void Awake()
    {
        //MeshRendererPoi = gameObject.GetComponent<MeshRenderer>();

        //Get all Poi on level below current Poi
        List<PointOfInterest> AllChildrenPoi = new List<PointOfInterest>(gameObject.GetComponentsInChildren<PointOfInterest>());
        ChildrenPoi = new List<PointOfInterest>();

        foreach (PointOfInterest poi in AllChildrenPoi)
        {
            if (poi.transform.parent == this.transform)
            {
                ChildrenPoi.Add(poi);
            }
        }

        if (ChildrenPoi != null || ChildrenPoi.Contains(this))
        {
            //Remove current Poi from list
            ChildrenPoi.Remove(this);
        }

        //Get parent Poi
        if (transform.parent != null)
        {
            ParentPoi = transform.parent.GetComponent<PointOfInterest>();
        }

        Initialized = true;
    }

    // Use this for initialization
    void Start () {
        //Get dot model
        if (IsRootPOI == true)
        {
            //Assign Poi level when creating the topmost object
            EstabilishPoiLevelForSelfAndChildren(0);
            active = true;
            PoiManager.Instance.RegisterTopLevelPoi(this);
            //SetVisibilityOfAllChildrenPoi(false, true);
        }
        CalculateBoundingBox(false);
    }

    // Update is called once per frame
    void Update () {
        //if(active) CheckCameraDistance();
    }

    public void PointOfInterestEnabled(bool enabled)
    {
        MeshRendererPoi.enabled = enabled;
        active = enabled;
    }

    public int GetPoiLevel()
    {
        return PoiLevel;
    }

    public PointOfInterest GetParentPoi()
    {
        return ParentPoi;
    }

    public void EstabilishPoiLevelForSelfAndChildren(int LevelForCurrentPoi)
    {
        PoiLevel = LevelForCurrentPoi;
        //SetColor(PoiManager.Instance.GetColorForGivenPoiLevel(PoiLevel));
        foreach (PointOfInterest poi in ChildrenPoi)
        {
            poi.EstabilishPoiLevelForSelfAndChildren(LevelForCurrentPoi + 1);
        }
    }
    
    private void CheckCameraDistance()
    {
        var distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        //if (ParentPoi == null) Debug.Log(distance);

        if (distance <= MinimumCameraDistance)
        {
            PointOfInterestEnabled(false);
            SetVisibilityOfAllChildrenPoi(true, false);
            if(ParentPoi != null)
            {
                ParentPoi.SetVisibilityOfAllSiblingPoi(false, true, this);
            }
        }

        if (distance >= MinimumCameraDistance + MaximumCameraDistanceDelta)
        {
            SetVisibilityOfAllChildrenPoi(false, true);
            if (ParentPoi != null)
            {
                ParentPoi.PointOfInterestEnabled(true);
                if(ParentPoi.GetParentPoi() != null)
                {
                    ParentPoi.GetParentPoi().SetVisibilityOfAllSiblingPoi(true, true, ParentPoi);
                }
            }
        }
    }

    public void SetVisibilityOfAllChildrenPoi(bool visible, bool recursive)
    {
        foreach(PointOfInterest poi in ChildrenPoi)
        {
            poi.PointOfInterestEnabled(visible);
            if (recursive)
            {
                poi.SetVisibilityOfAllChildrenPoi(visible, recursive);
            }
        }
    }

    public void SetVisibilityOfAllSiblingPoi(bool visible, bool recursive, PointOfInterest p)
    {
        foreach (PointOfInterest poi in ChildrenPoi)
        {
            if (!poi.Equals(p))
            {
                poi.PointOfInterestEnabled(visible);

                if (recursive)
                {
                    poi.SetVisibilityOfAllChildrenPoi(visible, recursive);
                }
            }
        }
    }

    public void CalculateBoundingBox(bool recursive)
    {
        CalculateBoundingBoxCenter();
        CalculateBoundingBoxSize();
        if (recursive)
        {
            foreach (PointOfInterest poi in ChildrenPoi)
            {
                poi.CalculateBoundingBox(true);
            }
        }
    }

    private void CalculateBoundingBoxCenter()
    {
        Vector3 center = Vector3.zero;
        if (ChildrenPoi.Count == 0)
        {
            center = Vector3.zero;
        }
        else
        {
            foreach (PointOfInterest poi in ChildrenPoi)
            {
                center += poi.transform.localPosition;
            }
            center /= ChildrenPoi.Count;
        }
        if(DebugMarker == null)
        {
            DebugMarker = Instantiate(DebugMarkerPrefab, transform, false);
        }
        DebugMarker.transform.localPosition = center;

        BoundingBoxCenter = DebugMarker.transform.position;
        BoundingBox = new Bounds(BoundingBoxCenter, Vector3.one);
    }

    private void CalculateBoundingBoxSize()
    {
        //TODO: recursive
        BoundingBox.Encapsulate(transform.position);
        foreach (PointOfInterest poi in ChildrenPoi)
        {
            BoundingBox.Encapsulate(poi.transform.position);
        }
    }

    private void SetColor(Color c)
    {
        PoiColor = c;
        MeshRendererPoi.material.color = PoiColor;
    }
}
