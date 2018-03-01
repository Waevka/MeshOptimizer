using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxGizmo : MonoBehaviour {

    public PointOfInterest poi;
    private void OnDrawGizmos()
    {
        Gizmos.color = poi.GetColor();
        if (poi.BoundingBox != null)
        {
            Gizmos.DrawWireCube(poi.BoundingBox.center, poi.BoundingBox.size);
        }
    }
    
}
