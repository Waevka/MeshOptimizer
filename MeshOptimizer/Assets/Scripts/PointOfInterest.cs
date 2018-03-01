using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public string PoiDescription;
    [SerializeField]
    private Vector3 BoundingBoxCenter;
    [SerializeField]
    public Bounds BoundingBox;
    private GameObject DebugMarker = null;

    [SerializeField]
    private float currentPercentageOfScreenCovered;
    [SerializeField]
    private float percentageOnTransition;
    [SerializeField]
    private float levelChangeTimeout;
    [SerializeField]
    private float levelChangeTimeoutDelta = 2.0f;
    [SerializeField]
    private float currentTime;

    [SerializeField]
    private MeshRenderer MeshRendererPoi;
    [SerializeField]
    private ScreenSpaceRectangle screenSpaceRectangle;
    [SerializeField]
    private Color32 PoiColor;
    [SerializeField]
    private Canvas UILabelCanvas;
    [SerializeField]
    private GameObject LabelPrefab;
    private GameObject instantiatedLabel;
    [SerializeField]
    private Text canvasText;
    [SerializeField]
    private bool isAnimating = false;

    private void Awake()
    {
        //MeshRendererPoi = gameObject.GetComponent<MeshRenderer>();
        UILabelCanvas = GameObject.Find("UILabels").GetComponent<Canvas>();
        instantiatedLabel = Instantiate(LabelPrefab);
        canvasText = instantiatedLabel.GetComponentInChildren<Text>();
        canvasText.text = PoiDescription;
        instantiatedLabel.transform.SetParent(UILabelCanvas.transform);
        instantiatedLabel.transform.localScale = Vector3.one;
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
            EstabilishPoiColorForSelfAndChildren();
            active = true;
            PoiManager.Instance.RegisterTopLevelPoi(this);
            SetVisibilityOfAllChildrenPoi(false, true);
        }
        CalculateBoundingBox(false);
        StartCoroutine(UpdateLevelChangeTimeout());
    }

    // Update is called once per frame
    void Update () {
        if(active && !isAnimating) CheckScreenCovered();
        UpdateLabelPosition();
    }

    public void SetPointOfInterestEnabled(bool enabled)
    {
        //MeshRendererPoi.enabled = enabled;
        active = enabled;
        StartCoroutine(StartFadeInOrOut(enabled));

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
        foreach (PointOfInterest poi in ChildrenPoi)
        {
            poi.EstabilishPoiLevelForSelfAndChildren(LevelForCurrentPoi + 1);
        }
        PoiManager.Instance.SetMinLevel(PoiLevel);
    }
    public void EstabilishPoiColorForSelfAndChildren()
    {
        SetColor(PoiManager.Instance.GetColorForGivenPoiLevel(PoiLevel));
        foreach (PointOfInterest poi in ChildrenPoi)
        {
            poi.EstabilishPoiColorForSelfAndChildren();
        }
    }

    /*private void CheckCameraDistance()
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
    }*/

    public void SetVisibilityOfAllChildrenPoi(bool visible, bool recursive)
    {
        foreach(PointOfInterest poi in ChildrenPoi)
        {
            poi.SetPointOfInterestEnabled(visible);
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
                poi.SetPointOfInterestEnabled(visible);

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
        int totalChildren = 0;
        if (ChildrenPoi.Count == 0)
        {
            center = Vector3.zero;
            totalChildren = 1;
        }
        else
        {
            foreach (PointOfInterest poi in ChildrenPoi)
            {
                foreach(PointOfInterest poichild in poi.ChildrenPoi)
                {
                    center += poichild.transform.localPosition;
                    totalChildren++;
                }
                center += poi.transform.localPosition;
                totalChildren++;
            }
            center /= totalChildren;
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
            foreach(PointOfInterest poichild in poi.ChildrenPoi)
            {
                BoundingBox.Encapsulate(poichild.transform.position);
            }
        }
    }

    private void CheckScreenCovered()
    {
        if(currentPercentageOfScreenCovered >= 0.33f || 
            ( currentPercentageOfScreenCovered != 0 
            && (screenSpaceRectangle.min.z < 7.0f && screenSpaceRectangle.min.z > 0.0)))
        {
            MoveLevelDown();
        }
        if(PoiLevel != 0 && ParentPoi.currentPercentageOfScreenCovered < ParentPoi.percentageOnTransition 
            && ParentPoi.levelChangeTimeout == 0.0f && ParentPoi.currentPercentageOfScreenCovered != 0)
        {
            MoveLevelUp();
        }
    }

    private void MoveLevelDown()
    {
        if (PoiLevel != PoiManager.Instance.GetMinLevel())
        {
            percentageOnTransition = currentPercentageOfScreenCovered;
            levelChangeTimeout = Time.time + levelChangeTimeoutDelta;
            SetPointOfInterestEnabled(false);
            SetVisibilityOfAllChildrenPoi(true, false);
        }
    }

    private void MoveLevelUp()
    {
        if(PoiLevel != 0)
        {
            Debug.Log("ascending");
            SetPointOfInterestEnabled(false);
            SetVisibilityOfAllChildrenPoi(false, false);
            ParentPoi.SetPointOfInterestEnabled(true);
        }
    }

    private void SetColor(Color32 c)
    {
        PoiColor = c;
        MeshRendererPoi.material.color = PoiColor;
    }

    public Color32 GetColor()
    {
        return PoiColor;
    }

    public void SetCurrentPercentageOnScreenTaken(float p)
    {
        currentPercentageOfScreenCovered = p;
    }

    public float GetCurrentPercentageOnScreenTaken()
    {
        return currentPercentageOfScreenCovered;
    }

    private IEnumerator UpdateLevelChangeTimeout()
    {
        while (gameObject.activeInHierarchy)
        {
            currentTime = Time.time;
            if (levelChangeTimeout > 0.0f && levelChangeTimeout < currentTime)
            {
                levelChangeTimeout = 0.0f;
            }
            yield return null;
        }
    }

    private IEnumerator StartFadeInOrOut(bool makeVisible)
    {
        float startTime = Time.time;
        float finishTime = startTime + 1.0f;
        Color meshColor = MeshRendererPoi.material.color;
        Color textColor = canvasText.color;

        isAnimating = true;

        while(Time.time < finishTime)
        {
            if (makeVisible)
            {
                textColor.a += 1.0f / 10.0f;
                meshColor.a += 1.0f / 10.0f;
            } else
            {
                textColor.a -= 1.0f / 10.0f;
                meshColor.a -= 1.0f / 10.0f;
            }
            MeshRendererPoi.material.color = meshColor;
            canvasText.color = textColor;
            yield return new WaitForSeconds(0.05f);
        }
        if (makeVisible)
        {
            meshColor.a = 1.0f;
            textColor.a = 1.0f;
        }
        else
        {
            meshColor.a = 0.0f;
            textColor.a = 0.0f;
        }
        MeshRendererPoi.material.color = meshColor;
        canvasText.color = textColor;
        isAnimating = false;
    }

    private void UpdateLabelPosition()
    {
        //offset
        Vector3 worldPos = transform.position;
        worldPos.y += 0.5f; //offset
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        instantiatedLabel.transform.position = screenPos;
    }
}
