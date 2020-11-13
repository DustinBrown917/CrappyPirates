using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class ForuneSiteMono : MonoBehaviour
{
    public bool transposeZ = false;
    public FortuneSite GetSite(bool zBased = false)
    {
        if (zBased) {
            return new FortuneSite(transform.position.x, transform.position.z);
        } else {
            return new FortuneSite(transform.position.x, transform.position.y);
        }
    } 
}
