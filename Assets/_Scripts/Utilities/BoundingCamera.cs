using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BoundingCamera : MonoBehaviour
{
    private List<ICameraFocusObject> boundingObjects = new List<ICameraFocusObject>();
    private Camera cam = null;
    [SerializeField] private float maxY = 20.0f;
    [SerializeField] private float minY = 10.0f;
    [SerializeField] private float extraOffset = 1.2f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        transform.position = CenterOnObjects();
    }

    public void AddBoundingObject(ICameraFocusObject obj)
    {
        boundingObjects.Add(obj);
    }

    public Vector3 CenterOnObjects()
    {
        if (boundingObjects.Count == 0) {
            return transform.position;
        }


        Vector3 bottomLeft = boundingObjects[0].Position;
        Vector3 topRight = boundingObjects[0].Position;

        for(int i = 1; i < boundingObjects.Count; i++) {
            Vector3 bop = boundingObjects[i].Position;

            if(bop.z > topRight.z) { topRight.z = bop.z; }
            else if(bop.z < bottomLeft.z) { bottomLeft.z = bop.z; }

            if (bop.x > topRight.x) { topRight.x = bop.x; } 
            else if (bop.x < bottomLeft.x) { bottomLeft.x = bop.x; }
        }

        float newY = (topRight - bottomLeft).magnitude  *extraOffset;

        return new Vector3((topRight.x + bottomLeft.x) * 0.5f, Mathf.Clamp(newY, minY, maxY), (topRight.z + bottomLeft.z) * 0.5f);
    }
}
