using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxObject : MonoBehaviour
{
    [SerializeField] private float paralaxFactor = -0.5f;
    [SerializeField] private Transform relativeTo = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = relativeTo.position.x * paralaxFactor;
        newPos.y = relativeTo.position.y * paralaxFactor;

        transform.position = newPos;
    }
}
