using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour
{
    [SerializeField] private GameObject focusTarget = null;
    [SerializeField] private float transSmoothTime = 0.2f;
    [SerializeField] private float rotSmoothTime = 0.1f;
    [SerializeField] private float lagDistanceSpeedFactor = -1;
    [SerializeField] private bool useRot = false;
    private float xVel = 0.0f;
    private float zVel = 0.0f;
    private float rotVel = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.y += Input.mouseScrollDelta.y;
        transform.position = newPos;
    }

    private void FixedUpdate()
    {

        Vector3 newPos = transform.position;
        newPos.x = Mathf.SmoothDamp(transform.position.x, focusTarget.transform.position.x, ref xVel, transSmoothTime);
        newPos.z = Mathf.SmoothDamp(transform.position.z, focusTarget.transform.position.z, ref zVel, transSmoothTime);
        

        if(useRot) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, focusTarget.transform.rotation.eulerAngles.y, ref rotVel, rotSmoothTime), 0);

        transform.position = newPos;
    }
}
