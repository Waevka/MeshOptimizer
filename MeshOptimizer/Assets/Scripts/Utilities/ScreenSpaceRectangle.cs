using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceRectangle : MonoBehaviour {
    [SerializeField]
    private PointOfInterest poi;
    GUIStyle RectangleStyle;
    GUIContent RectangleContent;
    [SerializeField]
    private Texture backgroundImage;
    public float percentageOnScreen;
    private Rect rectangle;

    public Vector3 max;
    public Vector3 min;
    // Use this for initialization

    void Start () {
        RectangleStyle = new GUIStyle();
        //RectangleStyle.border = new RectOffset(1,1,1,1);
        RectangleContent = new GUIContent();
        RectangleContent.image = backgroundImage;
        rectangle = Rect.zero;
        rectangle.size = new Vector2(1.0f, 1.0f);
	}

    private void OnGUI()
    {
        Vector2 center = Camera.main.WorldToScreenPoint(poi.BoundingBox.center);
        center.y = Screen.height - center.y;
        rectangle.center = center;
        if(max.z >= 0.0f && min.z >= 0.0f)
        {
            rectangle.xMin = Mathf.Clamp(min.x, 0.0f, Screen.width);
            rectangle.yMin = Mathf.Clamp(Screen.height - min.y, 0.0f, Screen.height);
            rectangle.xMax = Mathf.Clamp(max.x, 0.0f, Screen.width);
            rectangle.yMax = Mathf.Clamp(Screen.height - max.y, 0.0f, Screen.height);
            GUI.color = poi.GetColor();
            //GUI.Box(rectangle, RectangleContent/*, RectangleStyle*/);
            if (PoiManager.Instance.GetLevelBoundariesVisibility())
            {
                GUI.DrawTexture(rectangle, backgroundImage/*, backgroundImage, RectangleStyle*/);
            }
            percentageOnScreen = (rectangle.xMax - rectangle.xMin) * (rectangle.yMin - rectangle.yMax) / (Screen.width * Screen.height);
        } else
        {
            percentageOnScreen = 0.0f;
        }
        poi.SetCurrentPercentageOnScreenTaken(percentageOnScreen);
    }

    // Update is called once per frame
    void Update () {
        max = Camera.main.WorldToScreenPoint(poi.BoundingBox.max);
        min = Camera.main.WorldToScreenPoint(poi.BoundingBox.min);
    }
}
