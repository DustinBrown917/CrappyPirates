using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    public Vector3 preferredRotation = new Vector3();

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(preferredRotation);
    }
}
