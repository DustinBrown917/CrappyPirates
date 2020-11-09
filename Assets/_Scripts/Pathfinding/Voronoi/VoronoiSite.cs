using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiSite : MonoBehaviour
{
    public Vector2 location { get => new Vector2(transform.position.x, transform.position.z); }

    private VoronoiRegion region = null;
}
